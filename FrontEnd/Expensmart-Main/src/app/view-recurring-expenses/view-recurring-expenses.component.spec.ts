import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ViewRecurringExpensesComponent } from './view-recurring-expenses.component';
import { RecurringExpenseService } from './../services/recurring-expense.service';
import { FormBuilder, FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatPaginatorModule } from '@angular/material/paginator';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSortModule } from '@angular/material/sort';

describe('ViewRecurringExpensesComponent', () => {
  let component: ViewRecurringExpensesComponent;
  let fixture: ComponentFixture<ViewRecurringExpensesComponent>;
  let recurringExpenseService: jasmine.SpyObj<RecurringExpenseService>;
  let toastrService: jasmine.SpyObj<ToastrService>;
  let dialog: jasmine.SpyObj<MatDialog>;
  let router: jasmine.SpyObj<Router>;
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<ConfirmationDialogComponent>>;
  
  beforeEach(async () => {
    const recurringExpenseServiceSpy = jasmine.createSpyObj('RecurringExpenseService', ['deleteRecurringExpense']);
    const toastrServiceSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    const dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule,MatFormFieldModule,MatPaginatorModule,
        MatInputModule,
        MatSelectModule,MatChipsModule,FormsModule,MatTableModule,BrowserAnimationsModule,MatSortModule,
        MatIconModule,
        MatDatepickerModule],
      declarations: [ViewRecurringExpensesComponent],
      providers: [
        { provide: RecurringExpenseService, useValue: recurringExpenseServiceSpy },
        { provide: ToastrService, useValue: toastrServiceSpy },
        { provide: MatDialog, useValue: dialogSpy },
        { provide: Router, useValue: routerSpy },
        FormBuilder
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ViewRecurringExpensesComponent);
    component = fixture.componentInstance;
    recurringExpenseService = TestBed.inject(RecurringExpenseService) as jasmine.SpyObj<RecurringExpenseService>;
    toastrService = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;
    dialog = TestBed.inject(MatDialog) as jasmine.SpyObj<MatDialog>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['afterClosed']);
    dialog.open.and.returnValue(dialogRefSpy)
    
    spyOn(localStorage, 'getItem').and.callFake((key: string) => {
      if (key === 'userId') {
        return '1'; 
      }
      return null;
    });

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to login if userId is not found', () => {
    (localStorage.getItem as jasmine.Spy).and.returnValue(null);
    component.ngOnInit();
    expect(router.navigate).toHaveBeenCalledWith(['/Login']);
  });

  it('should load expenses on init', () => {
    spyOn(component, 'loadExpenses').and.callThrough();
    component.ngOnInit();
    expect(component.loadExpenses).toHaveBeenCalled();
  });

  it('should load categories on init', () => {
    spyOn(component, 'loadCategories').and.callThrough();
    component.ngOnInit();
    expect(component.loadCategories).toHaveBeenCalled();
  });

  it('should delete expense and show success message', () => {
    const expenseId = 1;
    const dialogRef = { afterClosed: () => of(true) };
    dialogRefSpy.afterClosed.and.returnValue(of(true));
    recurringExpenseService.deleteRecurringExpense.and.returnValue(of({ status: true, data: 'Expense deleted successfully' }));

    component.deleteExpense(expenseId);

    expect(recurringExpenseService.deleteRecurringExpense).toHaveBeenCalledWith(expenseId);
    expect(toastrService.success).toHaveBeenCalledWith('Expense deleted successfully', 'Success');
  });

  it('should handle error when deleting expense', () => {
    const expenseId = 1;
    const dialogRef = { afterClosed: () => of(true) };
    dialogRefSpy.afterClosed.and.returnValue(of(true));
    recurringExpenseService.deleteRecurringExpense.and.returnValue(of({ status: false, data: 'Error deleting expense' }));

    component.deleteExpense(expenseId);

    expect(toastrService.error).toHaveBeenCalledWith('Error deleting expense', 'Error');
  });

  it('should start editing an expense', () => {
    const expense = { expenseID: 1, amount: 100, date: new Date(), categoryID: 1, description: 'Test', expenseType: 'Personal',
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
    component.startEdit(expense);
    expect(component.editingExpense).toEqual(expense);
  });

  it('should cancel editing an expense', () => {
    component.cancelEdit();
    expect(component.editingExpense).toBeNull();
  });

  it('should apply filter', () => {
    const event = { target: { value: 'test' } };
    component.applyFilter(event as any);
    expect(component.dataSource.filter).toBe('test');
  });

  it('should toggle filter', () => {
    component.toggleFilter('Monthly Recurring');
    expect(component.selectedFilters.has('Monthly Recurring')).toBeTrue();
    component.toggleFilter('Monthly Recurring');
    expect(component.selectedFilters.has('Monthly Recurring')).toBeFalse();
  });

  it('should upload file and set receiptUrl', () => {
    const file = new File([''], 'test.png', { type: 'image/png' });
    const event = { target: { files: [file] } };
    component.onFileSelected(event as any);
    expect(component.editForm.value.receiptUrl).toBeDefined();
  });

  it('should open receipt URL', () => {
    const url = 'http://example.com/receipt.png';
    spyOn(window, 'open');
    component.openReceipt(url);
    expect(window.open).toHaveBeenCalledWith(url, '_blank');
  });
});