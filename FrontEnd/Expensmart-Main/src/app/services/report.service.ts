import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  LocalUserID=localStorage.getItem("userId");
private baseUrl = 'https://localhost:7154/Report';

  constructor(private http: HttpClient) { }
  
  getExpenses(startDate: string, endDate: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/GetExpensesByDates?userId=${this.LocalUserID}&startDate=${startDate}&endDate=${endDate}`);
  }

  getBudget(startDate: string, endDate: string): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/GetBudgetByDates?userId=${this.LocalUserID}&startDate=${startDate}&endDate=${endDate}`);
  }

  getCategoryExpenses(startDate: string, endDate: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/GetCategoryExpensesByDates?userId=${this.LocalUserID}&startDate=${startDate}&endDate=${endDate}&CategoryID=1`);
  }

  getDailyExpenses(startDate: string, endDate: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/GetExpensesByDates?userId=${this.LocalUserID}&startDate=${startDate}&endDate=${endDate}`);
  }
  getTotalExpensesByCategoryID(start: string, end: string ,charID:number): Observable<any> {
    return this.http.get(`${this.baseUrl}/GenerateTotalExpenseReport?userId=${this.LocalUserID}&startDate=${start}&endDate=${end}&categoryId=${charID}`);
  }
}
