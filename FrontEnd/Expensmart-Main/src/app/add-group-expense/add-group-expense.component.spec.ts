import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AddGroupExpenseComponent } from './add-group-expense.component';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ActivatedRoute } from '@angular/router';
import { GlobalService } from '../services/global.service';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';
import { DatePipe } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { provideNativeDateAdapter } from '@angular/material/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('AddGroupExpenseComponent', () => {
  let component: AddGroupExpenseComponent;
  let fixture: ComponentFixture<AddGroupExpenseComponent>;
  let globalService: jasmine.SpyObj<GlobalService>;
  let toastrService: jasmine.SpyObj<ToastrService>;
  let activatedRoute: jasmine.SpyObj<ActivatedRoute>;

  const mockCategories = [
    { categoryID: 1, name: 'Food', isActive: true, createdOn: new Date(), modifiedOn: new Date() },
    { categoryID: 2, name: 'Travel', isActive: true, createdOn: new Date(), modifiedOn: new Date() }
  ];

  const mockCategoryKeywords = [
    { categoryKeywordID: 1, categoryID: 1, keyword: 'food', categoriesEntity: mockCategories[0] },
    { categoryKeywordID: 2, categoryID: 2, keyword: 'travel', categoriesEntity: mockCategories[1] }
  ];

  beforeEach(async () => {
    globalService = jasmine.createSpyObj('GlobalService', [
      'loadCategories',
      'getCategoryKeywords',
      'convertCurrency',
      'addGroupExpense',
      'uploadFile'
    ]);
    toastrService = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    activatedRoute = jasmine.createSpyObj('ActivatedRoute', [], { params: of({ user1Id: '1', user2Id: '2' }) });

    await TestBed.configureTestingModule({
      imports: [
        ReactiveFormsModule,
        HttpClientTestingModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatDatepickerModule,
        MatIconModule,
        MatCheckboxModule,
        MatRadioModule,
        BrowserAnimationsModule
      ],
      declarations: [AddGroupExpenseComponent],
      providers: [
        FormBuilder,
        DatePipe,
        provideNativeDateAdapter(),
        { provide: GlobalService, useValue: globalService },
        { provide: ToastrService, useValue: toastrService },
        { provide: ActivatedRoute, useValue: activatedRoute }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AddGroupExpenseComponent);
    component = fixture.componentInstance;

    // Mock localStorage
    spyOn(localStorage, 'getItem').and.returnValue('123');

    // Mock ViewChild
    component.myInputVariable = { nativeElement: { value: '' } } as any;

    // Set default mock responses
    globalService.loadCategories.and.returnValue(of(mockCategories));
    globalService.getCategoryKeywords.and.returnValue(of(mockCategoryKeywords));
  });

  afterEach(() => {
    fixture.destroy();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form with default values', () => {
    fixture.detectChanges();
    expect(component.expenseForm).toBeDefined();
    expect(component.expenseForm.get('amount')?.value).toBe('');
    expect(component.expenseForm.get('date')?.value).toEqual(jasmine.any(Date));
    expect(component.expenseForm.get('expenseType')?.value).toBe('One-time');
  });

  it('should load categories and keywords on ngOnInit', fakeAsync(() => {
    component.ngOnInit();
    tick(500); // For debounceTime
    expect(component.categories).toEqual(mockCategories);
    expect(component.categoryKeywords['Food']).toEqual(['food']);
    expect(component.categoryKeywords['Travel']).toEqual(['travel']);
  }));

  it('should update user1Id and user2Id from route params', () => {
    component.ngOnInit();
    expect(component.user1Id).toBe(1);
    expect(component.user2Id).toBe(2);
  });

  it('should handle error loading categories', fakeAsync(() => {
    globalService.loadCategories.and.returnValue(throwError(() => new Error('Error')));
    component.ngOnInit();
    tick();
    expect(toastrService.error).toHaveBeenCalledWith('Error loading categories:', 'error');
  }));

  it('should handle error loading category keywords', fakeAsync(() => {
    globalService.getCategoryKeywords.and.returnValue(throwError(() => new Error('Error')));
    component.ngOnInit();
    tick();
    expect(toastrService.error).toHaveBeenCalledWith('Error loading category keywords:', 'error');
  }));

  it('should auto-select category based on description', fakeAsync(() => {
    component.categories = mockCategories;
    component.categoryKeywords = { Food: ['food'], Travel: ['travel'] };
    component.expenseForm.get('description')?.setValue('buying food');
    tick(500); // For debounceTime
    expect(component.expenseForm.get('categoryID')?.value).toBe(1);
  }));

  it('should not change category if no matching keyword', fakeAsync(() => {
    component.categories = mockCategories;
    component.categoryKeywords = { Food: ['food'], Travel: ['travel'] };
    component.expenseForm.get('description')?.setValue('random text');
    tick(500);
    expect(component.expenseForm.get('categoryID')?.value).toBe('');
  }));

  it('should convert amount to INR when currency is not INR', fakeAsync(() => {
    globalService.convertCurrency.and.returnValue(of(7500));
    component.selectedCurrency = 'USD';
    component.expenseForm.get('amount')?.setValue(100);
    tick(500);
  }));

  it('should not convert amount if currency is INR', fakeAsync(() => {
    component.selectedCurrency = 'INR';
    component.expenseForm.get('amount')?.setValue(100);
    tick(500);
    expect(globalService.convertCurrency).not.toHaveBeenCalled();
  }));

  it('should change currency and convert amount', fakeAsync(() => {
    globalService.convertCurrency.and.returnValue(of(75));
    component.expenseForm.get('amount')?.setValue(7500);
    component.changeCurrency('USD');
    tick();
    expect(globalService.convertCurrency).toHaveBeenCalledWith(7500, 'INR', 'USD');
    expect(component.expenseForm.get('amount')?.value).toBe(75);
    expect(component.selectedCurrency).toBe('USD');
  }));

  it('should reset to original amount when switching to INR', () => {
    component.originalAmount = 7500;
    component.selectedCurrency = 'USD';
    component.expenseForm.get('amount')?.setValue(100);
    component.changeCurrency('INR');
    expect(component.expenseForm.get('amount')?.value).toBe(7500);
    expect(component.selectedCurrency).toBe('INR');
  });

  it('should not convert currency if no amount', () => {
    component.changeCurrency('USD');
    expect(globalService.convertCurrency).not.toHaveBeenCalled();
  });

  it('should handle currency conversion error', fakeAsync(() => {
    globalService.convertCurrency.and.returnValue(throwError(() => new Error('Error')));
    component.expenseForm.get('amount')?.setValue(100);
    component.changeCurrency('USD');
    tick();
    expect(toastrService.error).toHaveBeenCalledWith('Error converting currency:', 'error');
  }));

  it('should submit form successfully', fakeAsync(() => {
    globalService.addGroupExpense.and.returnValue(of({ status: true, data: 'Expense added' }));
    component.groupID = 1;
    component.expenseForm.patchValue({
      amount: 100,
      date: new Date(),
      categoryID: 1,
      description: 'Test expense',
      expenseType: 'One-time'
    });
    component.onSubmit();
    tick();
    expect(component.isLoading).toBeFalse();
    expect(component.expenseForm.pristine).toBeTrue();
    expect(component.myInputVariable.nativeElement.value).toBe('');
  }));

  it('should handle submit error', fakeAsync(() => {
    globalService.addGroupExpense.and.returnValue(of({ status: false, data: 'Failed' }));
    component.groupID = 1;
    component.expenseForm.patchValue({
      amount: 100,
      date: new Date(),
      categoryID: 1,
      description: 'Test expense',
      expenseType: 'One-time'
    });
    component.onSubmit();
    tick();
    expect(component.isLoading).toBeFalse();
    expect(component.expenseForm.pristine).toBeTrue();
  }));

  it('should handle submit server error', fakeAsync(() => {
    globalService.addGroupExpense.and.returnValue(throwError(() => ({ message: 'Server error' })));
    component.groupID = 1;
    component.expenseForm.patchValue({
      amount: 100,
      date: new Date(),
      categoryID: 1,
      description: 'Test expense',
      expenseType: 'One-time'
    });
    component.onSubmit();
    tick();
    expect(component.isLoading).toBeFalse();
  }));

  it('should not submit if form is invalid', () => {
    component.onSubmit();
    expect(globalService.addGroupExpense).not.toHaveBeenCalled();
    expect(component.isLoading).toBeFalse();
  });

  it('should upload file and patch receiptUrl', fakeAsync(() => {
    globalService.uploadFile.and.returnValue(of({ filePath: 'path/to/receipt' }));
    const file = new File([''], 'receipt.jpg');
    const event = { target: { files: [file] } } as any;
    component.onFileSelected(event);
    tick();
    expect(globalService.uploadFile).toHaveBeenCalledWith(jasmine.any(FormData));
    expect(component.expenseForm.get('receiptUrl')?.value).toBe('path/to/receipt');
  }));

  it('should handle file upload error', fakeAsync(() => {
    globalService.uploadFile.and.returnValue(throwError(() => new Error('Upload error')));
    spyOn(console, 'error');
    const file = new File([''], 'receipt.jpg');
    const event = { target: { files: [file] } } as any;
    component.onFileSelected(event);
    tick();
    expect(console.error).toHaveBeenCalledWith('Error uploading file', jasmine.any(Error));
  }));

  it('should handle no file selected', () => {
    spyOn(console, 'error');
    const event = { target: { files: null } } as any;
    component.onFileSelected(event);
    expect(console.error).toHaveBeenCalledWith('No file selected');
  });

  it('should return currency symbol', () => {
    component.selectedCurrency = 'USD';
    expect(component.selectCurrency()).toBe('$');
    component.selectedCurrency = 'INR';
    expect(component.selectCurrency()).toBe('₹');
  });
});