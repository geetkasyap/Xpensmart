import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SharedAccountExpensesComponent } from './shared-account-expenses.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { RouterTestingModule } from '@angular/router/testing';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatIconModule } from '@angular/material/icon';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DatePipe } from '@angular/common';
import { of, throwError } from 'rxjs';

describe('SharedAccountExpensesComponent', () => {
  let component: SharedAccountExpensesComponent;
  let fixture: ComponentFixture<SharedAccountExpensesComponent>;
  let toastrServiceSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {
    const toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    await TestBed.configureTestingModule({
      declarations: [SharedAccountExpensesComponent],
      imports: [
        ReactiveFormsModule,
        FormsModule,
        HttpClientTestingModule,
        RouterTestingModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatDatepickerModule,
        MatCheckboxModule,
        MatRadioModule,
        MatIconModule,
        BrowserAnimationsModule,
        ToastrModule.forRoot(),
      ],
      providers: [
        { provide: ToastrService, useValue: toastrSpy },
        DatePipe,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SharedAccountExpensesComponent);
    component = fixture.componentInstance;
    toastrServiceSpy = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize forms correctly', () => {
    expect(component.expenseForm1).toBeDefined();
    expect(component.expenseForm2).toBeDefined();
    expect(component.expenseForm1.get('amount')?.value).toBe('');
    expect(component.expenseForm2.get('amount')?.value).toBe('');
  });

  it('should load users on init', () => {
    const mockUsers = [{ userID: 1, userName: 'User  1' }, { userID: 2, userName: 'User  2' }];
    spyOn(component.GlobalService, 'getSharedUserDetails').and.returnValue(of(mockUsers));

    component.loadUsers();

    expect(component.users).toEqual(mockUsers);
  });


  it('should change user and reset form', () => {
    component.selectedUser  = '2';
    component.onUserChange();

    expect(component.userId2).toBe(2);
    expect(component.expenseForm2).toBeDefined();
  });

  it('should submit form for user 1 successfully', () => {
    spyOn(component.GlobalService, 'addExpense').and.returnValue(of({ status: true, data: 'Expense added successfully' }));
    component.expenseForm1.patchValue({
      amount: 100,
      date: new Date(),
      categoryID: 1,
      description: 'Test',
      expenseType: 'One-time',
      isRecurring: false,
    });

    component.onSubmit(1);

    expect(toastrServiceSpy.success).toHaveBeenCalledWith('Expense added successfully', 'Success');
  });

  it('should handle error while submitting form for user 1', () => {
    spyOn(component.GlobalService, 'addExpense').and.returnValue(throwError('Error'));

    component.expenseForm1.patchValue({
      amount: 100,
      date: new Date(),
      categoryID: 1,
      description: 'Test',
      expenseType: 'One-time',
      isRecurring: false,
    });

    component.onSubmit(1);

    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Expense not added successfully for User 1!', 'Error');
  });

  it('should submit form for user 2 successfully', () => {
    component.selectedUser  = '2';
    component.userId2 = 2;
    spyOn(component.GlobalService, 'addExpense').and.returnValue(of({ status: true, data: 'Expense added successfully' }));
    component.expenseForm2.patchValue({
      amount: 100,
      date: new Date(),
      categoryID: 1,
      description: 'Test',
      expenseType: 'One-time',
      isRecurring: false,
    });

    component.onSubmit(2);

    expect(toastrServiceSpy.success).toHaveBeenCalledWith('Expense added successfully', 'Success');
  });

  it('should handle error while submitting form for user 2', () => {
    component.selectedUser  = '2';
    component.userId2 = 2;
    spyOn(component.GlobalService, 'addExpense').and.returnValue(throwError('Error'));

    component.expenseForm2.patchValue({
      amount: 100,
      date: new Date(),
      categoryID: 1,
      description: 'Test',
      expenseType: 'One-time',
      isRecurring: false,
    });

    component.onSubmit(2);

    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Expense not added successfully for User 2!', 'Error');
  });

  it('should load categories on init', () => {
    const mockCategories = [
      { categoryID: 1, name: 'Food', isActive: true, createdOn: new Date(), modifiedOn: new Date() },
      { categoryID: 2, name: 'Housing', isActive: true, createdOn: new Date(), modifiedOn: new Date() },
    ];

    spyOn(component.GlobalService, 'loadCategories').and.returnValue(of(mockCategories));

    component.loadCategories();

    expect(component.categories).toEqual(mockCategories);
  });

  it('should handle error while loading categories', () => {
    spyOn(component.GlobalService, 'loadCategories').and.returnValue(throwError('Error'));

    component.loadCategories();

  });

  it('should convert currency for user 1', () => {
    spyOn(component.GlobalService, 'convertCurrency').and.returnValue(of(75));

    component.convertCurrency(100, 'USD', 'INR', 1);

    expect(component.expenseForm1.get('amount')?.value).toBe(75);
  });

  it('should convert currency for user 2', () => {
    spyOn(component.GlobalService, 'convertCurrency').and.returnValue(of(75));

    component.convertCurrency(100, 'USD', 'INR', 2);

    expect(component.expenseForm2.get('amount')?.value).toBe(75);
  });

  it('should handle error while converting currency for user 1', () => {
    spyOn(component.GlobalService, 'convertCurrency').and.returnValue(throwError('Error'));

    component.convertCurrency(100, 'USD', 'INR', 1);

  });

  it('should handle error while converting currency for user 2', () => {
    spyOn(component.GlobalService, 'convertCurrency').and.returnValue(throwError('Error'));

    component.convertCurrency(100, 'USD', 'INR', 2);
  });

  it('should load category keywords on init', () => {
    const mockKeywords = [
      {
        categoryKeywordID: 1,
        categoryID: 1,
        keyword: 'grocery',
        categoriesEntity: {
          categoryID: 1,
          name: 'Food',
          isActive: true,
          createdOn: new Date(),
          modifiedOn: new Date(),
          expensesEntities: [],
          categoryKeywordsEntities: [],
        },
      },
    ];
    spyOn(component.GlobalService, 'getCategoryKeywords').and.returnValue(of(mockKeywords));

    component.loadCategoryKeywords();

    expect(component.categoryKeywords['Food']).toContain('grocery');
  });

  it('should handle error while loading category keywords', () => {
    spyOn(component.GlobalService, 'getCategoryKeywords').and.returnValue(throwError('Error'));

    component.loadCategoryKeywords();
  });

  it('should handle file selection for user 1', () => {
    const mockFile = new File([''], 'test.jpg', { type: 'image/jpeg' });
    const event = { target: { files: [mockFile] } };

    spyOn(component.GlobalService, 'uploadFile').and.returnValue(of({ filePath: 'uploads/test.jpg' }));

    component.onFileSelected1(event as any);

    expect(component.expenseForm1.get('receiptUrl')?.value).toBe('uploads/test.jpg');
  });

  it('should handle file selection for user 2', () => {
    const mockFile = new File([''], 'test.jpg', { type: 'image/jpeg' });
    const event = { target: { files: [mockFile] } };

    spyOn(component.GlobalService, 'uploadFile').and.returnValue(of({ filePath: 'uploads/test.jpg' }));

    component.onFileSelected2(event as any);

    expect(component.expenseForm2.get('receiptUrl')?.value).toBe('uploads/test.jpg');
  });

  it('should handle error while uploading file for user 1', () => {
    const mockFile = new File([''], 'test.jpg', { type: 'image/jpeg' });
    const event = { target: { files: [mockFile] } };

    spyOn(component.GlobalService, 'uploadFile').and.returnValue(throwError('Error'));

    component.onFileSelected1(event as any);
  });

  it('should handle error while uploading file for user 2', () => {
    const mockFile = new File([''], 'test.jpg', { type: 'image/jpeg' });
    const event = { target: { files: [mockFile] } };

    spyOn(component.GlobalService, 'uploadFile').and.returnValue(throwError('Error'));

    component.onFileSelected2(event as any);

  });

  it('should change currency for user 1', () => {
    component.selectedCurrency1 = 'USD';
    component.changeCurrency('INR', 1);
    expect(component.expenseForm1.get('amount')?.value).toBe('');
  });

  it('should change currency for user 2', () => {
    component.selectedCurrency2 = 'USD';
    component.changeCurrency('INR', 2);
    expect(component.expenseForm2.get('amount')?.value).toBe('');
  });

  it('should convert to INR for user 1', () => {
    spyOn(component.GlobalService, 'convertCurrency').and.returnValue(of(75));
    component.convertToINR(100, 1);
    expect(component.originalAmount1).toBe(75);
  });

  it('should convert to INR for user 2', () => {
    spyOn(component.GlobalService, 'convertCurrency').and.returnValue(of(75));
    component.convertToINR(100, 2);
    expect(component.originalAmount2).toBe(75);
  });

  it('should handle error while converting to INR for user 1', () => {
    spyOn(component.GlobalService, 'convertCurrency').and.returnValue(throwError('Error'));
    component.convertToINR(100, 1);
  });

  it('should handle error while converting to INR for user 2', () => {
    spyOn(component.GlobalService, 'convertCurrency').and.returnValue(throwError('Error'));
    component.convertToINR(100, 2);
  });
});