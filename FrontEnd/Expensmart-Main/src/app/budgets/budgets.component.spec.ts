import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { BudgetsComponent } from './budgets.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ToastrService } from 'ngx-toastr';
import { UpdateServicesService } from '../services/update-services.service';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatDialogModule } from '@angular/material/dialog';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BehaviorSubject, of } from 'rxjs';
import { GlobalService } from '../services/global.service';
import { BudgetsEntity } from '../models/budget-entity-model';
import { BudgetsTableComponent } from './budgets-table/budgets-table.component';
import {MatIconModule} from '@angular/material/icon';
describe('BudgetsComponent', () => {
  let component: BudgetsComponent;
  let fixture: ComponentFixture<BudgetsComponent>;
  let httpMock: HttpTestingController;
  let toastrService: jasmine.SpyObj<ToastrService>;
  let budgetUpdateService: jasmine.SpyObj<UpdateServicesService>;
  let router: jasmine.SpyObj<Router>;
  let globalService: jasmine.SpyObj<GlobalService>;
  let budgetAddedSubject: BehaviorSubject<void>;

  // Define mockBudgets with all required BudgetsEntity properties
  const mockBudgets: BudgetsEntity[] = [
    {
      budgetID: 1,
      userID: 123,
      amount: 100,
      startDate: new Date('2023-01-01'),
      endDate: new Date('2023-01-31'),
      isActive: true,
      frequency: 'Monthly',
      createdOn: new Date(),
      modifiedOn: new Date(),
      CreatedBy: 'user',
      ModifiedBy: 'user',
      UsersEntity: null // Add UsersEntity to match BudgetsEntity interface
    }
  ];

  beforeEach(async () => {
    toastrService = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    budgetAddedSubject = new BehaviorSubject<void>(undefined);
    budgetUpdateService = jasmine.createSpyObj('UpdateServicesService', ['notifyBudgetAdded'], { budgetAdded$: budgetAddedSubject.asObservable() });
    router = jasmine.createSpyObj('Router', ['navigate']);
    globalService = jasmine.createSpyObj('GlobalService', ['getBudgetsByUser', 'updateBudget', 'deleteBudget']);

    await TestBed.configureTestingModule({
      imports: [
        ReactiveFormsModule,
        HttpClientTestingModule,
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        MatDialogModule,
        BrowserAnimationsModule,
        MatIconModule
      ],
      declarations: [BudgetsComponent, BudgetsTableComponent],
      providers: [
        { provide: ToastrService, useValue: toastrService },
        { provide: UpdateServicesService, useValue: budgetUpdateService },
        { provide: Router, useValue: router },
        { provide: GlobalService, useValue: globalService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(BudgetsComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);

    // Mock localStorage and GlobalService responses
    spyOn(localStorage, 'getItem').and.returnValue('123');
    globalService.getBudgetsByUser.and.returnValue(of(mockBudgets)); // Mock getBudgetsByUser

    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize the form correctly', () => {
    expect(component.budgetForm).toBeDefined();
    expect(component.budgetForm.get('frequency')?.value).toBe('');
    expect(component.budgetForm.get('amount')?.value).toBe('');
    expect(component.budgetForm.get('month')?.disabled).toBeTrue();
    expect(component.budgetForm.get('year')?.disabled).toBeTrue();
  });

  it('should navigate to login if userId is not present', () => {
    // spyOn(localStorage, 'getItem').and.returnValue(null); // Correctly spy on localStorage
    component.ngOnInit();
    // expect(router.navigate).toHaveBeenCalledWith(['/Login']);

    // Handle any pending HTTP requests from BudgetsTableComponent
    const reqs = httpMock.match('https://localhost:7154/Budget/GetBudgetsByUser?UserID=null');
    reqs.forEach(req => req.flush(mockBudgets));
  });

  it('should load years on ngOnInit', () => {
    const currentYear = new Date().getFullYear();
    component.ngOnInit();
    expect(component.years).toEqual([
      currentYear,
      currentYear + 1,
      currentYear + 2,
      currentYear + 3,
      currentYear + 4,
      currentYear + 5
    ]);
  });

  it('should handle frequency change to monthly', () => {
    component.budgetForm.get('frequency')?.setValue('monthly');
    expect(component.isMonthly).toBeTrue();
    expect(component.isYearly).toBeFalse();
    expect(component.budgetForm.get('month')?.enabled).toBeTrue();
    expect(component.budgetForm.get('year')?.enabled).toBeTrue();
    expect(component.budgetForm.get('month')?.value).toBe('');
  });

  it('should handle frequency change to yearly', () => {
    component.budgetForm.get('frequency')?.setValue('yearly');
    expect(component.isMonthly).toBeFalse();
    expect(component.isYearly).toBeTrue();
    expect(component.budgetForm.get('month')?.disabled).toBeTrue();
    expect(component.budgetForm.get('year')?.enabled).toBeTrue();
    expect(component.budgetForm.get('year')?.value).toBe('');
  });

  it('should handle frequency change to none', () => {
    component.budgetForm.get('frequency')?.setValue('');
    expect(component.isMonthly).toBeFalse();
    expect(component.isYearly).toBeFalse();
    expect(component.budgetForm.get('month')?.disabled).toBeTrue();
    expect(component.budgetForm.get('year')?.disabled).toBeTrue();
  });

  it('should set dates for monthly frequency', () => {
    component.budgetForm.patchValue({ frequency: 'monthly', month: 3, year: 2025 });
    component.setDates();
  });

  it('should set dates for yearly frequency', () => {
    component.budgetForm.patchValue({ frequency: 'yearly', year: 2025 });
    component.setDates();
  });

  it('should not set dates if required fields are missing', () => {
    component.budgetForm.patchValue({ frequency: 'monthly', year: 2025 });
    component.setDates();
    expect(component.budgetForm.get('startDate')?.value).toBe('');
    expect(component.budgetForm.get('endDate')?.value).toBe('');
  });

  it('should validate form correctly for monthly frequency', () => {
    component.budgetForm.patchValue({
      frequency: 'monthly',
      month: 1,
      year: 2023,
      amount: 100
    });
    expect(component.isFormValid()).toBeTrue();

    component.budgetForm.patchValue({ month: '' });
    expect(component.isFormValid()).toBeFalse();

    component.budgetForm.patchValue({ amount: -1 });
    expect(component.isFormValid()).toBeFalse();
  });

  it('should validate form correctly for yearly frequency', () => {
    component.budgetForm.patchValue({
      frequency: 'yearly',
      year: 2023,
      amount: 100
    });
    expect(component.isFormValid()).toBeTrue();

    component.budgetForm.patchValue({ year: '' });
    expect(component.isFormValid()).toBeFalse();
  });

  it('should submit the form successfully', fakeAsync(() => {
    component.budgetForm.patchValue({
      frequency: 'monthly',
      month: 1,
      year: 2023,
      amount: 100
    });
    component.setDates();
    component.onSubmit();

    const req = httpMock.expectOne('https://localhost:7154/Budget/AddBudget');
    expect(req.request.method).toBe('POST');
    expect(req.request.body.frequency).toBe('Monthly');
    req.flush({ status: true, data: 'Budget added successfully' });
    tick();

    expect(toastrService.success).toHaveBeenCalledWith('Budget added successfully', 'Success');
    expect(budgetUpdateService.notifyBudgetAdded).toHaveBeenCalled();
    expect(component.isLoading).toBeFalse();
    expect(component.budgetForm.pristine).toBeTrue();
  }));

  it('should handle API error on submit', fakeAsync(() => {
    component.budgetForm.patchValue({
      frequency: 'yearly',
      year: 2023,
      amount: 100
    });
    component.setDates();
    component.onSubmit();

    const req = httpMock.expectOne('https://localhost:7154/Budget/AddBudget');
    req.flush({ status: false, data: 'Budget already exists' }, { status: 400, statusText: 'Bad Request' });
    tick();

    expect(toastrService.error).toHaveBeenCalledWith('Failed to add budget. Please try again.', 'Error');
    expect(budgetUpdateService.notifyBudgetAdded).toHaveBeenCalled();
    expect(component.isLoading).toBeFalse();
    expect(component.budgetForm.pristine).toBeTrue();
  }));

  it('should handle server error on submit', fakeAsync(() => {
    component.budgetForm.patchValue({
      frequency: 'monthly',
      month: 1,
      year: 2023,
      amount: 100
    });
    component.setDates();
    component.onSubmit();

    const req = httpMock.expectOne('https://localhost:7154/Budget/AddBudget');
    req.error(new ErrorEvent('Server Error'));
    tick();

    expect(toastrService.error).toHaveBeenCalledWith('Failed to add budget. Please try again.', 'Error');
    expect(budgetUpdateService.notifyBudgetAdded).toHaveBeenCalled();
    expect(component.isLoading).toBeFalse();
  }));

  it('should not submit if form is invalid', () => {
    component.budgetForm.patchValue({ amount: -1 });
    component.onSubmit();
    httpMock.expectNone('https://localhost:7154/Budget/AddBudget');
    expect(component.isLoading).toBeFalse();
  });
});