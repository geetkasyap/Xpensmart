import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SharedAccountTablesComponent } from './shared-account-tables.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ExpenseService } from '../services/expense.service';
import { of } from 'rxjs';
import { ExpensesEntity } from '../models/expenses-entity-model';
import { SharedAccountTableStructureComponent } from '../shared-account-table-structure/shared-account-table-structure.component';
import { ToastrService } from 'ngx-toastr';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatPaginatorModule } from '@angular/material/paginator';
import { FormsModule } from '@angular/forms';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSortModule } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';

describe('SharedAccountTablesComponent', () => {
  let component: SharedAccountTablesComponent;
  let fixture: ComponentFixture<SharedAccountTablesComponent>;
  let expenseServiceMock: jasmine.SpyObj<ExpenseService>;
  let routerMock: jasmine.SpyObj<Router>;
  let toastrServiceMock: jasmine.SpyObj<ToastrService>; 

  beforeEach(async () => {

    expenseServiceMock = jasmine.createSpyObj('ExpenseService', ['getExpensesByUserID','getExpenses']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);
    toastrServiceMock = jasmine.createSpyObj('ToastrService', ['success', 'error']); 

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule,MatFormFieldModule,MatPaginatorModule,
        MatInputModule,
        MatSelectModule,MatChipsModule,FormsModule,
      MatTableModule,BrowserAnimationsModule,MatSortModule,MatIconModule,
                    MatDatepickerModule],
      declarations: [SharedAccountTablesComponent, SharedAccountTableStructureComponent],
      providers: [
        { provide: ExpenseService, useValue: expenseServiceMock },
        { provide: MatDialog, useValue: {} },
        { provide: Router, useValue: routerMock },
        { provide: ToastrService, useValue: toastrServiceMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SharedAccountTablesComponent);
    component = fixture.componentInstance;
    
    const mockExpenses: ExpensesEntity[] = [
      {
        expenseID: 1,
        userID: component.user1Id,
        categoryID: 1,
        date: new Date('2023-01-01'),
        amount: 100,
        receiptUrl: 'http://example.com/receipt1.jpg',
        description: 'desc',
        expenseType: 'geet',
        isActive: true,
        isRecurring: true,
        createdOn: new Date('2023-01-01'),
        modifiedOn: new Date('2023-01-01'),
        createdBy: '',
        modifiedBy: '',
        usersEntity: undefined,
        categoriesEntity: undefined
      }
    ];

    expenseServiceMock.getExpensesByUserID.and.returnValue(of(mockExpenses));
    expenseServiceMock.getExpenses.and.returnValue(of(mockExpenses));
      
    component.dataSource1 = new MatTableDataSource<any>([]);
    component.dataSource2 = new MatTableDataSource<any>([]);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load users on init', () => {
    const userId = 1; 
    spyOn(localStorage, 'getItem').and.returnValue(userId.toString());
    spyOn(component, 'loadUsers').and.callThrough();
    spyOn(component, 'loadExpenses').and.callThrough();

    component.ngOnInit();

    expect(component.loadUsers).toHaveBeenCalled();
    expect(component.loadExpenses).toHaveBeenCalled();
  });

  it('should load users', () => {
    const userId = 1; 
    spyOn(localStorage, 'getItem').and.returnValue(userId.toString());
    const mockUsers = [{ userID: 1, userName: 'User  1' }];
    spyOn(component['http'], 'get').and.returnValue(of(mockUsers));

    component.loadUsers();

    expect(component.users).toEqual(mockUsers);
  });

  it('should load expenses for user1', () => {
    const mockExpenses: ExpensesEntity[] = [
      {
        expenseID: 1,
        userID: component.user1Id,
        categoryID: 1,
        date: new Date('2023-01-01'),
        amount: 100,
        receiptUrl: 'http://example.com/receipt1.jpg',
        description: 'desc',
        expenseType: 'geet',
        isActive: true,
        isRecurring: true,
        createdOn: new Date('2023-01-01'),
        modifiedOn: new Date('2023-01-01'),
        createdBy: '',
        modifiedBy: '',
        usersEntity: undefined,
        categoriesEntity: undefined
      }
    ];
    expenseServiceMock.getExpensesByUserID.and.returnValue(of(mockExpenses));
  
    component.loadExpenses();
  
    expect(expenseServiceMock.getExpensesByUserID).toHaveBeenCalledWith(component.user1Id);
    expect(component.dataSource1.data).toEqual(mockExpenses);
  });
  
  it('should load expenses for user2 when selected', () => {
    component.user2Id = 2; 
    const mockExpenses: ExpensesEntity[] = [
      {
        expenseID: 2,
        userID: component.user2Id,
        categoryID: 2,
        date: new Date('2023-01-02'),
        amount: 200,
        receiptUrl: 'http://example.com/receipt2.jpg',
        description: 'desc',
        expenseType: 'geet',
        isActive: true,
        isRecurring: true,
        createdOn: new Date('2023-01-02'),
        modifiedOn: new Date('2023-01-02'),
        createdBy: '',
        modifiedBy: '',
        usersEntity: undefined,
        categoriesEntity: undefined
      }
    ];
    expenseServiceMock.getExpensesByUserID.and.returnValue(of(mockExpenses));
  
    component.loadExpenses();
  
    expect(expenseServiceMock.getExpensesByUserID).toHaveBeenCalledWith(component.user2Id);
    expect(component.dataSource2.data).toEqual(mockExpenses);
  });

  it('should reset table2', () => {
    component.selectedUser2 = '2';
    component.user2Id = 2;
    component.dataSource2.data = [{ date: new Date('2023-01-02'), amount: 200, category: 'Transport' }];

    component.resetTable2();

    expect(component.selectedUser2).toBe('');
    expect(component.user2Id).toBeUndefined();
    expect(component.dataSource2.data).toEqual([]);
  });

  it('should navigate to expense form', () => {
    component.user1Id = 1;
    component.user2Id = 2;

    component.openExpenseForm();

    expect(routerMock.navigate).toHaveBeenCalledWith(['/addSharedExpenses', { user1Id: 1, user2Id: 2 }]);
  });

  it('should handle user2 change', () => {
    component.selectedUser2 = '2';
    spyOn(component, 'loadExpenses').and.callThrough();

    component.onUser2Change();

    expect(component.user2Id).toBe(2);
    expect(component.loadExpenses).toHaveBeenCalled();
  });

  it('should call resetTable2 on expense updated', () => {
    spyOn(component, 'resetTable2').and.callThrough();

    component.onExpenseUpdated();

    expect(component.resetTable2).toHaveBeenCalled();
  });
});