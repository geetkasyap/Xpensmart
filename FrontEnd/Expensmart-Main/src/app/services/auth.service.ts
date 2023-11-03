import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userLoggedInSource = new BehaviorSubject<boolean>(false);
  userLoggedIn$ = this.userLoggedInSource.asObservable();

  login() {
    this.userLoggedInSource.next(true);  }

  logout() {
    localStorage.clear();
    this.userLoggedInSource.next(false);
  }
}
