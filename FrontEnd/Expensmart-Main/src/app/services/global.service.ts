import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BudgetsEntity } from '../models/budget-entity-model';
import { ExpensesEntity } from '../models/expenses-entity-model';

interface Category {
  categoryID: number;
  name: string;
  isActive: boolean;
  createdOn: Date;
  modifiedOn: Date;
}

interface CategoryKeywordResponse {
  categoryKeywordID: number;
  categoryID: number;
  keyword: string;
  
  categoriesEntity: {
    categoryID: number;
    name: string;
    isActive: boolean;
    createdOn: Date;
    modifiedOn: Date;
    expensesEntities: any;
    categoryKeywordsEntities: any;
  };
}

interface SharedAccountsEntity {
  userID1: number;
  userID2: number;
  createdBy: string;
  modifiedBy: string;
}
interface Notification {
  notificationID: number;
  type: 'invitation' | 'expense' | 'shared_account';
  message: string;
  sender: string;
  timestamp: Date;
  readMessage: boolean;
}

interface IncludeConsizedExpense {
  expenseID: number;
  userID: number;
  amount: number;
  date: Date;
  categoryID: number;
  receiptUrl?: string;
  description?: string;
  expenseType?: string;
  isActive: boolean;
  isRecurring: boolean;
  createdOn: Date;
  modifiedOn: Date;
  createdBy: string;
  modifiedBy: string;
  usersEntity?: any; 
  categoriesEntity?: { 
      categoryID: number;
      name?: string;
      isActive: boolean;
      createdOn: Date;
      modifiedOn: Date;
  };
  transactionsEntities?: any[];
}

interface CategoryExpenseEntity {
  categoryName: string;
  amount: number;
  categoryId: number;
  }

  interface UserWithUserName {
    userID: number;
    userName: string;
  }

@Injectable({
  providedIn: 'root'
})

export class GlobalService {

  private baseUrl = 'https://localhost:7154';

  constructor(private http: HttpClient) { }

  ExpenseFormAddExpense(expenseData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Expenses/AddExpenses`, expenseData);
  }

  getCategoryKeywords(): Observable<any[]> {
    return this.http.get<CategoryKeywordResponse[]>(`${this.baseUrl}/CategoryKeywords/GetCategoryKeywords`);
  }

  loadCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}/GetCategories`);
  }

  convertCurrency(amount: number, fromCurrency: string, toCurrency: string): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/Expenses/ConvertCurrency?amount=${amount}&fromCurrency=${fromCurrency}&toCurrency=${toCurrency}`);
  }

  uploadFile(formData: FormData): Observable<{ filePath: string }> {
    return this.http.post<{ filePath: string }>('http://localhost:8080/uploads', formData);
  }
  addGroupExpense(userId: number, groupId: number, expenseData: any): Observable<any> {
    const obj = {
      groupID: groupId,
      expense: expenseData
    };
    return this.http.post(`${this.baseUrl}/Groups/AddGroupExpense/${userId}`, obj);
  }
  addRecurringTransaction(params: HttpParams): Observable<any> {
    return this.http.post(`${this.baseUrl}/RecurringTransactions/AddTransactions`, null, { params });
  }
  getBudgetsByUser (userId: string): Observable<BudgetsEntity[]> {
    return this.http.get<BudgetsEntity[]>(`${this.baseUrl}/Budget/GetBudgetsByUser?UserID=${userId}`);
  }

  updateBudget(budgetID: number, amount: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/Budget/UpdateBudget/${budgetID}?Amount=${amount}`, {});
  }

  deleteBudget(budgetID: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Budget/DeleteBudgets/${budgetID}`);
  }
  fetchUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Users/GetUserDetails/All`);
  }
  fetchNotifications(userId: string): Observable<Notification[]> {
    return this.http.get<Notification[]>(`${this.baseUrl}/Notification/GetnotificationDetails?UserID=${userId}`);
  }

  markNotificationAsRead(notificationId: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/Notification/ReadNotification/${notificationId}`, {});
  }

  createSharedUser (sharedAccount: SharedAccountsEntity): Observable<any> {
    return this.http.post(`${this.baseUrl}/Users/CreateSharedUser`, sharedAccount);
  }

  getUserIdByName(userName: string): Observable<{ userID: number; userName: string }> {
    return this.http.get<{ userID: number; userName: string }>(`${this.baseUrl}/Users/GetUserDetails/${encodeURIComponent(userName)}`);
  }

  deleteNotification(notificationId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Notification/DeleteNotification/${notificationId}`);
  }

  fetchUserName(userId: number): Observable<{ userName: string }> {
    return this.http.get<{ userName: string }>(`${this.baseUrl}/Users/GetUserNamesByUserID/${encodeURIComponent(userId)}`);
  }
  getRecentExpenses(userId: any): Observable<ExpensesEntity[]> {
    return this.http.get<ExpensesEntity[]>(`${this.baseUrl}/Report/top-three-recent-expenses?userId=${userId}`);
  }

  getUpcomingExpenses(userId: any): Observable<ExpensesEntity[]> {
    return this.http.get<ExpensesEntity[]>(`${this.baseUrl}/Report/upcoming-expenses?userId=${userId}`);
  }

  getRemainingExpenses(userId: any): Observable<ExpensesEntity[]> {
    return this.http.get<ExpensesEntity[]>(`${this.baseUrl}/Report/remaining-expenses?userId=${userId}`);
  }
  getMonthlyBudgets(userId: any, startDate: string, endDate: string): Observable<{ month: string, amount: number }[]> {
    return this.http.get<{ month: string, amount: number }[]>(`${this.baseUrl}/GetMonthlyBudgets?userId=${userId}&startDate=${startDate}&endDate=${endDate}`);
  }

  getExpensesByDates(userId: any, startDate: string, endDate: string): Observable<IncludeConsizedExpense[]> {
    return this.http.get<IncludeConsizedExpense[]>(`${this.baseUrl}/GetExpensesByDates?userId=${userId}&startDate=${startDate}&endDate=${endDate}`);
  }

  getCategorySummary(userId: string, startDate: string, endDate: string): Observable<CategoryExpenseEntity[]> {
    return this.http.get<CategoryExpenseEntity[]>(`${this.baseUrl}/GetCategorySummary?userId=${userId}&startDate=${startDate}&endDate=${endDate}`);
  }

  generateTotalExpenseReport(userId: string, startDate: string, endDate: string, categoryId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/GenerateTotalExpenseReport?userId=${userId}&startDate=${startDate}&endDate=${endDate}&categoryId=${categoryId}`);
  }

  getBudgetByDates(userId: string, startDate: string, endDate: string): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/GetBudgetByDates?userId=${userId}&startDate=${startDate}&endDate=${endDate}`);
  }
  getSharedUserDetails(userId: any): Observable<UserWithUserName[]> {
    return this.http.get<UserWithUserName[]>(`${this.baseUrl}/Users/GetSharedUserDetailsWithNames/${userId}`);
  }

  addExpense(expenseData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Expenses/AddExpenses`, expenseData);
  }
  getUserDetailsAllNames(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/Users/GetUserDetails/AllNames`);
  }

  getUserNamesByUserID(userId: string): Observable<{ userName: string }> {
    return this.http.get<{ userName: string }>(`${this.baseUrl}/Users/GetUserNamesByUserID/${encodeURIComponent(userId)}`);
  }

  getUserDetailsByName(userName: string): Observable<{ userID: number; userName: string }> {
    return this.http.get<{ userID: number; userName: string }>(`${this.baseUrl}/Users/GetUserDetails/${encodeURIComponent(userName)}`);
  }

  addNotification(notification: Notification): Observable<any> {
    return this.http.post(`${this.baseUrl}/Notification/AddNotifications`, notification);
  }

}