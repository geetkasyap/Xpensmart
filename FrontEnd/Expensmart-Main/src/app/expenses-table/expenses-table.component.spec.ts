import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ExpensesTableComponent } from './expenses-table.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { of, throwError } from 'rxjs';
import { ExpenseService } from '../services/expense.service';
import { GlobalService } from './../services/global.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { ConfirmationDialogComponent } from './../confirmation-dialog/confirmation-dialog.component';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ExpensesEntity } from '../models/expenses-entity-model';
import { CategoryEntity } from '../models/category-entity-model';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';

describe('ExpensesTableComponent', () => {
  let component: ExpensesTableComponent;
  let fixture: ComponentFixture<ExpensesTableComponent>;
  let expenseService: jasmine.SpyObj<ExpenseService>;
  let globalService: jasmine.SpyObj<GlobalService>;
  let toastr: jasmine.SpyObj<ToastrService>;
  let router: jasmine.SpyObj<Router>;
  let dialog: jasmine.SpyObj<MatDialog>;
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<ConfirmationDialogComponent>>;
  beforeEach(async () => {
    const expenseServiceSpy = jasmine.createSpyObj('ExpenseService', ['getExpenses', 'updateExpense', 'deleteExpense']);
    const globalServiceSpy = jasmine.createSpyObj('GlobalService', ['loadCategories', 'uploadFile']);
    const toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['afterClosed']);
    const dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    dialogSpy.open.and.returnValue(dialogRefSpy);
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule,ReactiveFormsModule,MatFormFieldModule,MatPaginatorModule,
                    MatInputModule,
                    MatSelectModule,MatChipsModule,FormsModule,MatTableModule,BrowserAnimationsModule,MatSortModule,MatIconModule,
                    MatDatepickerModule],
      declarations: [ExpensesTableComponent],
      providers: [
        { provide: ExpenseService, useValue: expenseServiceSpy },
        { provide: GlobalService, useValue: globalServiceSpy },
        { provide: ToastrService, useValue: toastrSpy },
        { provide: Router, useValue: routerSpy },
        { provide: MatDialog, useValue: dialogSpy }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExpensesTableComponent);
    component = fixture.componentInstance;
    expenseService = TestBed.inject(ExpenseService) as jasmine.SpyObj<ExpenseService>;
    globalService = TestBed.inject(GlobalService) as jasmine.SpyObj<GlobalService>;
    toastr = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    dialog = TestBed.inject(MatDialog) as jasmine.SpyObj<MatDialog>;
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['afterClosed']);
    component.dataSource = new MatTableDataSource();
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
        
        const mockCategories: CategoryEntity[] = [{
          categoryID: 1, name: 'Food', isActive: true, createdOn: new Date(), modifiedOn: new Date()
        }];
        globalService.loadCategories.and.returnValue(of(mockCategories));
        dialogRefSpy.afterClosed.and.returnValue(of(true));
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load expenses on init', () => {
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

    component.ngOnInit();

    expect(expenseService.getExpenses).toHaveBeenCalled();
    expect(component.dataSource.data).toEqual(mockExpenses);
  });

  it('should handle error when loading expenses', () => {
    expenseService.getExpenses.and.returnValue(throwError('Error loading expenses'));

    component.ngOnInit();

    expect(toastr.error).toHaveBeenCalledWith('Error loading expenses:', 'error');
  });

  it('should load categories', () => {
    const mockCategories: CategoryEntity[] = [{
      categoryID: 1, name: 'Food',
      isActive: false,
      createdOn: new Date(),
      modifiedOn: new Date()
    }];
    globalService.loadCategories.and.returnValue(of(mockCategories));

    component.loadCategories();

    expect(globalService.loadCategories).toHaveBeenCalled();
    expect(component.categories).toEqual(mockCategories);
  });

  it('should handle error when loading categories', () => {
    globalService.loadCategories.and.returnValue(throwError('Error loading categories'));

    component.loadCategories();

    expect(toastr.error).toHaveBeenCalledWith('Error loading categories:', 'error');
  });

  it('should apply filter', () => {
    const event = { target: { value: 'test' } };
    component.applyFilter(event as any);
    expect(component.dataSource.filter).toBe('test');
  });

  it('should toggle filter', () => {
    component.toggleFilter('Personal');
    expect(component.selectedFilters.has('Personal')).toBeTrue();

    component.toggleFilter('Personal');
    expect(component.selectedFilters.has('Personal')).toBeFalse();
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

  it('should save edit', () => {
    const mockExpense = { expenseID: 1, amount: 100, date: new Date(), categoryID: 1, description: 'Test', expenseType: 'Personal',
      userID: 0,
      receiptUrl: '',
      isActive: false,
      isRecurring: false,
      createdOn: new Date(),
      modifiedOn: new Date(),
      createdBy: '',
      modifiedBy: '',
      usersEntity: undefined,
      categoriesEntity: undefined };
    component.editingExpense = mockExpense;
    component.editForm.setValue({
      amount: 150,
      date: '2023-01-02',
      categoryID: 1,
      receiptUrl: '',
      description: 'Updated Test',
      expenseType: 'Personal',
    });

    expenseService.updateExpense.and.returnValue(of({ status: true, message: 'Expense updated successfully' }));

    component.saveEdit();

    expect(expenseService.updateExpense).toHaveBeenCalled();
    expect(toastr.success).toHaveBeenCalledWith(undefined, 'Success');
    expect(component.editingExpense).toBeNull();
  });

  it('should handle error when saving edit', () => {
    const mockExpense = { expenseID: 1, amount: 100, date: new Date(), categoryID: 1, description: 'Test', expenseType: 'Personal',
      userID: 0,
      receiptUrl: '',
      isActive: false,
      isRecurring: false,
      createdOn: new Date(),
      modifiedOn: new Date(),
      createdBy: '',
      modifiedBy: '',
      usersEntity: undefined,
      categoriesEntity: undefined};
    component.editingExpense = mockExpense;
    component.editForm.setValue({
      amount: 150,
      date: '2023-01-02',
      categoryID: 1,
      receiptUrl: '',
      description: 'Updated Test',
      expenseType: 'Personal',
    });

    expenseService.updateExpense.and.returnValue(throwError('Error updating expense'));

    component.saveEdit();

    expect(toastr.error).toHaveBeenCalledWith('Expense not Updated successfully!', 'Error');
  });

  it('should delete an expense', () => {
    const mockExpenseId = 1;
    dialogRefSpy.afterClosed.and.returnValue(of(true));
    expenseService.deleteExpense.and.returnValue(of({ status: true, data: 'Expense deleted successfully' }));

    component.deleteExpense(mockExpenseId);

    expect(dialog.open).toHaveBeenCalled();
    expect(expenseService.deleteExpense).toHaveBeenCalledWith(mockExpenseId);
    expect(toastr.success).toHaveBeenCalledWith('Expense deleted successfully', 'Success');
  });

  it('should handle error when deleting an expense', () => {
    const mockExpenseId = 1;
    dialogRefSpy.afterClosed.and.returnValue(of(true)); 
    expenseService.deleteExpense.and.returnValue(throwError('Error deleting expense'));

    component.deleteExpense(mockExpenseId);
    dialogRefSpy.afterClosed.calls.mostRecent().returnValue.subscribe();

    expect(toastr.error).toHaveBeenCalledWith('Expense not Deleted successfully!', 'Error');
});

  it('should upload a file', () => {
    const mockFile = new File([''], 'test.png');
    const event = { target: { files: [mockFile] } };
    globalService.uploadFile.and.returnValue(of({ filePath: 'path/to/file' }));

    component.onFileSelected(event as any);

    expect(globalService.uploadFile).toHaveBeenCalled();
    expect(component.editForm.value.receiptUrl).toBe('path/to/file');
  });

  it('should handle error when uploading a file', () => {
    const mockFile = new File([''], 'test.png');
    const event = { target: { files: [mockFile] } };
    globalService.uploadFile.and.returnValue(throwError('Error uploading file'));

    component.onFileSelected(event as any);

    expect(toastr.error).toHaveBeenCalledWith('Error uploading file', 'error');
  });

  it('should open a receipt', () => {
    const mockUrl = 'path/to/receipt';
    spyOn(window, 'open');
    component.openReceipt(mockUrl);
    expect(window.open).toHaveBeenCalledWith(mockUrl, '_blank');
  });
});