import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { BudgetsTableComponent } from './budgets-table.component';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { MatSortModule, MatSort } from '@angular/material/sort';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { GlobalService } from '../../services/global.service';
import { ToastrService } from 'ngx-toastr';
import { UpdateServicesService } from '../../services/update-services.service';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { BehaviorSubject, of, throwError } from 'rxjs'; // Import BehaviorSubject
import { ConfirmationDialogComponent } from '../../confirmation-dialog/confirmation-dialog.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {MatIconModule} from '@angular/material/icon';
describe('BudgetsTableComponent', () => {
  let component: BudgetsTableComponent;
  let fixture: ComponentFixture<BudgetsTableComponent>;
  let globalService: jasmine.SpyObj<GlobalService>;
  let toastrService: jasmine.SpyObj<ToastrService>;
  let budgetUpdateService: jasmine.SpyObj<UpdateServicesService>;
  let router: jasmine.SpyObj<Router>;
  let dialog: MatDialog;
  let budgetAddedSubject: BehaviorSubject<void>; // Add BehaviorSubject for budgetAdded$

  const mockBudgets: any[] = [
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
      ModifiedBy: 'user'
    }
  ];

  beforeEach(async () => {
    globalService = jasmine.createSpyObj('GlobalService', ['getBudgetsByUser', 'updateBudget', 'deleteBudget']);
    toastrService = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    budgetAddedSubject = new BehaviorSubject<void>(undefined); // Initialize BehaviorSubject
    budgetUpdateService = jasmine.createSpyObj('UpdateServicesService', [], { budgetAdded$: budgetAddedSubject.asObservable() });
    router = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        MatDialogModule,
        HttpClientTestingModule,
        ReactiveFormsModule,
        BrowserAnimationsModule,MatIconModule
      ],
      declarations: [BudgetsTableComponent, ConfirmationDialogComponent],
      providers: [
        { provide: GlobalService, useValue: globalService },
        { provide: ToastrService, useValue: toastrService },
        { provide: UpdateServicesService, useValue: budgetUpdateService },
        { provide: Router, useValue: router }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(BudgetsTableComponent);
    component = fixture.componentInstance;
    dialog = TestBed.inject(MatDialog);

    spyOn(localStorage, 'getItem').and.returnValue('123');
    globalService.getBudgetsByUser.and.returnValue(of(mockBudgets));

    // Mock paginator and sort
    component.paginator = { pageSize: 10 } as MatPaginator;
    component.sort = {} as MatSort;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load budgets on ngOnInit', fakeAsync(() => {
    component.ngOnInit();
    tick();
    expect(component.dataSource.data).toEqual(mockBudgets);
  }));

  it('should handle budget update notification', fakeAsync(() => {
    spyOn(component, 'loadBudgets');
    budgetAddedSubject.next(); // Use BehaviorSubject to emit a value
    tick();
    expect(component.loadBudgets).toHaveBeenCalled();
  }));

  it('should set paginator and sort in ngAfterViewInit', () => {
    component.ngAfterViewInit();
    expect(component.dataSource.paginator).toBeDefined();
    expect(component.dataSource.sort).toBeDefined();
  });

  it('should handle error loading budgets', fakeAsync(() => {
    globalService.getBudgetsByUser.and.returnValue(throwError(() => new Error('Error')));
    component.loadBudgets();
    tick();
    expect(toastrService.error).toHaveBeenCalledWith('Error loading budgets:', 'error');
  }));

  it('should start edit mode', () => {
    const budget = mockBudgets[0];
    component.startEdit(budget);
    expect(component.editingBudget).toBe(budget);
    expect(component.editForm.get('amount')?.value).toBe(100);
  });

  it('should cancel edit mode', () => {
    component.editingBudget = mockBudgets[0];
    component.cancelEdit();
    expect(component.editingBudget).toBeNull();
    expect(component.editForm.pristine).toBeTrue();
  });

  it('should save edit successfully', fakeAsync(() => {
    globalService.updateBudget.and.returnValue(of({ status: true, data: 'Budget updated' }));
    component.editingBudget = mockBudgets[0];
    component.editForm.patchValue({ amount: 150 });
    component.saveEdit();
    tick();
    expect(globalService.updateBudget).toHaveBeenCalledWith(1, 150);
    expect(toastrService.success).toHaveBeenCalledWith('Budget updated', 'Success');
    expect(component.editingBudget).toBeNull();
  }));

  it('should handle save edit error', fakeAsync(() => {
    globalService.updateBudget.and.returnValue(of({ status: false, data: 'Update failed' }));
    component.editingBudget = mockBudgets[0];
    component.editForm.patchValue({ amount: 150 });
    component.saveEdit();
    tick();
    expect(toastrService.error).toHaveBeenCalledWith('Update failed', 'Error');
  }));

  it('should handle server error on save edit', fakeAsync(() => {
    globalService.updateBudget.and.returnValue(throwError(() => new Error('Server error')));
    component.editingBudget = mockBudgets[0];
    component.editForm.patchValue({ amount: 150 });
    component.saveEdit();
    tick();
    expect(toastrService.error).toHaveBeenCalledWith('Failed to update budget. Please try again.', 'Error');
  }));

  it('should not save edit if form is invalid', () => {
    component.editingBudget = mockBudgets[0];
    component.editForm.patchValue({ amount: -1 });
    component.saveEdit();
    expect(globalService.updateBudget).not.toHaveBeenCalled();
  });

  it('should delete budget after confirmation', fakeAsync(() => {
    globalService.deleteBudget.and.returnValue(of({ status: true, data: 'Budget deleted' }));
    spyOn(component, 'ngOnInit');
    spyOn(dialog, 'open').and.returnValue({
      afterClosed: () => of(true)
    } as any);

    component.deleteBudget(1);
    tick();

    expect(dialog.open).toHaveBeenCalledWith(ConfirmationDialogComponent, {
      width: '300px',
      data: { message: 'Are you sure you want to delete this budget?' }
    });
    expect(globalService.deleteBudget).toHaveBeenCalledWith(1);
    expect(toastrService.success).toHaveBeenCalledWith('Budget deleted', 'Success');
    expect(component.ngOnInit).toHaveBeenCalled();
  }));

  it('should handle delete error', fakeAsync(() => {
    globalService.deleteBudget.and.returnValue(throwError(() => new Error('Delete error')));
    spyOn(dialog, 'open').and.returnValue({
      afterClosed: () => of(true)
    } as any);

    component.deleteBudget(1);
    tick();

    expect(toastrService.error).toHaveBeenCalledWith('Budget not Deleted successfully!', 'Error');
  }));

  it('should not delete budget if dialog is canceled', fakeAsync(() => {
    spyOn(dialog, 'open').and.returnValue({
      afterClosed: () => of(false)
    } as any);

    component.deleteBudget(1);
    tick();

    expect(globalService.deleteBudget).not.toHaveBeenCalled();
  }));
});