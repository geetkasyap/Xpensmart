import { GlobalService } from './../services/global.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
interface Notification {
  userID: number;
  type: 'invitation' | 'expense' | 'shared_account';
  message: string;
}

@Component({
  selector: 'app-shared-account-form',
  standalone: false,
  templateUrl: './shared-account-form.component.html',
  styleUrls: ['./shared-account-form.component.scss']
})
export class SharedAccountFormComponent implements OnInit {
  sharedAccountForm: FormGroup;
  filteredUsers: string[] = [];
  selectedUsers: string[] = [];
  isSubmitting = false;

  constructor(private fb: FormBuilder, private http: HttpClient,private Router:Router,
        private toastr: ToastrService,private GlobalService: GlobalService
  ) {
    this.sharedAccountForm = this.fb.group({
      userSearch: [''],
      notificationMessage: ['', Validators.required]
    });

    this.sharedAccountForm.get('userSearch')?.valueChanges.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(searchTerm => {
      if (searchTerm) {
        this.filterUsers(searchTerm);
      } else {
        this.filteredUsers = [];
      }
    });
  }

  ngOnInit() {
    const userId = localStorage.getItem('userId') ;
    if (userId ==undefined || null) {
      this.Router.navigate(['/Login']);
    }

    this.GetPresentUserName();
  }
  SharedExpenseReports()
  {
    this.Router.navigate(['/SharedExpenseReports']);
  }
  SharedExpenseTable()
  {
    this.Router.navigate(['/SharedExpenseTable']);
  }
  AddSharedExpense()
  {
    this.Router.navigate(['/AddSharedExpenses']);
  }
  filterUsers(searchTerm: string) {
    this.GlobalService.getUserDetailsAllNames().subscribe(users => {
        this.filteredUsers = users.filter(user => 
          user.toLowerCase().includes(searchTerm.toLowerCase()) &&
          !this.selectedUsers.includes(user)
        );
      });
  }

  selectUser (user: string) {
    if (this.selectedUsers.length === 0) { 
      this.selectedUsers.push(user);
      this.sharedAccountForm.get('userSearch')?.setValue('');
      this.filteredUsers = [];
    }
  }

  removeUser (index: number) {
    this.selectedUsers.splice(index, 1);
  }

  async onSubmit() {
    if (this.sharedAccountForm.invalid || this.selectedUsers.length === 0) return;
  
    this.isSubmitting = true;
    try {
      const notificationMessage = this.sharedAccountForm.get('notificationMessage')?.value;
  
      const userId = await this.getUserIdByName(this.selectedUsers[0]);
        const presentUserName = await this.GetPresentUserName();
      const notification: Notification = {
        userID: userId,
        type: 'invitation',
        message: `${notificationMessage} - An invitation for Shared Account Partner from ${presentUserName}`,
      };
      await this.sendNotification(notification);
  
      this.toastr.success('Shared user Request Sent! Ask them to accept the request', 'Success');
      this.resetForm();
    } catch (error) {
      console.error('Error sending invitation:', error);
      this.toastr.error('Expense not added successfully!', 'error');
    } finally {
      this.isSubmitting = false;
    }
  }
  private async GetPresentUserName(): Promise<string | null> {
    try {
      const userId = localStorage.getItem('userId');
      if (!userId) {
        console.error('User  ID not found in local storage');
        return null;
      }
      const response = await this.http.get<{ userName: string } | undefined>(`https://localhost:7154/Users/GetUserNamesByUserID/${encodeURIComponent(userId)}`).toPromise();     
       if (response && response.userName) {
        return response.userName;
      } else {
        console.error('Response is undefined or does not contain userName');
        return null;
      }
    } catch (error) {
      console.error('Error fetching user name:', error);
      return null;
    }
  }


  private async sendNotification(notification: Notification) {
    return this.http.post('https://localhost:7154/Notification/AddNotifications', notification).toPromise();

  }
  private async getUserIdByName(userName: string): Promise<number> {
    try {
      const trimmedUserName = userName.trim();
        const userDetails = await this.http.get<{ userID: number, userName: string }>(`https://localhost:7154/Users/GetUserDetails/${encodeURIComponent(trimmedUserName)}`).toPromise();
        return userDetails ? userDetails.userID : 0;
    } catch (error) {
      console.error('Error fetching user details:', error);
      return 0;
    }
  }

  private resetForm() {
    this.sharedAccountForm.reset();
    this.selectedUsers = [];
    this.filteredUsers = [];
  }
  
}