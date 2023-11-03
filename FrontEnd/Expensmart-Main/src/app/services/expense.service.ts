import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExpensesEntity } from '../models/expenses-entity-model';
import { ConsizedExpense } from '../models/expense-entity-consizedModel';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private apiUrl = 'https://localhost:7154/Expenses';
  UserID=localStorage.getItem("userId");
  UserIdNumber=localStorage.getItem("userId") as unknown as number;
  constructor(private http: HttpClient) { }

  getExpenses(): Observable<ExpensesEntity[]> {
    return this.http.get<ExpensesEntity[]>(`${this.apiUrl}/GetExpenseDetails/0?UserID=${this.UserID}`);
  }
  getExpensesByUserID(userId: number): Observable<ExpensesEntity[]> {
    return this.http.get<ExpensesEntity[]>(`${this.apiUrl}/GetExpenseDetails/0?UserID=${userId}`);
  }
  getExpensesByDateString(): Observable<ConsizedExpense[]> {
    
    const params =
    {
      UserID:this.UserIdNumber,
    }
    return this.http.get<ConsizedExpense[]>(`${this.apiUrl}/GetExpenseDetails/0`,{params});
  }
  addExpense(expense: ExpensesEntity): Observable<ExpensesEntity> {
    return this.http.post<ExpensesEntity>(`${this.apiUrl}/AddExpenses`, expense);
  }
updateExpense(expense: ExpensesEntity): Observable<{ message: string }> {
  return this.http.put<{ message: string }>(
      `${this.apiUrl}/UpdateExpenses/${expense.expenseID}?UserID=${this.UserID}`, 
      expense
  );
}
deleteExpense(expenseId: number): Observable<{ status: boolean; data: string }> {
  return this.http.delete<any>(`${this.apiUrl}/DeleteExpenses/${expenseId}`);
}
  getCategories() {
    return this.http.get<any[]>(`https://localhost:7154/GetCategories`);
  }
}
