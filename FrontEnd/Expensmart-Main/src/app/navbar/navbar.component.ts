import { GlobalService } from './../services/global.service';
import { AuthService } from './../services/auth.service';

import { Component, OnInit, OnDestroy, Output, EventEmitter, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { NumberFormatStyle } from '@angular/common';

interface Notification {
  notificationID: number;
  type: 'invitation' | 'expense' | 'shared_account';
  message: string;
  sender: string;
  timestamp: Date;
  readMessage: boolean;
}

interface SharedAccountsEntity {
  userID1: number;
  userID2: number;
  createdBy: string;
  modifiedBy: string;
}
@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NavbarComponent implements OnInit, OnDestroy {
  UserName:string='';
  LoggedUserID=localStorage.getItem('userId') as unknown as number;
  notifications: Notification[] = [];
  showNotifications = false;
  unreadCount = 0;
  private pollingInterval: any;
  @Output() LogOut:EventEmitter<string> = new EventEmitter();

  constructor(private router: Router, private http: HttpClient,private toastr: ToastrService,private authService:AuthService,public cdr:ChangeDetectorRef,private GlobalService:GlobalService) {}

  ngOnInit(): void {
    this.fetchNotifications();
    this.startPolling();
    this.fetchUserName(this.LoggedUserID);
  }

  ngOnDestroy(): void {
    this.stopPolling();
  }

  fetchNotifications() {
    const userId = localStorage.getItem('userId');
    if (!userId) {
      console.error('User ID not found in local storage');
      return;
    }

    this.GlobalService.fetchNotifications(userId).subscribe(
        (notifications) => {
          this.notifications = notifications;
          this.unreadCount = notifications.filter((n) => !n.readMessage).length;
        },
        (error) => {} 
      );
  }

  startPolling() {
    this.pollingInterval = setInterval(() => {
      this.fetchNotifications();
    }, 5000);
  }

  stopPolling() {
    if (this.pollingInterval) {
      clearInterval(this.pollingInterval);
    }
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
    if (this.showNotifications) {
      this.markAllAsRead();
    }
  }

  markAllAsRead() {
    const unreadNotifications = this.notifications.filter((n) => !n.readMessage);
    unreadNotifications.forEach((notification) => {
      this.GlobalService.markNotificationAsRead(notification.notificationID).subscribe(
          () => {
            notification.readMessage = true;
            this.unreadCount = this.notifications.filter((n) => !n.readMessage).length;
          },
          (error) => this.toastr.error('Error marking notification as read', "error")
        );
    });
  }

  acceptInvitation(notificationId: number) {
    this.GlobalService.markNotificationAsRead(notificationId).subscribe(
        async (response) => {
          const notification = this.notifications.find((n) => n.notificationID === notificationId);
          if (notification) {
            const partnerUserName = notification.message.split(" - An invitation for Shared Account Partner from ")[1];         
            if (partnerUserName) {
              const currentUserId = localStorage.getItem('userId');
              if (currentUserId) {
                const partnerUserId = await this.getUserIdByName(partnerUserName);
                if (partnerUserId) {
                  const sharedAccount: SharedAccountsEntity = {
                    userID1: parseInt(currentUserId, 10),
                    userID2: partnerUserId,
                    createdBy: '',
                    modifiedBy: '',
                  };
                  this.GlobalService.createSharedUser (sharedAccount).subscribe(
                      (response:any) => {
                        if(response.status === true) {
                          this.toastr.success(response.data, 'Success'); 
                        }
                        else
                        {
                          this.toastr.error(response.data, 'Error'); 
                        }
                        this.notifications = this.notifications.filter((n) => n.notificationID !== notificationId);
                        this.fetchNotifications();
                      },
                      (error) => {
                        this.toastr.error('Invitation expired or could not create shared account.', 'error'); 
                      }
                    );
                } else {
                  this.toastr.error('Partner user not found.', 'error'); 
                }
              } else {
                this.toastr.error('Current user ID not found.', 'error'); 
              }
            } else {
              this.toastr.error('Invalid notification message format.', 'error'); 
            }
          }
        },
        (error) => {
          this.toastr.error('Error accepting invitation. Please try again.', 'error'); 
        }
      );
  }

  private async getUserIdByName(userName: string): Promise<number> {
    try {
      const trimmedUserName = userName.trim();
      const userDetails = await this.http
        .get<{ userID: number; userName: string }>(
          `https://localhost:7154/Users/GetUserDetails/${encodeURIComponent(trimmedUserName)}`
        )
        .toPromise();
      return userDetails ? userDetails.userID : 0;
    } catch (error) {
      this.toastr.error('Error fetching user details:', "error");
      return 0;
    }
  }

  deleteNotification(notificationId: number) {
    this.GlobalService.deleteNotification(notificationId).subscribe(
        (response) => {
          this.notifications = this.notifications.filter((n) => n.notificationID !== notificationId);
          this.unreadCount = this.notifications.filter((n) => !n.readMessage).length;
        },
        (error) => console.error('Error deleting notification', error)
      );
  }
  async fetchUserName(userId: number): Promise<string | undefined> {
    try {
      const response = await this.http.get<{ userName: string } | undefined>(`https://localhost:7154/Users/GetUserNamesByUserID/${encodeURIComponent(userId)}`).toPromise();
      if (response && response.userName) {
        this.UserName=response.userName;
        this.cdr.detectChanges();
        return response.userName;
      } else {
        console.error('Response is undefined or does not contain userName');
        return undefined;
      }
    } catch (error) {
      console.error('Error fetching user name:', error);
      return undefined;
    }
  }

  logout(): void {
    this.LogOut.emit("Logout");
    this.authService.logout();
    this.router.navigate(['/Login']);
  }

  
}