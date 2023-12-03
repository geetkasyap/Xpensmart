import { GlobalService } from './../services/global.service';
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-group-creation',
  standalone: false,
  templateUrl: './group-creation.component.html',
  styleUrls: ['./group-creation.component.scss']
})
export class GroupCreationComponent implements OnInit {
  groupName: string = '';
  userSearch: string = '';
  selectedUsers: any[] = [];
  filteredUsers: any[] = [];
  users: any[] = [];
  locUserID=localStorage.getItem('userId') as unknown as number;
  constructor(private http: HttpClient,private router: Router,private toastr: ToastrService,private GlobalService: GlobalService) {}

  ngOnInit(): void {
    this.fetchUsers();
  }

  fetchUsers() {
    this.GlobalService.fetchUsers().subscribe(
      (data: any) => {
      this.users = data;
    });
  }

  filterUsers(searchTerm: string) {
    if (searchTerm) {
      this.filteredUsers = this.users.filter(user => 
        user.fullName.toLowerCase().includes(searchTerm.toLowerCase()) &&
        !this.selectedUsers.includes(user)
      );
    } else {
      this.filteredUsers = [];
    }
  }

  selectUser (user: any) {
    this.selectedUsers.push(user);
    this.userSearch = '';
    this.filteredUsers = [];
  }

  removeUser (index: number) {
    this.selectedUsers.splice(index, 1);
  }

  createGroup() {
    if (!this.groupName || this.selectedUsers.length === 0) {
      alert('Please fill in all required fields.');
      return;
    }

    const payload = {
      GroupName: this.groupName,
      UserIds: this.selectedUsers.map(user => user.userID)
    };

    this.http.post(`https://localhost:7154/Groups/CreateGroup/${this.locUserID}`, payload).subscribe(
      (response: any) => {
        if(response.status === true) {
          this.toastr.success(response.data, 'Success');
        }
        else
        {
          this.toastr.error(response.data, 'Error');
        }
        this.resetForm();
      },
      (error) => {
        this.toastr.error('Group not created!', 'error');
      }
    );
  }

  resetForm() {
    this.groupName = '';
    this.selectedUsers = [];
    this.userSearch = '';
    this.filteredUsers = [];
  }
  ViewGroups()
  {
    this.router.navigate(['/ViewGroups']);
  }
}