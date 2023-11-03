import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = 'https://localhost:7154/Users';
  constructor(private http: HttpClient) { }

  LoginCheck(email: string, password: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/LoginCheck/${email}?Password=${password}`);
  }

  ForgotPassword(email: string): Observable<any> {
    return this.http.get(`${this.baseUrl}/ForgotPassoword/${email}`);
  }

}
