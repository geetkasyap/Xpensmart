import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedExpenseService {
  constructor(private http: HttpClient) {}
  filterUsers(searchTerm: string): Observable<string[]> {
    return this.http.get<string[]>('https://localhost:7154/Users/GetUserDetails/AllNames')
      .pipe(
        map(users => users.filter(user => 
          user.toLowerCase().includes(searchTerm.toLowerCase())
        ))
      );
  }

  getUserDetails(username: string): Observable<any> {
    return this.http.get(`https://localhost:7154/Users/GetUserDetails/${username}`);
  }

  createSharedAccount(sharedAccountData: any): Observable<any> {
    return this.http.post('https://localhost:7154/SharedAccounts/CreateSharedUser', sharedAccountData);
  }

  getSharedUserDetails(userID: number): Observable<any> {
    return this.http.get(`https://localhost:7154/SharedAccounts/GetSharedUserDetails/${userID}`);
  }
}
