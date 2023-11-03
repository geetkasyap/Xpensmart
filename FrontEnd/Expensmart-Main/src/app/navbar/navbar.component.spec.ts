// import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
// import { NavbarComponent } from './navbar.component';
// import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
// import { Router } from '@angular/router';
// import { ToastrService } from 'ngx-toastr';
// import { AuthService } from '../services/auth.service';
// import { GlobalService } from '../services/global.service';
// import { ChangeDetectorRef } from '@angular/core';
// import { of, throwError } from 'rxjs';
// import { MatIconModule } from '@angular/material/icon';

// fdescribe('NavbarComponent', () => {
//   let component: NavbarComponent;
//   let fixture: ComponentFixture<NavbarComponent>;
//   let httpMock: HttpTestingController;
//   let router: jasmine.SpyObj<Router>;
//   let toastrService: jasmine.SpyObj<ToastrService>;
//   let authService: jasmine.SpyObj<AuthService>;
//   let globalService: jasmine.SpyObj<GlobalService>;
//   let cdr: jasmine.SpyObj<ChangeDetectorRef>;

//   const mockNotifications: any[] = [
//     {
//       notificationID: 1,
//       type: 'invitation',
//       message: 'Invitation - An invitation for Shared Account Partner from JaneDoe',
//       sender: 'JaneDoe',
//       timestamp: new Date(),
//       readMessage: false
//     },
//     {
//       notificationID: 2,
//       type: 'expense',
//       message: 'Expense added',
//       sender: 'JohnDoe',
//       timestamp: new Date(),
//       readMessage: true
//     }
//   ];

//   beforeEach(async () => {
//     router = jasmine.createSpyObj('Router', ['navigate']);
//     toastrService = jasmine.createSpyObj('ToastrService', ['success', 'error']);
//     authService = jasmine.createSpyObj('AuthService', ['logout']);
//     globalService = jasmine.createSpyObj('GlobalService', [
//       'fetchNotifications',
//       'markNotificationAsRead',
//       'createSharedUser',
//       'deleteNotification'
//     ]);
//     cdr = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);

//     await TestBed.configureTestingModule({
//       imports: [HttpClientTestingModule, MatIconModule],
//       declarations: [NavbarComponent],
//       providers: [
//         { provide: Router, useValue: router },
//         { provide: ToastrService, useValue: toastrService },
//         { provide: AuthService, useValue: authService },
//         { provide: GlobalService, useValue: globalService },
//         { provide: ChangeDetectorRef, useValue: cdr }
//       ]
//     }).compileComponents();

//     fixture = TestBed.createComponent(NavbarComponent);
//     component = fixture.componentInstance;
//     httpMock = TestBed.inject(HttpTestingController);

//     // Mock localStorage
//     spyOn(localStorage, 'getItem').and.returnValue('123');

//     // Mock GlobalService responses
//     globalService.fetchNotifications.and.returnValue(of(mockNotifications));
//     globalService.markNotificationAsRead.and.returnValue(of({}));
//     globalService.createSharedUser.and.returnValue(of({ status: true, data: 'Shared account created' }));
//     globalService.deleteNotification.and.returnValue(of({}));

//     // Trigger ngOnInit
//     fixture.detectChanges();

//     // Handle the default fetchUserName request in ngOnInit
//     const req = httpMock.expectOne('https://localhost:7154/Users/GetUserNamesByUserID/123');
//     req.flush({ userName: 'JohnDoe' });
//   });

//   afterEach(() => {
//     httpMock.verify();
//   });

//   it('should create', () => {
//     expect(component).toBeTruthy();
//   });

//   it('should initialize with userId and fetch notifications and username on ngOnInit', () => {
//     expect(component.LoggedUserID).toBe(123);
//     expect(globalService.fetchNotifications).toHaveBeenCalledWith('123');
//     expect(component.notifications).toEqual(mockNotifications);
//     expect(component.unreadCount).toBe(1);
//     expect(component.UserName).toBe('JohnDoe');
//   });

//   it('should start polling on ngOnInit', fakeAsync(() => {
//     spyOn(localStorage, 'getItem').and.returnValue('123'); // Reset mock
//     component.ngOnInit();
//     tick(5000);
//     expect(globalService.fetchNotifications).toHaveBeenCalledTimes(2); // Initial + 1 polling call
//     const req = httpMock.expectOne('https://localhost:7154/Users/GetUserNamesByUserID/123');
//     req.flush({ userName: 'JohnDoe' });
//   }));

//   it('should stop polling on ngOnDestroy', () => {
//     spyOn(window, 'clearInterval');
//     component.ngOnInit(); // Start polling to set interval
//     const req = httpMock.expectOne('https://localhost:7154/Users/GetUserNamesByUserID/123');
//     req.flush({ userName: 'JohnDoe' });
//     component.ngOnDestroy();
//     expect(window.clearInterval).toHaveBeenCalled();
//   });

//   it('should not fetch notifications if userId is not present', fakeAsync(() => {
//     spyOn(localStorage, 'getItem').and.returnValue(null);
//     spyOn(console, 'error');
//     component.fetchNotifications();
//     expect(console.error).toHaveBeenCalledWith('User ID not found in local storage');
//     expect(globalService.fetchNotifications).not.toHaveBeenCalled();

//     // Handle fetchUserName request with null userId from previous ngOnInit
//     const req = httpMock.expectOne('https://localhost:7154/Users/GetUserNamesByUserID/null');
//     req.flush(null);
//   }));

//   it('should handle fetchNotifications error', () => {
//     globalService.fetchNotifications.and.returnValue(throwError(() => new Error('Fetch error')));
//     component.fetchNotifications();
//     expect(component.notifications).toEqual([]);
//     expect(component.unreadCount).toBe(0);
//   });

//   it('should toggle notifications and mark all as read', () => {
//     component.toggleNotifications();
//     expect(component.showNotifications).toBeTrue();
//     expect(globalService.markNotificationAsRead).toHaveBeenCalledWith(1);
//     expect(component.unreadCount).toBe(0);
//   });

//   it('should mark all unread notifications as read', () => {
//     component.markAllAsRead();
//     expect(globalService.markNotificationAsRead).toHaveBeenCalledWith(1);
//     expect(component.notifications[0].readMessage).toBeTrue();
//     expect(component.unreadCount).toBe(0);
//   });

//   it('should handle error marking notification as read', () => {
//     globalService.markNotificationAsRead.and.returnValue(throwError(() => new Error('Mark error')));
//     component.markAllAsRead();
//     expect(toastrService.error).toHaveBeenCalledWith('Error marking notification as read', 'error');
//   });

//   it('should accept invitation successfully', fakeAsync(() => {
//     component.acceptInvitation(1);
//     const req = httpMock.expectOne('https://localhost:7154/Users/GetUserDetails/JaneDoe');
//     req.flush({ userID: 456, userName: 'JaneDoe' });
//     tick();

//     expect(globalService.markNotificationAsRead).toHaveBeenCalledWith(1);
//     expect(globalService.createSharedUser).toHaveBeenCalledWith({
//       userID1: 123,
//       userID2: 456,
//       createdBy: '',
//       modifiedBy: ''
//     });
//     expect(toastrService.success).toHaveBeenCalledWith('Shared account created', 'Success');
//     expect(component.notifications.length).toBe(1);
//   }));

//   it('should handle invalid notification message format in acceptInvitation', () => {
//     component.notifications[0].message = 'Invalid message';
//     component.acceptInvitation(1);
//     expect(toastrService.error).toHaveBeenCalledWith('Invalid notification message format.', 'error');
//   });

//   it('should handle partner user not found in acceptInvitation', fakeAsync(() => {
//     spyOn(localStorage, 'getItem').and.returnValue('123'); // Ensure consistent userId
//     component.acceptInvitation(1);
//     const reqDetails = httpMock.expectOne('https://localhost:7154/Users/GetUserDetails/JaneDoe');
//     reqDetails.flush(null); // Simulate partner not found
//     tick();
//     expect(toastrService.error).toHaveBeenCalledWith('Partner user not found.', 'error');
//   }));

//   it('should handle error accepting invitation', () => {
//     globalService.markNotificationAsRead.and.returnValue(throwError(() => new Error('Accept error')));
//     component.acceptInvitation(1);
//     expect(toastrService.error).toHaveBeenCalledWith('Error accepting invitation. Please try again.', 'error');
//   });

//   it('should handle error creating shared account in acceptInvitation', fakeAsync(() => {
//     globalService.createSharedUser.and.returnValue(throwError(() => new Error('Create error')));
//     component.acceptInvitation(1);
//     const req = httpMock.expectOne('https://localhost:7154/Users/GetUserDetails/JaneDoe');
//     req.flush({ userID: 456, userName: 'JaneDoe' });
//     tick();
//     expect(toastrService.error).toHaveBeenCalledWith('Invitation expired or could not create shared account.', 'error');
//   }));

//   it('should fetch user ID by name successfully', fakeAsync(() => {
//     component['getUserIdByName']('JaneDoe').then(id => {
//       expect(id).toBe(456);
//     });
//     const req = httpMock.expectOne('https://localhost:7154/Users/GetUserDetails/JaneDoe');
//     req.flush({ userID: 456, userName: 'JaneDoe' });
//     tick();
//   }));

//   it('should handle error fetching user ID by name', fakeAsync(() => {
//     component['getUserIdByName']('JaneDoe').then(id => {
//       expect(id).toBe(0);
//     });
//     const req = httpMock.expectOne('https://localhost:7154/Users/GetUserDetails/JaneDoe');
//     req.error(new ErrorEvent('Fetch error'));
//     tick();
//     expect(toastrService.error).toHaveBeenCalledWith('Error fetching user details:', 'error');
//   }));

//   it('should delete notification', () => {
//     component.deleteNotification(2);
//     expect(globalService.deleteNotification).toHaveBeenCalledWith(2);
//     expect(component.notifications.length).toBe(1);
//     expect(component.unreadCount).toBe(1);
//   });

//   it('should handle error deleting notification', () => {
//     spyOn(console, 'error');
//     globalService.deleteNotification.and.returnValue(throwError(() => new Error('Delete error')));
//     component.deleteNotification(2);
//     expect(console.error).toHaveBeenCalledWith('Error deleting notification', jasmine.any(Error));
//   });

//   it('should fetch username successfully', fakeAsync(() => {
//     component.fetchUserName(123).then(username => {
//       expect(username).toBe('JohnDoe');
//       expect(component.UserName).toBe('JohnDoe');
//       expect(cdr.detectChanges).toHaveBeenCalled();
//     });
//     const req = httpMock.expectOne('https://localhost:7154/Users/GetUserNamesByUserID/123');
//     req.flush({ userName: 'JohnDoe' });
//     tick();
//   }));

//   it('should handle error fetching username', fakeAsync(() => {
//     spyOn(console, 'error');
//     component.fetchUserName(123).then(username => {
//       expect(username).toBeUndefined();
//     });
//     const req = httpMock.expectOne('https://localhost:7154/Users/GetUserNamesByUserID/123');
//     req.error(new ErrorEvent('Fetch error'));
//     tick();
//     expect(console.error).toHaveBeenCalledWith('Error fetching user name:', jasmine.any(Error));
//   }));

//   it('should handle null response when fetching username', fakeAsync(() => {
//     spyOn(console, 'error');
//     component.fetchUserName(123).then(username => {
//       expect(username).toBeUndefined();
//     });
//     const req = httpMock.expectOne('https://localhost:7154/Users/GetUserNamesByUserID/123');
//     req.flush(null);
//     tick();
//     expect(console.error).toHaveBeenCalledWith('Response is undefined or does not contain userName');
//   }));

//   it('should logout', () => {
//     component.logout();
//     expect(component.LogOut.emit).toHaveBeenCalledWith('Logout');
//     expect(authService.logout).toHaveBeenCalled();
//     expect(router.navigate).toHaveBeenCalledWith(['/Login']);
//   });
// });