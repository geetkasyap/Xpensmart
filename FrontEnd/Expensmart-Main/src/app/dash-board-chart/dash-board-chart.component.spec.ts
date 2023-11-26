import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { DashBoardChartComponent } from './dash-board-chart.component';
import { ExpenseService } from '../services/expense.service';
import { AuthService } from '../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { ChangeDetectorRef } from '@angular/core';
import { Chart } from 'chart.js/auto';
import { of, throwError } from 'rxjs';

describe('DashBoardChartComponent', () => {
  let component: DashBoardChartComponent;
  let fixture: ComponentFixture<DashBoardChartComponent>;
  let expenseService: jasmine.SpyObj<ExpenseService>;
  let authService: jasmine.SpyObj<AuthService>;
  let toastrService: jasmine.SpyObj<ToastrService>;
  let cdr: jasmine.SpyObj<ChangeDetectorRef>;
  let localStorageSpy: jasmine.Spy;

  const mockExpenses: any[] = [
    { expenseID: 1, amount: 100, date: '2025-03-01', categoryID: 1, description: 'Food expense' },
    { expenseID: 2, amount: 200, date: '2025-03-02', categoryID: 2, description: 'Travel expense' }
  ];
  const mockCategories: any[] = [
    { categoryID: 1, name: 'Food' },
    { categoryID: 2, name: 'Travel' }
  ];

  beforeEach(async () => {
    expenseService = jasmine.createSpyObj('ExpenseService', ['getExpensesByDateString', 'getCategories']);
    authService = jasmine.createSpyObj('AuthService', [], { userLoggedIn$: of(true) });
    toastrService = jasmine.createSpyObj('ToastrService', ['error']);
    cdr = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);

    await TestBed.configureTestingModule({
      declarations: [DashBoardChartComponent],
      providers: [
        { provide: ExpenseService, useValue: expenseService },
        { provide: AuthService, useValue: authService },
        { provide: ToastrService, useValue: toastrService },
        { provide: ChangeDetectorRef, useValue: cdr }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DashBoardChartComponent);
    component = fixture.componentInstance;

    // Mock ViewChild elements with a proper canvas context
    component.expenseCanvas = {
      nativeElement: {
        getContext: () => ({
          addEventListener: jasmine.createSpy('addEventListener'),
          removeEventListener: jasmine.createSpy('removeEventListener')
        })
      }
    } as any;
    component.monthSelect = { value: 2 } as any;

    // Spy on localStorage once, with configurable return value
    localStorageSpy = spyOn(localStorage, 'getItem').and.returnValue('123');

    // Spy on private methods
    spyOn(component as any, 'loadData').and.callThrough();
    spyOn(component as any, 'loadCategories').and.callThrough();
    spyOn(component as any, 'processDataAndCreateChart').and.callThrough();
    spyOn(component as any, 'createChart').and.callThrough();

    // Mock Chart constructor to avoid real instantiation
    // spyOn(Chart, 'Chart').and.callFake((ctx, config) => {
    //   return {
    //     data: config.data,
    //     destroy: jasmine.createSpy('destroy'),
    //     update: jasmine.createSpy('update')
    //   } as any;
    // });
  });

  afterEach(() => {
    fixture.destroy();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with default selectedMonth and months list', () => {
    expect(component.selectedMonth).toBe(new Date().getMonth());
    expect(component.months).toEqual([
      'January', 'February', 'March', 'April', 'May', 'June',
      'July', 'August', 'September', 'October', 'November', 'December'
    ]);
  });

  it('should call loadData on ngOnInit', () => {
    component.ngOnInit();
    expect(component['loadData']).toHaveBeenCalled();
  });

  it('should reset chart data when user is not logged in', () => {
    authService.userLoggedIn$ = of(false);
    spyOn(component as any, 'resetChartData');
    component.ngOnInit();
    expect(component['resetChartData']).toHaveBeenCalled();
    expect(component['loadData']).not.toHaveBeenCalled();
  });

  it('should unsubscribe and destroy chart on ngOnDestroy', () => {
    const sub = jasmine.createSpyObj('Subscription', ['unsubscribe']);
    component['subscriptions'] = [sub];
    (component as any).chart = { destroy: jasmine.createSpy('destroy') };
    component.ngOnDestroy();
    expect(sub.unsubscribe).toHaveBeenCalled();
    expect((component as any).chart.destroy).toHaveBeenCalled();
  });

  it('should reset chart data correctly', () => {
    component.expenses = mockExpenses;
    component.categories = mockCategories;
    component.hasData = true;
    component.isLoading = false;
    component.resetChartData();
    expect(component.expenses).toEqual([]);
    expect(component.categories).toEqual([]);
    expect(component.hasData).toBeFalse();
    expect(component.isLoading).toBeTrue();
  });

  it('should load data and handle expenses successfully', fakeAsync(() => {
    expenseService.getExpensesByDateString.and.returnValue(of(mockExpenses));
    component['loadData']();
    tick();
    expect(component.isLoading).toBeTrue(); // Initially true
    expect(expenseService.getExpensesByDateString).toHaveBeenCalled();
    expect(component.expenses).toEqual(mockExpenses);
    expect(component['loadCategories']).toHaveBeenCalled();
    expect(cdr.detectChanges).toHaveBeenCalled();
    expect(component.isLoading).toBeFalse();
  }));

  it('should handle error when loading expenses', fakeAsync(() => {
    expenseService.getExpensesByDateString.and.returnValue(throwError(() => new Error('Error')));
    component['loadData']();
    tick();
    expect(toastrService.error).toHaveBeenCalledWith('Error loading expenses:', 'error');
    expect(component.isLoading).toBeFalse();
  }));

  it('should not load data if UserID is null', fakeAsync(() => {
    localStorageSpy.and.returnValue(null); // Modify existing spy return value
    component['loadData']();
    tick();
    expect(expenseService.getExpensesByDateString).not.toHaveBeenCalled();
    expect(component.isLoading).toBeFalse();
  }));

  it('should load categories and process data', fakeAsync(() => {
    expenseService.getCategories.and.returnValue(of(mockCategories));
    component['loadCategories']();
    tick();
    expect(expenseService.getCategories).toHaveBeenCalled();
    expect(component.categories).toEqual(mockCategories);
    expect(component['processDataAndCreateChart']).toHaveBeenCalled();
  }));

  it('should handle error when loading categories', fakeAsync(() => {
    expenseService.getCategories.and.returnValue(throwError(() => new Error('Error')));
    component['loadCategories']();
    tick();
    expect(toastrService.error).toHaveBeenCalledWith('Error loading categories:', 'error');
  }));

  it('should handle month change with valid value', () => {
    const chartDestroySpy = jasmine.createSpy('destroy');
    (component as any).chart = { destroy: chartDestroySpy };
    const event = { target: { value: '3' } };
    component.onMonthChange(event);
    expect(component.selectedMonth).toBe(3);
    expect(chartDestroySpy).toHaveBeenCalled();
    expect(component.hasData).toBeFalse();
    expect(component['processDataAndCreateChart']).toHaveBeenCalled();
    expect((component as any).chart).toBeUndefined(); // Chart is set to undefined after destroy
  });

  it('should handle invalid month change', () => {
    const event = { target: { value: 'invalid' } };
    component.onMonthChange(event);
    expect(toastrService.error).toHaveBeenCalledWith('Invalid month selected:', 'invalid');
    expect(component.selectedMonth).not.toBeNaN();
  });

  it('should process data and create chart with no data', () => {
    component.expenses = [];
    component.categories = [];
    component['processDataAndCreateChart']();
    expect(component.hasData).toBeFalse();
  });

  it('should process data and create chart with valid data', () => {
    component.expenses = mockExpenses;
    component.categories = mockCategories;
    component.selectedMonth = 2; // March (0-based index)
    component['processDataAndCreateChart']();
    expect(component.hasData).toBeTrue();
    expect(component['createChart']).toHaveBeenCalledWith(jasmine.any(Array));
  });

  it('should not create chart if no monthly expenses', () => {
    component.expenses = [{ ...mockExpenses[0], date: '2025-04-01' }]; // Different month
    component.categories = mockCategories;
    component.selectedMonth = 2; // March
    component['processDataAndCreateChart']();
    expect(component.hasData).toBeFalse();
  });

  it('should create chart with valid data', () => {
    const mockData = [
      { day: 1, Food: 100, Travel: 0 },
      { day: 2, Food: 0, Travel: 200 }
    ];
    component.categories = mockCategories;
    component['createChart'](mockData);
    expect(component.hasData).toBeTrue();
    expect((component as any).chart).toBeDefined();
    expect((component as any).chart.data.labels).toEqual([1, 2]);
    expect((component as any).chart.data.datasets.length).toBe(2);
  });

  it('should not create chart with empty data', () => {
    component['createChart']([]);
    expect(component.hasData).toBeFalse();
    expect((component as any).chart).toBeUndefined();
  });

  it('should destroy existing chart before creating new one', () => {
    const chartDestroySpy = jasmine.createSpy('destroy');
    (component as any).chart = { destroy: chartDestroySpy };
    const mockData = [{ day: 1, Food: 100 }];
    component.categories = mockCategories;
    component['createChart'](mockData);
    expect(chartDestroySpy).toHaveBeenCalled();
  });
});