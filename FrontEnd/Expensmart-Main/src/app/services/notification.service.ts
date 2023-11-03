import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor(private http: HttpClient) {}

  getNotifications(userID: number): Observable<any[]> {
    return this.http.get<any[]>(`https://localhost:7154/Notifications/GetNotifications/${userID}`);
  }

  acceptInvitation(invitationID: number): Observable<any> {
    return this.http.post(`https://localhost:7154/Notifications/AcceptInvitation`, { invitationID });
  }

  rejectInvitation(invitationID: number): Observable<any> {
    return this.http.post(`https://localhost:7154/Notifications/RejectInvitation`, { invitationID });
  }
}