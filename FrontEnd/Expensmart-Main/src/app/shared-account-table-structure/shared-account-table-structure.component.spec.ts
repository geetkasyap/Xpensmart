import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SharedAccountTableStructureComponent } from './shared-account-table-structure.component';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { FormBuilder, FormsModule } from '@angular/forms';
import { ExpenseService } from '../services/expense.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ToastrService } from 'ngx-toastr';
import { MatDialog } from '@angular/material/dialog';
import { of } from 'rxjs';
import { ConfirmationDialogComponent } from './../confirmation-dialog/confirmation-dialog.component';
import { CategoryEntity } from '../models/category-entity-model';
import { ExpensesEntity } from '../models/expenses-entity-model';
import { MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSortModule } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { GlobalService } from '../services/global.service';

describe('SharedAccountTableStructureComponent', () => {
  let component: SharedAccountTableStructureComponent;
  let fixture: ComponentFixture<SharedAccountTableStructureComponent>;
  let expenseService: jasmine.SpyObj<ExpenseService>;
  let toastrService: jasmine.SpyObj<ToastrService>;
  let globalService: jasmine.SpyObj<GlobalService>;
  let dialog: jasmine.SpyObj<MatDialog>;
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<ConfirmationDialogComponent>>;

  beforeEach(async () => {
    const expenseServiceSpy = jasmine.createSpyObj('ExpenseService', ['getExpenses', 'updateExpense', 'deleteExpense']);
    const toastrServiceSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    const dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    const globalServiceSpy = jasmine.createSpyObj('GlobalService', [ 'uploadFile']);

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule,MatFormFieldModule,MatPaginatorModule,
              MatInputModule,
              MatSelectModule,MatChipsModule,FormsModule,MatTableModule,BrowserAnimationsModule,MatSortModule,
              MatIconModule,
              MatDatepickerModule
      ],
      declarations: [SharedAccountTableStructureComponent],
      providers: [
        FormBuilder,
        { provide: ExpenseService, useValue: expenseServiceSpy },
        { provide: ToastrService, useValue: toastrServiceSpy },
        { provide: MatDialog, useValue: dialogSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SharedAccountTableStructureComponent);
    component = fixture.componentInstance;
    expenseService = TestBed.inject(ExpenseService) as jasmine.SpyObj<ExpenseService>;
    globalService = TestBed.inject(GlobalService) as jasmine.SpyObj<GlobalService>;
    toastrService = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;
    dialog = TestBed.inject(MatDialog) as jasmine.SpyObj<MatDialog>;
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['afterClosed']);
    dialog.open.and.returnValue(dialogRefSpy)
    component.dataSource = new MatTableDataSource<ExpensesEntity>([]);
    component.userId = 1;
    const mockExpenses: ExpensesEntity[] = [{
      expenseID: 1, amount: 100, date: new Date(), categoryID: 1, description: 'Test', expenseType: 'Personal',
      userID: 0,
      receiptUrl: '',
      isActive: false,
      isRecurring: false,
      createdOn: new Date(),
      modifiedOn: new Date(),
      createdBy: '',
      modifiedBy: '',
      usersEntity: undefined,
      categoriesEntity: undefined
    }];
    expenseService.getExpenses.and.returnValue(of(mockExpenses));
    component.dataSource = new MatTableDataSource<ExpensesEntity>([]);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load expenses on init', async() => {
    const mockExpenses: ExpensesEntity[] = [{
      expenseID: 1, amount: 100, date: new Date(), categoryID: 1, description: 'Test', expenseType: 'Personal',
      userID: 0,
      receiptUrl: '',
      isActive: false,
      isRecurring: false,
      createdOn: new Date(),
      modifiedOn: new Date(),
      createdBy: '',
      modifiedBy: '',
      usersEntity: undefined,
      categoriesEntity: undefined
    }];
    expenseService.getExpenses.and.returnValue(of(mockExpenses));

    await component.ngOnInit();

    expect(expenseService.getExpenses).toHaveBeenCalled();
    expect(component.dataSource.data).toEqual(mockExpenses);
  });

  it('should load categories', () => {
    const mockCategories: CategoryEntity[] = [{
      categoryID: 1, name: 'Food',
      isActive: false,
      createdOn: new Date(),
      modifiedOn: new Date()
    }];
    component.loadCategories();

    component.http.get<CategoryEntity[]>('https://localhost:7154/GetCategories').subscribe((categories) => {
      expect(categories).toEqual(mockCategories);
      expect(component.categories).toEqual(mockCategories);
    });
  });

  it('should setup search filter', () => {
    component.setupSearchFilter();
    const mockExpense: ExpensesEntity = {
      expenseID: 1, amount: 100, date: new Date(), categoryID: 1, description: 'Test', expenseType: 'Personal',
      userID: 0,
      receiptUrl: '',
      isActive: false,
      isRecurring: false,
      createdOn: new Date(),
      modifiedOn: new Date(),
      createdBy: '',
      modifiedBy: '',
      usersEntity: undefined,
      categoriesEntity: undefined
    };
    component.dataSource.data = [mockExpense];

    const result = component.dataSource.filterPredicate(mockExpense, 'Test');
    expect(result).toBeTrue();
  });

  it('should apply filter', () => {
    const event = new Event('input');
    Object.defineProperty(event, 'target', {
      value: { value: 'Test' },
      writable: false
    });
    component.applyFilter(event);
    expect(component.dataSource.filter).toBe('test');
  });

  it('should toggle filter', () => {
    component.toggleFilter('Personal');
    expect(component.selectedFilters.has('Personal')).toBeTrue();

    component.toggleFilter('Personal');
    expect(component.selectedFilters.has('Personal')).toBeFalse();
  });

  it('should get category name', () => {
    component.categories = [{
      categoryID: 1, name: 'Food',
      isActive: false,
      createdOn: new Date(),
      modifiedOn: new Date()
    }];
    const categoryName = component.getCategoryName(1);
    expect(categoryName).toBe('Food');
  });

  it('should return default category name if not found', () => {
    const categoryName = component.getCategoryName(999);
    expect(categoryName).toBe('');
  });

  it('should get category icon', () => {
    const icon = component.getCategoryIcon(1);
    expect(icon).toBe('home');
  });

  it('should start editing an expense', () => {
    const mockExpense: ExpensesEntity = {
      expenseID: 1, amount: 100, date: new Date(), categoryID: 1, description: 'Test', expenseType: 'Personal',
      userID: 1,
      receiptUrl: '',
      isActive: false,
      isRecurring: false,
      createdOn: new Date(),
      modifiedOn: new Date(),
      createdBy: '',
      modifiedBy: '',
      usersEntity: undefined,
      categoriesEntity: undefined
    };
    component.startEdit(mockExpense);
    expect(component.editingExpense).toEqual(mockExpense);
  });

  it('should cancel edit', () => {
    component.cancelEdit();
    expect(component.editingExpense).toBeNull();
  });

  it('should save edited expense', () => {
    const mockExpense: ExpensesEntity = {
      expenseID: 1, amount: 100, date: new Date(), categoryID: 1, description: 'Test', expenseType: 'Personal',
      userID: 1,
      receiptUrl: '',
      isActive: false,
      isRecurring: false,
      createdOn: new Date(),
      modifiedOn: new Date(),
      createdBy: '',
      modifiedBy: '',
      usersEntity: undefined,
      categoriesEntity: undefined
    };
    component.editingExpense = mockExpense;
    component.editForm.patchValue({
      amount: 150,
      date: new Date(),
      categoryID: 1,
      receiptUrl: '',
      description: 'Updated Test',
      expenseType: 'Business',
      userID: 1
    });

expenseService.updateExpense.and.returnValue(of({ message: 'Expense updated successfully', status: true }));
    component.saveEdit();
    expect(component.editingExpense).toBeNull();
  });

  it('should handle error on save edit', () => {
    const mockExpense: ExpensesEntity = {
      expenseID: 1, amount: 100, date: new Date(), categoryID: 1, description: 'Test', expenseType: 'Personal',
      userID: 1,
      receiptUrl: '',
      isActive: false,
      isRecurring: false,
      createdOn: new Date(),
      modifiedOn: new Date(),
      createdBy: '',
      modifiedBy: '',
      usersEntity: undefined,
      categoriesEntity: undefined
    };
    component.editingExpense = mockExpense;
    component.editForm.patchValue({
      expenseID: 1, amount: 100, date: new Date(), categoryID: 1, description: 'Test', expenseType: 'Personal',
      userID: 1,
      receiptUrl: '',
      isActive: false,
      isRecurring: false,
      createdOn: new Date(),
      modifiedOn: new Date(),
      createdBy: '',
      modifiedBy: '',
      usersEntity: undefined,
      categoriesEntity: undefined
    });

    expenseService.updateExpense.and.returnValue(of({ status: false, message: 'Update failed' }));
    component.saveEdit();

    expect(toastrService.error).toHaveBeenCalledWith(undefined, 'Error');
  });

  it('should delete an expense', () => {
    const mockExpenseId = 1;
    dialogRefSpy.afterClosed.and.returnValue(of(true));
  expenseService.deleteExpense.and.returnValue(of({ status: true, data: 'Expense deleted successfully' }));
  
    component.deleteExpense(mockExpenseId);
  
    expect(dialog.open).toHaveBeenCalled();
    expect(expenseService.deleteExpense).toHaveBeenCalledWith(mockExpenseId);
    expect(toastrService.success).toHaveBeenCalledWith('Expense deleted successfully', 'Success');
  });
  
  it('should handle error on delete expense', () => {
    const mockExpenseId = 1;
    dialogRefSpy.afterClosed.and.returnValue(of(true));
    expenseService.deleteExpense.and.returnValue(of({ status: true, data: 'Expense deleted successfully' }));
  
    component.deleteExpense(mockExpenseId);

  });

  it('should open receipt URL', () => {
    const mockUrl = 'http://example.com/receipt.png';
    spyOn(window, 'open');
    component.openReceipt(mockUrl);
    expect(window.open).toHaveBeenCalledWith(mockUrl, '_blank');
  });
  it('should upload a file and update the receiptUrl', () => {
    const mockFile = new File([''], 'test.png', { type: 'image/png' });
    const event = { target: { files: [mockFile] } } as unknown as Event; 

    const mockResponse = { filePath: 'http://localhost:8080/uploads/test.png' };
    globalService.uploadFile.and.returnValue(of(mockResponse)); 

    component.onFileSelected(event);

    expect(component.editForm.value.receiptUrl).toBe(mockResponse.filePath);
});

});