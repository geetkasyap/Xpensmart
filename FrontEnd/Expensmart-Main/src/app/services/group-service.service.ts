import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class GroupServiceService {
  private apiUrl = 'https://localhost:7154/Groups';

  constructor(private http: HttpClient) { }

  getGroupsForUser(userId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/GetGroupsForUser/${userId}`);
  }

  getExpensesForGroup(groupId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/GetExpensesForGroup/${groupId}`);
  }

  addExpenseToGroup(groupId: number, expense: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/AddExpenseToGroup?groupId=${groupId}`, expense);
  }
}
