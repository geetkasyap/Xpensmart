import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { AddRecurringExpenseComponent } from './add-recurring-expense.component';
import { GlobalService } from './../services/global.service';
import { UpdateServicesService } from '../services/update-services.service';
import { BehaviorSubject, of, throwError } from 'rxjs';
import { ViewRecurringExpensesComponent } from '../view-recurring-expenses/view-recurring-expenses.component';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatChipsModule } from '@angular/material/chips';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSortModule } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';

describe('AddRecurringExpenseComponent', () => {
  let component: AddRecurringExpenseComponent;
  let fixture: ComponentFixture<AddRecurringExpenseComponent>;
  let toastrService: jasmine.SpyObj<ToastrService>;
  let globalService: jasmine.SpyObj<GlobalService>;
  let updateService: jasmine.SpyObj<UpdateServicesService>;
  let router: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    toastrService = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    globalService = jasmine.createSpyObj('GlobalService', ['loadCategories', 'convertCurrency', 'addRecurringTransaction', 'uploadFile']);
    updateService = jasmine.createSpyObj('UpdateServicesService', ['notifyExpenseAdded']);
    router = jasmine.createSpyObj('Router', ['navigate']);
    globalService.convertCurrency.and.returnValue(of(75));
    const expenseAddedSubject = new BehaviorSubject<void>(undefined);
    updateService.expenseAdded$ = expenseAddedSubject.asObservable();

    await TestBed.configureTestingModule({
      imports: [
        ReactiveFormsModule,
        HttpClientTestingModule,
        MatFormFieldModule, 
        MatInputModule,
        MatChipsModule,
        MatPaginatorModule,
                MatSelectModule,FormsModule,MatTableModule,BrowserAnimationsModule,MatSortModule,
                MatIconModule,
                MatDatepickerModule
      ],
      declarations: [AddRecurringExpenseComponent, ViewRecurringExpensesComponent],
      providers: [
        { provide: ToastrService, useValue: toastrService },
        { provide: GlobalService, useValue: globalService },
        { provide: UpdateServicesService, useValue: updateService },
        { provide: Router, useValue: router }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AddRecurringExpenseComponent);
    component = fixture.componentInstance;

    const categories = [{ categoryID: 1, name: 'Food', isActive: true, createdOn: new Date(), modifiedOn: new Date() }];
    globalService.loadCategories.and.returnValue(of(categories));

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with default values', () => {
    expect(component.recurringExpenseForm).toBeDefined();
    expect(component.recurringExpenseForm.get('amount')?.value).toBe('');
    expect(component.recurringExpenseForm.get('frequency')?.value).toBe('');
    expect(component.recurringExpenseForm.get('fromDate')?.value).toBe('');
    expect(component.recurringExpenseForm.get('toDate')?.value).toBe('');
    expect(component.recurringExpenseForm.get('fromYear')?.value).toBe('');
    expect(component.recurringExpenseForm.get('toYear')?.value).toBe('');
    expect(component.recurringExpenseForm.get('categoryID')?.value).toBe('');
    expect(component.recurringExpenseForm.get('receiptUrl')?.value).toBe('');
  });

  it('should load categories on init', () => {
    component.ngOnInit();

    expect(globalService.loadCategories).toHaveBeenCalled();
    expect(component.categories.length).toBeGreaterThan(0);
  });

  it('should handle error when loading categories', () => {
    globalService.loadCategories.and.returnValue(throwError('Error'));

    component.ngOnInit();

    expect(toastrService.error).toHaveBeenCalledWith('Error loading categories:', 'error');
  });

  it('should generate years correctly', () => {
    component.generateYears();
    expect(component.years.length).toBeGreaterThan(0);
    expect(component.years[0]).toBe(2020);
    expect(component.years[component.years.length - 1]).toBe(component.currentYear + 6);
  });

  it('should change currency and convert amount', () => {
    component.recurringExpenseForm.patchValue({ amount: 100 });
    component.changeCurrency('USD');

    expect(component.selectedCurrency).toBe('USD');
    expect(globalService.convertCurrency).toHaveBeenCalledWith(100, 'INR', 'USD');
  });

  it('should convert amount to INR', () => {
    component.selectedCurrency = 'USD';
    component.originalAmount = 100;

    globalService.convertCurrency.and.returnValue(of(75));

    component.convertToINR(100);

    expect(component.originalAmount).toBe(75);
    expect(toastrService.error).not.toHaveBeenCalled();
  });

  it('should handle error during currency conversion', () => {
    globalService.convertCurrency.and.returnValue(throwError('Error'));

    component.convertToINR(100);

    expect(toastrService.error).toHaveBeenCalledWith('Error converting currency:', 'error');
  });

  it('should submit form when valid', () => {
    component.recurringExpenseForm.patchValue({
      amount: 100,
      frequency: 'Monthly Recurring',
      fromDate: '2023-01-01',
      toDate: '2023-12-31',
      fromYear: '',
      toYear: '',
      categoryID: 1,
      receiptUrl: ''
    });
    component.originalAmount = 100;

    globalService.addRecurringTransaction.and.returnValue(of({ status: true, data: 'Transaction added successfully' }));

    component.onSubmit();

    expect(globalService.addRecurringTransaction).toHaveBeenCalled();
    expect(toastrService.success).toHaveBeenCalledWith('Transaction added successfully', 'Success');
    expect(updateService.notifyExpenseAdded).toHaveBeenCalled();
  });

  it('should handle error during form submission', () => {
    component.recurringExpenseForm.patchValue({
      amount: 100,
      frequency: 'Monthly Recurring',
      fromDate: '2023-01-01',
      toDate: '2023-12-31',
      fromYear: '',
      toYear: '',
      categoryID: 1,
      receiptUrl: ''
    });
    component.originalAmount = 100;

    globalService.addRecurringTransaction.and.returnValue(throwError('Error'));

    component.onSubmit();

    expect(toastrService.error).toHaveBeenCalledWith('AddTransactions not added successfully!', 'Error');
  });

  it('should validate dates correctly for monthly frequency', () => {
    component.recurringExpenseForm.patchValue({
      frequency: 'Monthly Recurring',
      fromDate: '2023-01-01',
      toDate: '2023-12-31'
    });

    const validationResult = component.dateValidator(component.recurringExpenseForm);
    expect(validationResult).toBeNull(); 
  });

  it('should invalidate dates for monthly frequency when toDate is before fromDate', () => {
    component.recurringExpenseForm.patchValue({
      frequency: 'Monthly Recurring',
      fromDate: '2023-12-31',
      toDate: '2023-01-01'
    });

    const validationResult = component.dateValidator(component.recurringExpenseForm);
    expect(validationResult).toEqual({ dateInvalid: true });
  });

  it('should validate dates correctly for yearly frequency', () => {
    component.recurringExpenseForm.patchValue({
      frequency: 'Yearly Recurring',
      fromYear: 2023,
      toYear: 2025
    });

    const validationResult = component.dateValidator(component.recurringExpenseForm);
    expect(validationResult).toBeNull();
  });

  it('should invalidate dates for yearly frequency when toYear is before fromYear', () => {
    component.recurringExpenseForm.patchValue({
      frequency: 'Yearly Recurring',
      fromYear: 2025,
      toYear: 2023
    });

    const validationResult = component.dateValidator(component.recurringExpenseForm);
    expect(validationResult).toEqual({ dateInvalid: true });
  });

  it('should handle file selection and upload', () => {
    const file = new File([''], 'receipt.png');
    const event = { target: { files: [file] } } as unknown as Event;

    globalService.uploadFile.and.returnValue(of({ filePath: 'path/to/receipt.png' }));

    component.onFileSelected(event);

    expect(globalService.uploadFile).toHaveBeenCalled();
    expect(component.recurringExpenseForm.get('receiptUrl')?.value).toBe('path/to/receipt.png');
  });

  it('should handle error during file upload', () => {
    const file = new File([''], 'receipt.png');
    const event = { target: { files: [file] } } as unknown as Event;

    globalService.uploadFile.and.returnValue(throwError('Error'));

    component.onFileSelected(event);

    expect(toastrService.error).toHaveBeenCalledWith('Error uploading file', 'error');
  });
});