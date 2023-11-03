import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RecurringExpenseService {

  private apiUrl = 'https://localhost:7154/RecurringTransactions';
  UserID=localStorage.getItem("userId");
  constructor(private http: HttpClient) { }

    deleteRecurringExpense(expenseId: number): Observable<{ status: boolean; data: string }> {
      return this.http.delete<any>(`${this.apiUrl}/DeleteTransactions/${expenseId}`);
    }
}
