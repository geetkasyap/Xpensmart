import { ComponentFixture, TestBed } from '@angular/core/testing';
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
import { ExpenseFormComponent } from './expense-form.component';
import { DatePipe } from '@angular/common';
import { of, throwError } from 'rxjs';
import { BulkExpensesComponent } from '../bulk-expenses/bulk-expenses.component';

describe('ExpenseFormComponent', () => {
  let component: ExpenseFormComponent;
  let fixture: ComponentFixture<ExpenseFormComponent>;
  let toastrServiceSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {
    const toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);

    await TestBed.configureTestingModule({
      declarations: [ExpenseFormComponent,BulkExpensesComponent],
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

    fixture = TestBed.createComponent(ExpenseFormComponent);
    component = fixture.componentInstance;
    toastrServiceSpy = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with default values', () => {
    expect(component.expenseForm).toBeDefined();
    expect(component.expenseForm.get('amount')?.value).toBe('');
    expect(component.expenseForm.get('date')?.value).toBeInstanceOf(Date);
    expect(component.expenseForm.get('categoryID')?.value).toBe('');
    expect(component.expenseForm.get('description')?.value).toBe('');
    expect(component.expenseForm.get('expenseType')?.value).toBe('One-time');
    expect(component.expenseForm.get('isRecurring')?.value).toBe(false);
    expect(component.expenseForm.get('userID')?.value).toBe(component.locUserID);
  });

  it('should load categories on init', () => {
    const mockCategories = [
      { categoryID: 1, name: 'Food', isActive: true, createdOn: new Date(), modifiedOn: new Date() },
      { categoryID: 2, name: 'Housing', isActive: true, createdOn: new Date(), modifiedOn: new Date() },
    ];
  
    spyOn(component['http'], 'get').and.returnValue(of(mockCategories));

    component.loadCategories();
  
    expect(component.categories).toEqual(mockCategories);
  });

  it('should handle error while loading categories', () => {
    spyOn(component['http'], 'get').and.returnValue(throwError({ message: 'Error' }));
    component.loadCategories();
    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Error loading categories:', 'error');
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
    spyOn(component['http'], 'get').and.returnValue(of(mockKeywords));

    component.loadCategoryKeywords();
    expect(component.categoryKeywords['Food']).toContain('grocery');
  });

  it('should handle error while loading category keywords', () => {
    spyOn(component['http'], 'get').and.returnValue(throwError({ message: 'Error' }));

    component.loadCategoryKeywords();
    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Error loading category keywords:', 'error');
  });

  it('should auto-select category based on description', () => {
    component.categoryKeywords = { Food: ['grocery'] };
    component.categories = [{ categoryID: 1, name: 'Food', isActive: true, createdOn: new Date(), modifiedOn: new Date() }];

    component.autoSelectCategory('bought grocery');
    expect(component.expenseForm.get('categoryID')?.value).toBe(1);
  });

  it('should convert currency successfully', () => {
    spyOn(component['http'], 'get').and.returnValue(of(75));

    component.convertCurrency(1, 'USD', 'INR');
    expect(component.expenseForm.get('amount')?.value).toBe(75);
  });

  it('should handle error while converting currency', () => {
    spyOn(component['http'], 'get').and.returnValue(throwError({ message: 'Error' }));

    component.convertCurrency(1, 'USD', 'INR');
    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Error converting currency:', 'error');
  });

  it('should get category keywords', () => {
    const mockKeywords = [
      {
        categoryKeywordID: 1,
        categoryID: 1,
        keyword: "rent",
        categoriesEntity: {
        categoryID: 1,
        name: "Housing, Groceries & Utilities",
        isActive: true,
        createdOn: "2025-02-13T22:35:46.493",
        modifiedOn: "2025-02-13T22:35:46.493",
        expensesEntities: null,
        categoryKeywordsEntities: null
        },
      },
    ];

    spyOn(component['http'], 'get').and.returnValue(of({mockKeywords}));

    component.getCategoryKeywords();
  });



  it('should reset the form', () => {
    component.expenseForm.patchValue({
      amount: 100,
      date: new Date(),
      categoryID: 1,
      receiptUrl: 'test.jpg',
      description: 'Test',
      expenseType: 'Business',
      isRecurring: true,
    });

    component.resetForm();
    expect(component.expenseForm.get('amount')?.value).toBe('');
    expect(component.expenseForm.get('receiptUrl')?.value).toBe('');
    expect(component.selectedCurrency).toBe('INR');
    expect(component.originalAmount).toBe(0);
  });

  it('should validate file upload with allowed types', () => {
    const mockFile = new File([''], 'test.jpg', { type: 'image/jpeg' });
    const event = { target: { files: [mockFile] } };

    spyOn(component['http'], 'post').and.returnValue(of({ filePath: 'uploads/test.jpg' }));

    component.onFileSelected(event as any);
    expect(component.expenseForm.get('receiptUrl')?.value).toBe('uploads/test.jpg');
  });

  it('should reject invalid file type', () => {
    const mockFile = new File([''], 'test.txt', { type: 'text/plain' });
    const event = { target: { files: [mockFile] } };

    component.onFileSelected(event as any);
    expect(toastrServiceSpy.error).toHaveBeenCalledWith(
      'Invalid file type. Only images, PDFs, and ZIP files are allowed.',
      'Error'
    );
  });

  it('should reject file exceeding size limit', () => {
    const mockFile = new File([''], 'largefile.zip', { type: 'application/zip' });
    Object.defineProperty(mockFile, 'size', { value: 101 * 1024 * 1024 });
    const event = { target: { files: [mockFile] } };

    component.onFileSelected(event as any);
    expect(toastrServiceSpy.error).toHaveBeenCalledWith(
      'File size exceeds the maximum limit of 100 MB.',
      'Error'
    );
  });

  it('should submit form successfully', () => {
    spyOn(component['http'], 'post').and.returnValue(of({ status: true, data: 'Expense added successfully' }));
    spyOn(component['http'], 'get').and.returnValue(of(100));

    component.expenseForm.patchValue({
      amount: 100,
      date: new Date(),
      categoryID: 1,
      description: 'Test',
      expenseType: 'One-time',
      isRecurring: false,
    });

    component.onSubmit();
    expect(toastrServiceSpy.success).toHaveBeenCalledWith('Expense added successfully', 'Success');
  });

  it('should handle error while submitting form', () => {
    spyOn(component['http'], 'post').and.returnValue(throwError({ message: 'Error' }));
    spyOn(component['http'], 'get').and.returnValue(of(100)); 

    component.expenseForm.patchValue({
      amount: 100,
      date: new Date(),
      categoryID: 1,
      description: 'Test',
      expenseType: 'One-time',
      isRecurring: false,
    });

    component.onSubmit();
    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Error adding expense:', 'error');
  });

  it('should change currency', () => {
    component.changeCurrency("INR");
  });
  it('should change another currency', () => {
    component.changeCurrency("USD");
  });
  it('should call convert to INR', () => {

    spyOn(component['http'], 'get').and.returnValue(of(3000));
    component.convertToINR(100);
  });
  it('should call convert to INR throws expection', () => {

    spyOn(component['http'], 'get').and.returnValue(throwError({ message: 'Error' }));
    component.convertToINR(100);
  });
  it('should get data error message', () => {
    spyOn(component['http'], 'get').and.returnValue(of('null'));
    component.getDateErrorMessage();
  });
  it('should return "Date is required" when date field is touched and invalid', () => {
    const dateControl = component.expenseForm.get('date');
    dateControl?.setValue(null);
    dateControl?.markAsTouched();
    fixture.detectChanges();
    const errorMessage = component.getDateErrorMessage();
    expect(errorMessage).toBe('Date is required');
  });
  it('should convert amount to selected currency', () => {
    component.selectedCurrency='USD';
    spyOn(component['http'], 'get').and.returnValue(of(200));
    component.convertAmountToSelectedCurrency(10);
  });
  it('should submit form toaster error', () => {
    spyOn(component['http'], 'post').and.returnValue(of({ status: false, data: 'Expense adding failed' }));
    spyOn(component['http'], 'get').and.returnValue(of(100));

    component.expenseForm.patchValue({
      amount: 100,
      date: new Date(),
      categoryID: 1,
      description: 'Test',
      expenseType: 'One-time',
      isRecurring: false,
    });

    component.onSubmit();
    expect(toastrServiceSpy.error).toHaveBeenCalledWith( 'Expense adding failed', 'Error' );
  });
  it('should handle error while uploading file', () => {
    const mockFile = new File([''], 'test.jpg', { type: 'image/jpeg' });
    const event = { target: { files: [mockFile] } };

    spyOn(component['GlobalService'], 'uploadFile').and.returnValue(throwError({ message: 'Error uploading file' }));

    component.onFileSelected(event as any);
    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Error uploading file', 'error');
  });

  it('should handle error while converting currency', () => {
    spyOn(component['GlobalService'], 'convertCurrency').and.returnValue(throwError({ message: 'Error converting currency' }));

    component.convertCurrency(100, 'USD', 'INR');
    expect(toastrServiceSpy.error).toHaveBeenCalledWith('Error converting currency:', 'error');
  });
});