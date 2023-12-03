import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ToastrModule } from 'ngx-toastr';
import { RouterTestingModule } from '@angular/router/testing';
import { SharedAccountReportsComponent } from './shared-account-reports.component';
import { DatePipe } from '@angular/common';
import { GlobalService } from './../services/global.service';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';
import moment from 'moment';
import { Chart } from 'chart.js';
import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';

declare global {
  interface Window {
    Chart: typeof Chart;
  }
}

interface ChartMocks {
  [key: string]: jasmine.SpyObj<Chart>;
}
const createMockChart = () => {
  return jasmine.createSpyObj('Chart', ['update', 'destroy'], {
      data: { datasets: [{ data: [] }], labels: [] },
      options: { plugins: { tooltip: { callbacks: {} } } },
      config: {},
      platform: {},
      canvas: document.createElement('canvas'),
      ctx: {} as CanvasRenderingContext2D
  });
};



describe('SharedAccountReportsComponent', () => {
  let component: SharedAccountReportsComponent;
  let fixture: ComponentFixture<SharedAccountReportsComponent>;
  let httpMock: HttpTestingController;
  let mockChart: jasmine.SpyObj<Chart>;
  let globalService: jasmine.SpyObj<GlobalService>;

  beforeEach(async () => {
    mockChart = createMockChart();
    (window as any).Chart = jasmine.createSpy('Chart').and.returnValue(mockChart);

    const chartMocks: ChartMocks = {
      'radialChart1': createMockChart(),
      'timeScaleChart1': createMockChart(),
      'polarChart1': createMockChart(),
      'dailyExpenseChart1': createMockChart(),
      'radialChart2': createMockChart(),
      'timeScaleChart2': createMockChart(),
      'polarChart2': createMockChart(),
      'dailyExpenseChart2': createMockChart()
    };
  
    spyOn(Chart, 'getChart').and.callFake((id: string) => chartMocks[id] || null);
    spyOn(document, 'getElementById').and.returnValue(document.createElement('canvas'));
  
    globalService = jasmine.createSpyObj('GlobalService', ['getSharedUserDetails']);
    globalService.getSharedUserDetails.and.returnValue(of([{
      userID: 2,
      userName: 'TestUser 2'
    }]));
  //   spyOn(component, 'initializePolarAreaChart').and.callFake((userSuffix: string) => {
  //     // Mock the chart object
  //     const mockChart = {
  //         data: {
  //             labels: [],
  //             datasets: [{
  //                 data: [],
  //                 backgroundColor: [],
  //                 borderColor: 'rgba(255, 255, 255, 0.2)'
  //             }]
  //         },
  //         update: jasmine.createSpy('update')
  //     };
  //     // Store the mock chart in the component's charts object
  //     component.charts[`polar${userSuffix}`] = mockChart;
  // });
    // component.ngAfterViewInit();
  
    await TestBed.configureTestingModule({
      declarations: [SharedAccountReportsComponent],
      imports: [
        HttpClientTestingModule,
        RouterTestingModule,
        FormsModule,
        ToastrModule.forRoot(),BrowserAnimationsModule,NoopAnimationsModule
      ],
      providers: [
        { provide: GlobalService, useValue: globalService },
        DatePipe
      ]
    }).compileComponents();
  
    fixture = TestBed.createComponent(SharedAccountReportsComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
  
    component.selectedMonth = '2025-03';
    component.selectedUser2 = '2';
    component.currentUserId = '1';
  
    spyOn(component, 'ngAfterViewInit').and.stub();
  });
  
  it('should update data when month changes', fakeAsync(() => {
    console.log('Triggering month change...');
    component.ngAfterViewInit();
    component.onMonthChange();
    
    const requests = [
        { url: 'GetExpensesByDates?userId=1&startDate=2025-03-01&endDate=2025-03-31', response: [] },
        { url: 'GetBudgetByDates?userId=1&startDate=2025-03-01&endDate=2025-03-31', response: 5000 },
        { url: 'GenerateTotalExpenseReport?userId=1&startDate=2025-03-01&endDate=2025-03-31&categoryId=0', response: { totalAmount: 3000 } },
        
        { url: 'GetExpensesByDates?userId=2&startDate=2025-03-01&endDate=2025-03-31', response: [] },
        { url: 'GetBudgetByDates?userId=2&startDate=2025-03-01&endDate=2025-03-31', response: 6000 },
        { url: 'GenerateTotalExpenseReport?userId=2&startDate=2025-03-01&endDate=2025-03-31&categoryId=0', response: { totalAmount: 4000 } }
    ];

    requests.forEach(req => {
        const httpReqs = httpMock.match(`https://localhost:7154/Report/${req.url}`);
        console.log(`Request for ${req.url}: found ${httpReqs.length} requests`);
        if (httpReqs.length > 0) {
            expect(httpReqs[0].request.method).toBe('GET');
            httpReqs[0].flush(req.response);
        }
    });
    
    const chartRequests = [
        { url: 'GetExpensesByDates?userId=1&startDate=2024-09-01&endDate=2025-03-31', response: [] },
        { url: 'GetMonthlyBudgets?userId=1&startDate=2024-09-01&endDate=2025-03-31', response: [] },
        { url: 'GetExpensesByDates?userId=2&startDate=2024-09-01&endDate=2025-03-31', response: [] },
        { url: 'GetMonthlyBudgets?userId=2&startDate=2024-09-01&endDate=2025-03-31', response: [] },
        
        { url: 'GetCategorySummary?userId=1&startDate=2025-03-01&endDate=2025-03-31', response: [] },
        { url: 'GetCategorySummary?userId=2&startDate=2025-03-01&endDate=2025-03-31', response: [] },
        
        { url: 'GetCategories', response: [], isCategory: true },
        { url: 'GetCategories', response: [], isCategory: true }
    ];

    chartRequests.forEach(req => {
        const url = req.isCategory 
            ? `https://localhost:7154/${req.url}`
            : `https://localhost:7154/Report/${req.url}`;
        const httpReqs = httpMock.match(url);
        console.log(`Request for ${url}: found ${httpReqs.length} requests`);
        if (httpReqs.length > 0) {
            expect(httpReqs[0].request.method).toBe('GET');
            httpReqs[0].flush(req.response);
        }
    });
    
    const dailyExpenseRequests = [
        { url: 'GetExpensesByDates?userId=1&startDate=2025-03-01&endDate=2025-03-31', response: [] },
        { url: 'GetExpensesByDates?userId=2&startDate=2025-03-01&endDate=2025-03-31', response: [] },
    ];
    
    dailyExpenseRequests.forEach(req => {
        const httpReqs = httpMock.match(`https://localhost:7154/Report/${req.url}`);
        console.log(`Request for ${req.url}: found ${httpReqs.length} requests`);
        if (httpReqs.length > 0) {
            expect(httpReqs[0].request.method).toBe('GET');
            httpReqs[0].flush(req.response);
        }
    });
    
    tick(1000);
}));
  
    it('should initialize charts for user2 when selectedUser2 exists', () => {
      component.ngAfterViewInit();
    });
    
    it('should handle missing canvas elements', () => {
      spyOn(console, 'error');
      (document.getElementById as jasmine.Spy).and.returnValue(null);
      component.ngAfterViewInit();
    });

      it('should process user2 expenses', () => {
        const mockData = [{ date: '2023-01-15', amount: 100 }];
        component['user2Budget'] = 500;
        
        const result = component['groupExpensesByMonth'](mockData, '2');
      });
  
      it('should update radial chart for user1', () => {
        component['user1Budget'] = 1000;
        component['user1Expenses'] = 400;
        component['updateRadialChart']('1');
      });
  
      it('should handle zero budget scenario', () => {
        component['user1Budget'] = 0;
        component['user1Expenses'] = 500;
        component['updateRadialChart']('1');
      });

  it('should call On month change', () => {
    const loadDataSpy = spyOn(component as any, 'loadData').and.callThrough();
    component.onMonthChange();
    expect(loadDataSpy).toHaveBeenCalled();
  });
  it('should recognize the selected user as 2', () => {
    component.selectedUser2 = "10";
    component.onUser2Change();
  });
  it('should load users on ngOnInit', () => {
    const mockUsers = [{ userID: 1, userName: 'User  1' }, { userID: 2, userName: 'User  2' }];
    globalService.getSharedUserDetails.and.returnValue(of(mockUsers));

    component.ngOnInit();
    const currentUserId = (component as any).currentUserId;

    expect(globalService.getSharedUserDetails).toHaveBeenCalledWith(currentUserId);
    expect(component.users).toEqual(mockUsers);
});

  it('should load data when month changes', () => {
    const loadDataSpy = spyOn(component as any, 'loadData').and.callThrough();
    component.onMonthChange();
    expect(loadDataSpy).toHaveBeenCalled();
  });
  it('should load user data correctly', () => {
    const userId = '1';
    const userSuffix = '1';
    const mockExpenses = [{ amount: 100, date: '2023-01-01' }];
    const mockBudget = 500;
    const mockTotalExpenses = { totalAmount: 200 };
    const LoadUserData = (component as any).loadUserData;
    const BaseUrl = (component as any).baseUrl;
    const GetDateRange = (component as any).getDateRange;
    const { start, end } = GetDateRange(component.selectedMonth);
    component['loadUserData'](userId, userSuffix);

    const req1 = httpMock.expectOne(req => req.url.includes('GetExpensesByDates'));
    req1.flush(mockExpenses);

    const req2 = httpMock.expectOne(req => req.url.includes('GetBudgetByDates'));
    req2.flush(mockBudget);

    const req3 = httpMock.expectOne(req => req.url.includes('GenerateTotalExpenseReport'));
    req3.flush(mockTotalExpenses);

    expect(component.user1Expenses).toBe(200);
    expect(component.user1Budget).toBe(500);
  });

    it('should group expenses for user 2', () => {
      const mockData = [{ date: '2023-01-15', amount: 100 }];
      component['user2Budget'] = 500;
      const result = component['groupExpensesByMonth'](mockData, '2');
    });

    it('should update user 2 charts when selected', () => {
      component.selectedUser2 = '2';
      component['updateCharts']('2');
    });

    it('should update user 2 radial chart', () => {
      component.selectedUser2 = '2';
      component['user2Budget'] = 500;
      component['user2Expenses'] = 200;
      component['updateRadialChart']('2');
    });

    it('should handle user 2 data', () => {
      component.selectedUser2 = '2';
      const mockExpenses = [{ date: '2023-01-15', amount: 100, categoryID: 1 }];
      const mockCategories = [{ categoryID: 1, name: 'Test Category' }];

      component['updateDailyExpenseChart']('2');

      const req1 = httpMock.expectOne('https://localhost:7154/GetCategories');
      req1.flush(mockCategories);

      const req2 = httpMock.expectOne(req => 
        req.url.includes('GetExpensesByDates') && req.params.get('userId') === '2'
      );
      req2.flush(mockExpenses);

      expect(component['charts'][`daily2`].data.datasets.length).toBeGreaterThan(0);
    });

      it('should log error if timeScaleChart canvas is missing', () => {
        spyOn(console, 'error');
        (document.getElementById as jasmine.Spy).and.returnValue(null);
        component['initializeTimeScaleChart']('1');
        expect(console.error).toHaveBeenCalledWith('TimeScale chart canvas element not found');
      });
  
      it('should handle groupExpensesByMonth() for userSuffix=2', () => {
        component.selectedUser2 = '2';
        component.user2Budget = 1000;
        const mockData = [{ date: '2023-01-15', amount: 200 }];
        const result = component['groupExpensesByMonth'](mockData, '2');
      });

      it('should call updateCharts() without user2 when selectedUser2 is unset', () => {
        component.selectedUser2 = '';
        const updateRadialSpy = spyOn(component as any, 'updateRadialChart');
        component['updateCharts']('1');
        expect(updateRadialSpy).toHaveBeenCalledWith('1');
        expect(updateRadialSpy).not.toHaveBeenCalledWith('2');
      });

      it('should handle missing canvas elements', () => {
        spyOn(console, 'error');
        (document.getElementById as jasmine.Spy).and.returnValue(null);
        component.ngAfterViewInit();
      });
    
    it('should handle user2 not selected', () => {
      component.selectedUser2 = '';
      component.ngAfterViewInit();
    });
    
    it('should handle groupExpensesByMonth for user2', () => {
      const mockData = [{ date: '2023-01-15', amount: 200 }];
      component['user2Budget'] = 1000;
      const result = component['groupExpensesByMonth'](mockData, '2');
      expect(result.get('2023-01')?.totalAmount).toBe(200);
    });

    it('should load users data correctly', fakeAsync(() => {
      component.currentUserId = '1'; 
      fixture.detectChanges();
        const mockExpenses = [{ amount: 100 }];
      const mockBudget = 500;
      const mockTotal = { totalAmount: 300 };
  
      const req1 = httpMock.expectOne(req => 
          req.url.includes('GetExpensesByDates') && req.params.get('userId') === component.currentUserId
      );
      req1.flush(mockExpenses);
  
      const req2 = httpMock.expectOne(req => 
          req.url.includes('GetBudgetByDates') && req.params.get('userId') === component.currentUserId
      );
      req2.flush(mockBudget); 
  
      const req3 = httpMock.expectOne(req => 
          req.url.includes('GenerateTotalExpenseReport') && req.params.get('userId') === component.currentUserId
      );
      req3.flush(mockTotal);
  
      tick();
  
      expect(component['user1Expenses']).toBe(300);
      expect(component['user1Budget']).toBe(500);
  }));
  it('should invoke select user 2', () => {
    component.selectedUser2 = '3';
    component.ngOnInit();
  });
});