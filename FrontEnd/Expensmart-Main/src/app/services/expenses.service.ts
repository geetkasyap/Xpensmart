import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export interface Expense {
expenseID: number;
  userID: number;
  amount: number;
  date: string;
  categoryID: number;
  receiptUrl: string;
  description: string;
  expenseType: string;
  isActive: boolean;
  isRecurring: boolean;
  createdOn: string;
  modifiedOn: string;
  createdBy: number;
  modifiedBy: number;
}


@Injectable({
  providedIn: 'root'
})
export class ExpensesService {
  private apiUrl = 'https://localhost:7154/Expenses';
  constructor(private http: HttpClient) {}

  getExpenses(): Observable<Expense[]> {
    return this.http.get<Expense[]>(`${this.apiUrl}/GetExpenseDetails/0?UserID=0`);
  }

  getExpensesByDate(date: string): Observable<Expense[]> {
    return this.http.get<Expense[]>(`${this.apiUrl}/GetExpenseDetails/0?UserID=0`).pipe(
      map(expenses => expenses.filter(expense => new Date(expense.date).toDateString() === new Date(date).toDateString()))
    );
  }

  deleteExpense(expenseID: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/DeleteExpenses/${expenseID}`);
  }

  updateExpense(expense: Expense): Observable<any> {
    return this.http.put(`${this.apiUrl}/UpdateExpenses/${expense.expenseID}`, expense);
  }
}
