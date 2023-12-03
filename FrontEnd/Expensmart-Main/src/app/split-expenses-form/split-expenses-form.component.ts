import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient, HttpParams } from '@angular/common/http';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

interface Category {
  categoryID: number;
  name: string;
}
interface ExpenseSplitRequest
{
  totalAmount:number;
  numberOfMembers:number;
  userNames:string[];
}

@Component({
  selector: 'app-split-expenses-form',
  standalone: false,
  templateUrl: './split-expenses-form.component.html',
  styleUrl: './split-expenses-form.component.scss'
})
export class SplitExpensesFormComponent implements OnInit {
  expenseForm: FormGroup;
  categories: Category[] = [];
  filteredUsers: string[] = [];
  selectedUsers: string[] = [];
  loggedInUser: string = '';
  isSubmitting = false;
  selectedFile: File | null = null;
  
  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private toastr: ToastrService,
    private router: Router
  ) {
    this.expenseForm = this.fb.group({
      totalAmount: ['', [Validators.required, Validators.min(0.02),Validators.max(9999999999999999.99)]],
      numberOfMembers: ['', [Validators.required, Validators.min(2)]],
      categoryId: ['', Validators.required],
      userSearch: [''],
      receipt: ['']
    });

    this.expenseForm.get('numberOfMembers')?.valueChanges.subscribe(value => {
      if (value < this.selectedUsers.length) {
        this.selectedUsers = this.selectedUsers.slice(0, value);
      }
    });

    this.expenseForm.get('userSearch')?.valueChanges.pipe(
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
      this.router.navigate(['/Login']);
    }
    this.loadCategories();

    this.http.get<string>(`https://localhost:7154/Users/GetUserNamesByUserID/${Number(localStorage.getItem('userId'))}`)
    .subscribe(
      (user:any) => {
      this.loggedInUser = user.userName;
      this.selectedUsers.push(this.loggedInUser);
    });
  }

  loadCategories() {
    this.http.get<Category[]>('https://localhost:7154/GetCategories')
      .subscribe(categories => this.categories = categories);
  }

  filterUsers(searchTerm: string) {
    this.http.get<string[]>('https://localhost:7154/Users/GetUserDetails/AllNames')
      .subscribe(users => {
        this.filteredUsers = users.filter(user => 
          user.toLowerCase().includes(searchTerm.toLowerCase()) &&
          !this.selectedUsers.includes(user) &&
          user !== this.loggedInUser
        );
      });
  }

  selectUser(user: string) {
    const maxMembers = this.expenseForm.get('numberOfMembers')?.value || 0;
    if (this.selectedUsers.length < maxMembers) {
      this.selectedUsers.push(user);
      this.expenseForm.get('userSearch')?.setValue('');
      this.filteredUsers = [];
    }
  }

  removeUser(index: number) {
    this.selectedUsers.splice(index, 1);
  }

  onFileSelected(event: Event) {
    const target = event.target as HTMLInputElement;
    this.selectedFile = target.files?.[0] || null;
  }

  async uploadFile(): Promise<string> {
    if (!this.selectedFile) return '';
    
    const formData = new FormData();
    formData.append('receipt', this.selectedFile);
    
    try {
      const response = await this.http.post<{ filePath: string }>(
        'http://localhost:8080/uploads',
        formData
      ).toPromise();
      return response?.filePath || '';
    } catch (error) {
      console.error('Error uploading file:', error);
      return '';
    }
  }
async onSubmit() {
  if (this.expenseForm.invalid || this.selectedUsers.length === 0) return;

  const numberOfMembers = this.expenseForm.get('numberOfMembers')?.value;
  if (this.selectedUsers.length !== numberOfMembers) {
      alert('Please select the correct number of users');
      return;
  }

  this.isSubmitting = true;
  try {
      const fileUrl = await this.uploadFile();
      const totalAmount = this.expenseForm.get('totalAmount')?.value; 
      const userId = localStorage.getItem('userId');
      const categoryId = this.expenseForm.get('categoryId')?.value;

      const splitRequest: ExpenseSplitRequest = {
          totalAmount: totalAmount,
          numberOfMembers: numberOfMembers,
          userNames: this.selectedUsers
      };

      const params = new HttpParams()
          .set('userId', userId || '')
          .set('categoryID', categoryId)
          .set('FileUrl', fileUrl);

          await this.http.post('https://localhost:7154/Expenses/split-expense', splitRequest,
            { params: params })
          .subscribe({
              next: (response: any) => { 
                if(response.status===true)
                {
                  this.toastr.success(response.data, 'Success'); 
                }
                else
                {
                  this.toastr.error(response.data, 'Error'); 
                }
                  this.resetForm();
                  this.isSubmitting = false;
              },
              error: (error) => {
                  console.error('Error adding budget:', error);
                  this.toastr.error('Failed to split expense. Please try again.', 'Error'); 
              },
          });
  } catch (error) {
      console.error('Error splitting expense:', error);
      alert('Error splitting expense. Please try again.');
  } finally {
      this.isSubmitting = false;
  }
}
  private resetForm() {
    this.expenseForm.reset();
    this.selectedUsers = [this.loggedInUser];
    this.selectedFile = null;
    const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
    if (fileInput) fileInput.value = '';
  }
}
