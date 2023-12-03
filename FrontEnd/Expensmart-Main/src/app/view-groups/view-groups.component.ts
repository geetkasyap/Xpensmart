import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { 
  FormArray, 
  FormBuilder, 
  FormGroup, 
  FormControl, 
  Validators, 
  AbstractControl
} from '@angular/forms';
interface User {
  userID: number;
  userName: string;
  amount: number;
}

interface Group {
  groupID: number;
  groupName: string;
  isActive: boolean;
  createdOn: Date;
  modifiedOn: Date;
}

interface Expense {
  expenseID: number;
  userID: number;
  amount: number;
  date: Date;
  categoryID: number;
  receiptUrl: string;
  description: string;
  expenseType: string;
  isActive: boolean;
  isRecurring: boolean;
  createdOn: Date;
  modifiedOn: Date;
  createdBy: string;
  modifiedBy: string;
  userName?: string;
}
interface SettleExpenseUser {
  userID: number;
  userName: string;
  totalOwed: number;
  expenses: SettleExpenseDetail[];
}

interface SettleExpenseDetail {
  expenseID?: number;
  amount: number;
  description: string;
  categoryID: number;
  fileUrl?: string;
}
interface ExpenseFormValue {
  amount: number;
  description: string;
  categoryID: number;
  fileUrl?: string;
}
@Component({
  selector: 'app-view-groups',
  standalone: false,

  templateUrl: './view-groups.component.html',
  styleUrl: './view-groups.component.scss'
})
export class ViewGroupsComponent implements OnInit{
  
  settleExpensesForm!: FormGroup;
  groups: Group[] = [];
  selectedGroup: Group | null = null;
  hoveredGroup: Group | null = null;
  expenses: Expense[] = [];
  otherExpenses: Expense[] = [];
  myExpenses: Expense[] = [];
  showAddExpenseModal: boolean = false;
  locUserID = Number(localStorage.getItem("userId"));
  reconciliationForm!: FormGroup;
  categories: any[] = [];
  groupUsers: User[] = [];
  totalMismatch = false;
  showReconciliationForm = false;
  showSettleExpensesModal: boolean = false;
  settleExpensesUsers: SettleExpenseUser[] = [];
  settleCategories: any[] = [];

  constructor(public http: HttpClient, private router: Router,
    public fb: FormBuilder,public cdr:ChangeDetectorRef,
      public toastr: ToastrService,) {

      }
  
  get userExpenses(): FormArray {
    return this.reconciliationForm.get('userExpenses') as FormArray;
  }
  
  ngOnInit(): void {
        const userId = localStorage.getItem('userId') ;
    if (userId ==undefined || null) {
      this.router.navigate(['/Login']);
    }

    if (this.selectedGroup) {
      this.fetchExpenses(this.selectedGroup.groupID);
    }
    else{
      this.fetchGroups();
      this.fetchCategories();
      this.initializeReconciliationForm();
      this.initializeSettleExpensesForm();
    }
    
  }
  initializeReconciliationForm() {
    this.reconciliationForm = this.fb.group({
      totalAmount: ['', [Validators.required, Validators.min(0.1)]],
      date: ['', Validators.required],
      categoryID: ['', Validators.required],
      fileUrl: [''],
      userExpenses: this.fb.array([])
    });
  }
  fetchGroups(): void {
    this.http.get<Group[]>(`https://localhost:7154/Groups/GetGroupsForUser/${this.locUserID}`)
      .subscribe(
        data => {
          this.groups = data;
        },
        error => console.error('Error fetching groups:', error)
      );
  }

  openGroup(group: Group): void {
    this.selectedGroup = group;
    this.fetchExpenses(group.groupID);
    this.showReconciliationForm = false; 
    this.groupUsers = []; 
  }

  async fetchExpenses(groupID: number): Promise<void> {
    this.http.get<Expense[]>(`https://localhost:7154/Groups/GetExpensesForGroup/${groupID}`)
      .subscribe(async (data) => {
        this.expenses = data;
  
        for (const expense of this.expenses) {
          if (expense.userID === this.locUserID) {
            expense.userName = 'You';
          } else {
            expense.userName = await this.fetchUserName(expense.userID);
          }
        }
  
        this.expenses.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
        this.cdr.detectChanges();
      },
      error => console.error('Error fetching expenses:', error));
  }

  navigateToGroupCreation(): void {
    this.router.navigate(['/GroupCreation']);
  }

  openAddExpenseModal(): void {
    this.showAddExpenseModal = true;
  }

  closeAddExpenseModal(): void {
    this.showAddExpenseModal = false;
    if (this.selectedGroup) {
      this.fetchExpenses(this.selectedGroup.groupID);
    }
    this.ngOnInit();
  }
  async fetchUserName(userId: number): Promise<string | undefined> {
    try {
      const response = await this.http.get<{ userName: string } | undefined>(`https://localhost:7154/Users/GetUserNamesByUserID/${encodeURIComponent(userId)}`).toPromise();
      if (response && response.userName) {
        return response.userName;
      } else {
        console.error('Response is undefined or does not contain userName');
        return undefined;
      }
    } catch (error) {
      console.error('Error fetching user name:', error);
      return undefined;
    }
  }

  fetchCategories() {
    this.http.get('https://localhost:7154/GetCategories').subscribe(
      (data: any) => {
        this.categories = data;
      },
      error => console.error('Error fetching categories:', error)
    );
  }
  
  fetchGroupUsers(groupId: number): void {
    this.http.get<User[]>(`https://localhost:7154/Groups/GetUsersInGroup/${groupId}`).subscribe(
      data => {
        this.groupUsers = data.map(user => ({ ...user, amount: 0 })); 
        this.setUserExpenses(); 
      },
      error => console.error('Error fetching group users:', error)
    );
  }
setUserExpenses() {
  const userExpensesArray = this.reconciliationForm.get('userExpenses') as FormArray;
  userExpensesArray.clear();

  this.groupUsers.forEach(user => {
    userExpensesArray.push(this.fb.control(0, [Validators.required, Validators.min(0)]));
  });
}

  validateTotal() {
    const totalAmount = this.reconciliationForm.get('totalAmount')?.value;
    const userExpensesArray = this.reconciliationForm.get('userExpenses') as FormArray;
    const userTotal = userExpensesArray.controls.reduce((sum, control) => sum + (control.value || 0), 0);
    this.totalMismatch = totalAmount !== userTotal;
  }
  onOneFileSelected(event: Event) {
    const target = event.target as HTMLInputElement;
    const file = target.files ? target.files[0] : null;
    if (file) {
      const formData = new FormData();
      formData.append('receipt', file);
      this.http.post<{ filePath: string }>('http://localhost:8080/uploads', formData).subscribe(
        response => {
          this.reconciliationForm.patchValue({ fileUrl: response.filePath });
        },
        error => console.error('Error uploading file', error)
      );
    }
  }

  deleteGroup(groupId: number): void {
    if (!groupId) {
      this.toastr.warning('Please select a group first.');
      return;
    }
  
    this.http.delete(`https://localhost:7154/Groups/DeleteGroup/${groupId}`).subscribe(
      (response:any) => {
        if(response.status===true)
        {
          this.toastr.success(response.data,"Success");
        }
        else
      {
        this.toastr.error(response.data,"Error");
      }
        this.fetchGroups(); 
        this.selectedGroup = null; 
      },
      error => {
        this.toastr.error('Failed to delete group.','Settle the Expenses first');
        console.error('Error deleting group:', error);
      }
    );
  }

 
onSubmit() {
  if (!this.selectedGroup) {
    this.toastr.error('No group selected', 'Error');
    return;
  }

  if (this.reconciliationForm.valid && !this.totalMismatch) {
    const userExpenses = this.reconciliationForm.get('userExpenses') as FormArray;
    const request = {
      groupID: this.selectedGroup?.groupID,
      totalAmount: this.reconciliationForm.get('totalAmount')?.value,
      date: this.reconciliationForm.get('date')?.value,
      categoryID: this.reconciliationForm.get('categoryID')?.value,
      fileUrl: this.reconciliationForm.get('fileUrl')?.value,
      createdBy: 'system',
      userExpenses: this.groupUsers.map((user, index) => ({
        userID: user.userID,
        amount: userExpenses.at(index).value
      }))
    };

    this.http.post(`https://localhost:7154/Groups/AddReconciliationExpense/${this.locUserID}`, request).subscribe(
      (response:any) => {
        if(response.status===true)
        {
          this.toastr.success(response.data, 'Success');
        }
        else
        {
          this.toastr.error(response.data, 'Error');
        }
      this.fetchExpenses(this.selectedGroup!.groupID);
      this.resetReconciliationForm(); 
      },
      error => {
        this.toastr.error('Reconciliation Expense not added successfully!', 'Error');
      }
    );
  }
}

onExpenseAdded(): void {
  if (this.selectedGroup) {
      this.fetchExpenses(this.selectedGroup.groupID);
  }
}

resetReconciliationForm() {
  this.reconciliationForm.reset();
  this.totalMismatch = false; 
  this.showReconciliationForm = false;
}
  openReconciliationForm(): void {
    if (!this.selectedGroup) {
      alert('Please select a group first.');
      return;
    }
    this.showReconciliationForm = true;
    this.fetchGroupUsers(this.selectedGroup.groupID);
  }

  initializeSettleExpensesForm() {
    this.settleExpensesForm = this.fb.group({
      users: this.fb.array([])
    });
  }

  get settleExpensesUserControls(): FormArray {
    return this.settleExpensesForm.get('users') as FormArray;
  }

  openSettleExpenses(): void {
    if (!this.selectedGroup) {
      this.toastr.warning('Please select a group first');
      return;
    }

    this.fetchGroupUsersForSettlement(this.selectedGroup.groupID);
  }

  populateSettleExpensesForm() {
    const usersFormArray = this.settleExpensesUserControls;
    usersFormArray.clear();

    this.settleExpensesUsers.forEach(user => {
      const userGroup = this.fb.group({
        userID: [user.userID],
        userName: [user.userName],
        totalOwed: [user.totalOwed],
        expenses: this.fb.array([])
      });

      const expensesFormArray = userGroup.get('expenses') as FormArray;
      user.expenses.forEach(expense => {
        expensesFormArray.push(this.createExpenseFormGroup(expense));
      });

      usersFormArray.push(userGroup);
    });
  }

  onFileSelected(userIndex: number, expenseIndex: number, event: Event) {
    const target = event.target as HTMLInputElement;
    const file = target.files ? target.files[0] : null;
    if (file) {
      const formData = new FormData();
      formData.append('receipt', file);
      this.http.post<{ filePath: string }>('http://localhost:8080/uploads', formData).subscribe(
        response => {
          const usersFormArray = this.settleExpensesUserControls;
          const userGroup = usersFormArray.at(userIndex) as FormGroup;
          const expensesFormArray = userGroup.get('expenses') as FormArray;
          expensesFormArray.at(expenseIndex).patchValue({ fileUrl: response.filePath });
        },
        error => {
          console.error('Error uploading file', error);
          this.toastr.error('File upload failed');
        }
      );
    }
  }

  settleExpenses(): void {
    if (this.settleExpensesForm.invalid) {
      this.toastr.warning('Please fill all required fields');
      return;
    }

    const settlementRequest = {
      groupID: this.selectedGroup?.groupID,
      userSettlements: this.settleExpensesUserControls.value
    };

    this.http.post(`https://localhost:7154/Groups/SettleGroupExpenses/${this.locUserID}`,settlementRequest)
      .subscribe(
        (response:any) => {
          if(response.status===true)
          {
            this.toastr.success(response.data,"Success");
          }
          else
          {
            this.toastr.error(response.data,"Error"); 
          }
          this.showSettleExpensesModal = false;
          this.fetchExpenses(this.selectedGroup!.groupID);
        },
        error => {
          console.error('Error settling expenses:', error);
          this.toastr.error('Failed to settle expenses');
        }
      );
  }
  createUserExpenseGroup(user?: any): FormGroup {
    return this.fb.group({
      userID: [user?.userID || null],
      userName: [user?.userName || ''],
      totalOwed: [user?.totalOwed || 0],
      expenses: this.fb.array([])
    });
  }

  createExpenseFormGroup(expense?: Partial<ExpenseFormValue>): FormGroup {
    return this.fb.group({
      amount: [expense?.amount || 0, [Validators.required, Validators.min(0)]],
      description: [expense?.description || '', Validators.required],
      categoryID: [expense?.categoryID || null, Validators.required],
      fileUrl: [expense?.fileUrl || '']
    });
  }

  getUserExpensesControls(userGroup: AbstractControl): FormGroup[] {
    const expensesFormArray = userGroup.get('expenses') as FormArray;
    return expensesFormArray ? 
      expensesFormArray.controls as FormGroup[] : 
      [];
  }

  addUserExpense(userIndex: number): void {
    const userGroup = this.settleExpensesUserControls.at(userIndex) as FormGroup;
    const expensesFormArray = userGroup.get('expenses') as FormArray;
    
    expensesFormArray.push(this.createExpenseFormGroup());
  }

  fetchGroupUsersForSettlement(groupId: number): void {
    this.http.get<any[]>(`https://localhost:7154/Groups/GetUserSettlementDetails/${groupId}`)
      .subscribe(
        (users) => {
          const usersFormArray = this.settleExpensesUserControls;
          usersFormArray.clear();

          users.forEach(user => {
            const userGroup = this.createUserExpenseGroup(user);
            
            const expensesFormArray = userGroup.get('expenses') as FormArray;
            (user.expenses || []).forEach((expense: any) => {
              expensesFormArray.push(this.createExpenseFormGroup(expense));
            });

            usersFormArray.push(userGroup);
          });

          this.showSettleExpensesModal = true;
        },
        error => {
          console.error('Error fetching group users for settlement:', error);
          this.toastr.error('Failed to fetch settlement details');
        }
      );
  }
}