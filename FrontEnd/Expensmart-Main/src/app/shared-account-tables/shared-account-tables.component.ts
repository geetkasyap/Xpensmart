import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ExpenseService } from '../services/expense.service';

interface UserWithUserName {
  userID: number;
  userName: string;
}
@Component({
  selector: 'app-shared-account-tables',
  standalone: false,
  templateUrl: './shared-account-tables.component.html',
  styleUrls: ['./shared-account-tables.component.scss']
})
export class SharedAccountTablesComponent implements OnInit {
  users: any[] = [];
  selectedUser2: string = '';
  user1Id: number;
  user2Id?: number;
  dataSource1 = new MatTableDataSource<any>();
  dataSource2 = new MatTableDataSource<any>();
  displayedColumns: string[] = ['date', 'amount', 'category'];

  constructor(
    private http: HttpClient,
    private expenseService: ExpenseService,
    private dialog: MatDialog,
    private router: Router
  ) {
    this.user1Id = Number(localStorage.getItem('userId'));
  }

  ngOnInit() {
    this.loadUsers();
    this.loadExpenses();
  }

  loadUsers() {
    this.http.get<UserWithUserName[]>(`https://localhost:7154/Users/GetSharedUserDetailsWithNames/${this.user1Id}`).subscribe(users => {
      this.users = users;
    });
  }

  onUser2Change() {
    this.user2Id = Number(this.selectedUser2);
    this.loadExpenses();
  }

  loadExpenses() {
    this.expenseService.getExpensesByUserID(this.user1Id).subscribe((expenses: any[]) => {
      this.dataSource1.data = expenses;
    });

    if (this.user2Id) {
      this.expenseService.getExpensesByUserID(this.user2Id).subscribe((expenses: any[]) => {
        this.dataSource2.data = expenses;
      });
    }
    else {
      this.dataSource2.data = [];
    }
  }

  openExpenseForm() {
    this.router.navigate(['/addSharedExpenses', { user1Id: this.user1Id, user2Id: this.user2Id }]);
  }
  resetTable2() {
    this.selectedUser2 = ''; 
    this.user2Id = undefined;
    this.dataSource2.data = [];
    this.loadUsers(); 
  }
  onExpenseUpdated() {
    this.resetTable2(); 
  }
}

