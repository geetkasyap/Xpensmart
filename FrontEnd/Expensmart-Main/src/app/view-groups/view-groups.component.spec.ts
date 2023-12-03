import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ViewGroupsComponent } from './view-groups.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormArray, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { of, throwError } from 'rxjs';

describe('ViewGroupsComponent', () => {
  let component: ViewGroupsComponent;
  let fixture: ComponentFixture<ViewGroupsComponent>;
  let mockRouter: any;
  let mockToastr: any;

  beforeEach(async () => {
    mockRouter = {
      navigate: jasmine.createSpy('navigate')
    };

    mockToastr = {
      success: jasmine.createSpy('success'),
      error: jasmine.createSpy('error'),
      warning: jasmine.createSpy('warning')
    };

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, ReactiveFormsModule],
      declarations: [ViewGroupsComponent],
      providers: [
        { provide: Router, useValue: mockRouter },
        { provide: ToastrService, useValue: mockToastr }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ViewGroupsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();


    spyOn(console, 'error');
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to login if userId is not found', () => {
    spyOn(localStorage, 'getItem').and.returnValue(null);
    component.ngOnInit();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/Login']);
  });

  it('should fetch groups on initialization', () => {
    spyOn(localStorage, 'getItem').and.returnValue('1');
    spyOn(component, 'fetchGroups').and.callThrough();
    component.ngOnInit();
    expect(component.fetchGroups).toHaveBeenCalled();
  });

  it('should open a group and fetch expenses', () => {

    const group = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date() };
    spyOn(component, 'fetchExpenses').and.callThrough();
    component.openGroup(group);
    expect(component.selectedGroup).toEqual(group);
    expect(component.fetchExpenses).toHaveBeenCalledWith(group.groupID);
  });

  it('should fetch expenses for a group', () => {
    const groupID = 1;
    spyOn(component.http, 'get').and.returnValue(of([]));
    component.fetchExpenses(groupID);
    expect(component.http.get).toHaveBeenCalledWith(`https://localhost:7154/Groups/GetExpensesForGroup/${groupID}`);
  });

  it('should handle error when fetching expenses', () => {
    const groupID = 1;
    spyOn(component.http, 'get').and.returnValue(throwError('Error'));
    component.fetchExpenses(groupID);
  });

  it('should open add expense modal', () => {
    component.openAddExpenseModal();
    expect(component.showAddExpenseModal).toBeTrue();
  });

  it('should close add expense modal and fetch expenses', () => {
    const group = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date() };
    component.selectedGroup = group;
    spyOn(component, 'fetchExpenses').and.callThrough();
    component.closeAddExpenseModal();
    expect(component.showAddExpenseModal).toBeFalse();
    expect(component.fetchExpenses).toHaveBeenCalledWith(group.groupID);
  });

  it('should validate total correctly', () => {
    component.reconciliationForm = component.fb.group({
      totalAmount: [100],
      userExpenses: component.fb.array([component.fb.control(50), component.fb.control(50)])
    });
    component.validateTotal();
    expect(component.totalMismatch).toBeFalse();
  });

  it('should handle file selection', () => {
    const event = {
      target: {
        files: [new File([''], 'test.png')]
      }
    } as unknown as Event;
    spyOn(component.http, 'post').and.returnValue(of({ filePath: 'path/to/file' }));
    component.onOneFileSelected(event);
    expect(component.reconciliationForm.get('fileUrl')?.value).toEqual('path/to/file');
  });

  it('should delete a group', () => {
    const groupId = 1;
    spyOn(component.http, 'delete').and.returnValue(of({ status: true, data: 'Group deleted' }));
    component.deleteGroup(groupId);
    expect(component.http.delete).toHaveBeenCalledWith(`https://localhost:7154/Groups/DeleteGroup/${groupId}`);
    expect(mockToastr.success).toHaveBeenCalledWith('Group deleted', 'Success');
  });

  it('should handle error when deleting a group', () => {
    const groupId = 1;
    spyOn(component.http, 'delete').and.returnValue(throwError('Error'));
    component.deleteGroup(groupId);
    expect(mockToastr.error).toHaveBeenCalledWith('Failed to delete group.', 'Settle the Expenses first');
  });

  it('should submit reconciliation form', () => {
    component.reconciliationForm = component.fb.group({
      totalAmount: [100],
      date: ['2023-01-01'],
      categoryID: [1],
      fileUrl: [''],
      userExpenses: component.fb.array([component.fb.control(50), component.fb.control(50)])
    });
    spyOn(component.http, 'post').and.returnValue(of({ status: true, data: 'Reconciliation added' }));
    component.onSubmit();
    });

  it('should handle error on reconciliation form submission', () => {
    component.reconciliationForm = component.fb.group({
      totalAmount: [100],
      date: ['2023-01-01'],
      categoryID: [1],
      fileUrl: [''],
      userExpenses: component.fb.array([component.fb.control(50), component.fb.control(50)])
    });
    spyOn(component.http, 'post').and.returnValue(throwError('Error'));
    component.onSubmit();
    expect(mockToastr.error).toHaveBeenCalledWith('No group selected', 'Error');
  });

  it('should open reconciliation form', () => {
    const group = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date() };
    component.selectedGroup = group;
    spyOn(component, 'fetchGroupUsers').and.callThrough();
    component.openReconciliationForm();
    expect(component.showReconciliationForm).toBeTrue();
    expect(component.fetchGroupUsers).toHaveBeenCalledWith(group.groupID);
  });

  it('should settle expenses', () => {
    component.settleExpensesForm = component.fb.group({
      users: component.fb.array([
        component.fb.group({
          userID: 1,
          userName: 'User  1',
          totalOwed: 50,
          expenses: component.fb.array([])
        })
      ])
    });
  
    component.selectedGroup = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date() };
  
    spyOn(component.http, 'post').and.returnValue(of({ status: true, data: 'Expenses settled' }));
    component.settleExpenses();
    expect(component.http.post).toHaveBeenCalled();
    expect(mockToastr.success).toHaveBeenCalledWith('Expenses settled', 'Success');
  });

  it('should handle error on settling expenses', () => {
    component.settleExpensesForm = component.fb.group({
      users: component.fb.array([
        component.fb.group({
          userID: 1,
          userName: 'User  1',
          totalOwed: 50,
          expenses: component.fb.array([])
        })
      ])
    });
  
    component.selectedGroup = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date() };
  
    spyOn(component.http, 'post').and.returnValue(throwError('Error'));
    component.settleExpenses();
    expect(mockToastr.error).toHaveBeenCalledWith('Failed to settle expenses');
  });

  it('should set user expenses correctly', () => {
    component.groupUsers = [{ userID: 1, userName: 'User  1', amount: 0 }];
    component.setUserExpenses();
    expect(component.userExpenses.length).toBe(1);
    expect(component.userExpenses.at(0).value).toBe(0);
  });

  it('should fetch categories', () => {
    spyOn(component.http, 'get').and.returnValue(of(['Category 1', 'Category 2']));
    component.fetchCategories();
    expect(component.http.get).toHaveBeenCalledWith('https://localhost:7154/GetCategories');
    expect(component.categories.length).toBe(2);
  });

  it('should handle error when fetching categories', () => {
    spyOn(component.http, 'get').and.returnValue(throwError('Error'));
    component.fetchCategories();
    expect(console.error).toHaveBeenCalledWith('Error fetching categories:', 'Error');
  });

  it('should fetch group users', () => {
    const groupId = 1;
    spyOn(component.http, 'get').and.returnValue(of([{ userID: 1, userName: 'User  1' }]));
    component.fetchGroupUsers(groupId);
    expect(component.http.get).toHaveBeenCalledWith(`https://localhost:7154/Groups/GetUsersInGroup/${groupId}`);
    expect(component.groupUsers.length).toBe(1);
  });

  it('should handle error when fetching group users', () => {
    const groupId = 1;
    spyOn(component.http, 'get').and.returnValue(throwError('Error'));
    component.fetchGroupUsers(groupId);
    expect(console.error).toHaveBeenCalledWith('Error fetching group users:', 'Error');
  });

  it('should handle file selection for expenses', () => {
    component.settleExpensesForm = component.fb.group({
      users: component.fb.array([
        component.fb.group({
          userID: 1,
          userName: 'User  1',
          totalOwed: 50,
          expenses: component.fb.array([
            component.fb.group({
              amount: [100],
              description: ['Test Expense'],
              categoryID: [1],
              fileUrl: ['']
            })
          ])
        })
      ])
    });
    const event = {
      target: {
        files: [new File([''], 'test.png')]
      }
    } as unknown as Event;

    spyOn(component.http, 'post').and.returnValue(of({ filePath: 'path/to/file' }));
    component.onFileSelected(0, 0, event);

    const userGroup = component.settleExpensesForm.get('users') as FormArray;
    const expensesFormArray = userGroup.at(0).get('expenses') as FormArray;
    expect(expensesFormArray.at(0).get('fileUrl')?.value).toEqual('path/to/file');
  });

  it('should settle expenses', () => {
    component.settleExpensesForm = component.fb.group({
      users: component.fb.array([
        component.fb.group({
          userID: 1,
          userName: 'User  1',
          totalOwed: 50,
          expenses: component.fb.array([])
        })
      ])
    });
  
    component.selectedGroup = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date() };
  
    spyOn(component.http, 'post').and.returnValue(of({ status: true, data: 'Expenses settled' }));
    component.settleExpenses();
    expect(component.http.post).toHaveBeenCalled();
    expect(mockToastr.success).toHaveBeenCalledWith('Expenses settled', 'Success');
  });

  it('should handle error on settling expenses', () => {
    component.settleExpensesForm = component.fb.group({
      users: component.fb.array([
        component.fb.group({
          userID: 1,
          userName: 'User  1',
          totalOwed: 50,
          expenses: component.fb.array([])
        })
      ])
    });
  
    component.selectedGroup = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date() };
  
    spyOn(component.http, 'post').and.returnValue(throwError('Error'));
    component.settleExpenses();
    expect(mockToastr.error).toHaveBeenCalledWith('Failed to settle expenses');
  });

  it('should populate settle expenses form', () => {
    component.settleExpensesUsers = [
      { userID: 1, userName: 'User1', totalOwed: 100, expenses: [] }
    ];
    component.populateSettleExpensesForm();
    expect(component.settleExpensesUserControls.length).toBe(1);
    expect(component.settleExpensesUserControls.at(0).get('userName')?.value).toEqual('User1');
  });

  it('should create expense form group', () => {
    const expenseGroup = component.createExpenseFormGroup({ amount: 50, description: 'Test', categoryID: 1 });
    expect(expenseGroup.get('amount')?.value).toBe(50);
    expect(expenseGroup.get('description')?.value).toBe('Test');
  });

  it('should add user expense', () => {
    component.settleExpensesForm = component.fb.group({
      users: component.fb.array([component.fb.group({ expenses: component.fb.array([]) })])
    });
    component.addUserExpense(0);
  });

  it('should fetch group users for settlement', () => {
    const groupId = 1;
    spyOn(component.http, 'get').and.returnValue(of([{ userID: 1, userName: 'User  1', expenses: [] }]));
    component.fetchGroupUsersForSettlement(groupId);
    expect(component.http.get).toHaveBeenCalledWith(`https://localhost:7154/Groups/GetUserSettlementDetails/${groupId}`);
    expect(component.settleExpensesUserControls.length).toBe(1);
  });

  it('should handle error when fetching group users for settlement', () => {
    const groupId = 1;
    spyOn(component.http, 'get').and.returnValue(throwError('Error'));
    component.fetchGroupUsersForSettlement(groupId);
    expect(console.error).toHaveBeenCalledWith('Error fetching group users for settlement:', 'Error');
  });
 it('should set user expenses correctly', () => {
    component.groupUsers = [{ userID: 1, userName: 'User   1', amount: 0 }];
    component.setUserExpenses();
    expect(component.userExpenses.length).toBe(1);
    expect(component.userExpenses.at(0).value).toBe(0);
  });

  it('should fetch categories', () => {
    spyOn(component.http, 'get').and.returnValue(of(['Category 1', 'Category 2']));
    component.fetchCategories();
    expect(component.http.get).toHaveBeenCalledWith('https://localhost:7154/GetCategories');
    expect(component.categories.length).toBe(2);
  });

  it('should handle error when fetching categories', () => {
    spyOn(component.http, 'get').and.returnValue(throwError('Error'));
    component.fetchCategories();
    expect(console.error).toHaveBeenCalledWith('Error fetching categories:', 'Error');
  });

  it('should fetch group users', () => {
    const groupId = 1;
    spyOn(component.http, 'get').and.returnValue(of([{ userID: 1, userName: 'User   1' }]));
    component.fetchGroupUsers(groupId);
    expect(component.http.get).toHaveBeenCalledWith(`https://localhost:7154/Groups/GetUsersInGroup/${groupId}`);
    expect(component.groupUsers.length).toBe(1);
  });

  it('should handle error when fetching group users', () => {
    const groupId = 1;
    spyOn(component.http, 'get').and.returnValue(throwError('Error'));
    component.fetchGroupUsers(groupId);
    expect(console.error).toHaveBeenCalledWith('Error fetching group users:', 'Error');
  });

  it('should handle error when fetching group users for settlement', () => {
    const groupId = 1;
    spyOn(component.http, 'get').and.returnValue(throwError('Error'));
    component.fetchGroupUsersForSettlement(groupId);
    expect(console.error).toHaveBeenCalledWith('Error fetching group users for settlement:', 'Error');
  });

  it('should settle expenses', () => {
 component.settleExpensesForm = component.fb.group({
      users: component.fb.array([
        component.fb.group({
          userID: 1,
          userName: 'User  1',
          totalOwed: 50,
          expenses: component.fb.array([])
        })
      ])
    });
  
    component.selectedGroup = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date() };
  
    spyOn(component.http, 'post').and.returnValue(of({ status: true, data: 'Expenses settled' }));
    component.settleExpenses();
    expect(component.http.post).toHaveBeenCalled();
    expect(mockToastr.success).toHaveBeenCalledWith('Expenses settled', 'Success');
  });

  it('should handle error on settling expenses', () => {
    component.settleExpensesForm = component.fb.group({
      users: component.fb.array([
        component.fb.group({
          userID: 1,
          userName: 'User  1',
          totalOwed: 50,
          expenses: component.fb.array([])
        })
      ])
    });
  
    component.selectedGroup = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date() };
    spyOn(component.http, 'post').and.returnValue(throwError('Error'));
    component.settleExpenses();
    expect(mockToastr.error).toHaveBeenCalledWith('Failed to settle expenses');
  });

  it('should populate settle expenses form', () => {
    component.settleExpensesUsers = [
      { userID: 1, userName: 'User   1', totalOwed: 100, expenses: [] }
    ];
    component.populateSettleExpensesForm();
    expect(component.settleExpensesUserControls.length).toBe(1);
    expect(component.settleExpensesUserControls.at(0).get('userName')?.value).toEqual('User   1');
  });

  it('should create expense form group', () => {
    const expenseGroup = component.createExpenseFormGroup({ amount: 50, description: 'Test', categoryID: 1 });
    expect(expenseGroup.get('amount')?.value).toBe(50);
    expect(expenseGroup.get('description')?.value).toBe('Test');
  });

  it('should add user expense', () => {
    component.settleExpensesForm = component.fb.group({
      users: component.fb.array([component.fb.group({ expenses: component.fb.array([]) })])
    });
    component.addUserExpense(0);
  });

  it('should fetch group users for settlement', () => {
    const groupId = 1;
    spyOn(component.http, 'get').and.returnValue(of([{ userID: 1, userName: 'User   1', expenses: [] }]));
    component.fetchGroupUsersForSettlement(groupId);
    expect(component.http.get).toHaveBeenCalledWith(`https://localhost:7154/Groups/GetUserSettlementDetails/${groupId}`);
    expect(component.settleExpensesUserControls.length).toBe(1);
  });

  it('should handle error when fetching group users for settlement', () => {
    const groupId = 1;
    spyOn(component.http, 'get').and.returnValue(throwError('Error'));
    component.fetchGroupUsersForSettlement(groupId);
    expect(console.error).toHaveBeenCalledWith('Error fetching group users for settlement:', 'Error');
  });
  it('should settle expenses successfully', () => {
    component.settleExpensesForm = component.fb.group({
      users: component.fb.array([
        component.fb.group({
          userID: 1,
          userName: 'User  1',
          totalOwed: 50,
          expenses: component.fb.array([])
        })
      ])
    });
  
    component.selectedGroup = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date() };
  
    spyOn(component.http, 'post').and.returnValue(of({ status: true, data: 'Expenses settled' }));
    spyOn(component, 'fetchExpenses').and.callThrough();
    component.settleExpenses();
    expect(component.http.post).toHaveBeenCalledWith(`https://localhost:7154/Groups/SettleGroupExpenses/${component.locUserID}`, {
      groupID: component.selectedGroup.groupID,
      userSettlements: component.settleExpensesUserControls.value
    });
    expect(mockToastr.success).toHaveBeenCalledWith('Expenses settled', 'Success');
    expect(component.showSettleExpensesModal).toBeFalse();
    expect(component.fetchExpenses).toHaveBeenCalledWith(component.selectedGroup.groupID);
  });

  it('should submit reconciliation form successfully when form is valid and no total mismatch', () => {
    component.reconciliationForm = component.fb.group({
      totalAmount: [100],
      date: ['2023-01-01'],
      categoryID: [1],
      fileUrl: [''],
      userExpenses: component.fb.array([component.fb.control(50), component.fb.control(50)]) 
    });
    
    component.totalMismatch = false;
    component.selectedGroup = {groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date() };
  
    spyOn(component.http, 'post').and.returnValue(of({ status: true, data: 'Reconciliation added' }));
    component.onSubmit();

  });
  
  it('should not submit reconciliation form when form is invalid', () => {
    component.reconciliationForm = component.fb.group({
      totalAmount: [null],
      date: ['2023-01-01'],
      categoryID: [1],
      fileUrl: [''],
      userExpenses: component.fb.array([component.fb.control(50), component.fb.control(0)])
    });
    
    component.totalMismatch = true;
    component.selectedGroup = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date()}; 

    component.onSubmit();
  });
  
  it('should handle error on reconciliation form submission', () => {
    component.reconciliationForm = component.fb.group({
      totalAmount: [100],
      date: ['2023-01-01'],
      categoryID: [1],
      fileUrl: [''],
      userExpenses: component.fb.array([component.fb.control(50), component.fb.control(50)])
    });
    
    component.totalMismatch = false;
    component.selectedGroup = { groupID: 1, groupName: 'Test Group', isActive: true, createdOn: new Date(), modifiedOn: new Date()};
  
    spyOn(component.http, 'post').and.returnValue(throwError('Error'));
    component.onSubmit();

    expect(component.toastr.error).toHaveBeenCalledWith('Reconciliation Expense not added successfully!', 'Error');
  });
  
  it('should call navigateToGroupCreation', () => {
    component.navigateToGroupCreation();
  });

  it('should deleteGroup successfully', () => {
    component.deleteGroup(0);
  });
  it('should openReconciliationForm successfully', () => {
    component.openReconciliationForm();
  });
  it('should openSettleExpenses successfully', () => {
    component.openSettleExpenses();
  });



});
