import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from './../services/user.service';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-login-page',
  standalone: false,
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss']
})
export class LoginPageComponent implements OnInit {
  loginForm: FormGroup;
  forgotPasswordForm: FormGroup;
  isLoading = false;
  isForgotPasswordLoading = false;
  showPassword = false;
  showForgotPassword = false;
  errorMessage: string | null = null;
  forgotPasswordError: string | null = null;
  retrievedPassword: string | null = null;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private UserService:UserService,
    private toastr:ToastrService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });

    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.pattern(/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/)]]
    });

  }

  ngOnInit(): void {
    const userId = localStorage.getItem('userId') ;
    if (userId) {
      this.router.navigate(['/Main']);
    }
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = null;

      const { email, password } = this.loginForm.value;

      this.UserService.LoginCheck(email, password).subscribe(
        (response:any) => {
          if (response && response.userID) {
            localStorage.setItem('userId', response.userID);
            this.isLoading = false;
            this.toastr.success('You have successfully logged In');
            this.router.navigate(['/Main']);
          } else {
            this.isLoading = false;
            this.toastr.error('UserID is missing', 'Login Again');
            this.errorMessage = 'UserID is missing in the response.';
          }
        },
        (error) => {
          this.isLoading = false;
          this.toastr.error('Invalid email or password. Please try again.', 'Login Again');
          this.errorMessage = 'Invalid email or password. Please try again.';
        }
      );
        } else {
      Object.keys(this.loginForm.controls).forEach(key => {
        const control = this.loginForm.get(key);
        if (control?.invalid) {
          control.markAsTouched();
        }
      });
    }
  }

  logout(): void {
    localStorage.clear();
    this.router.navigate(['/login']);
    this.toastr.success('You have been Logged Out','SignIn Again!!');
  }

  

  onForgotPasswordSubmit(): void {
    if (this.forgotPasswordForm.valid) {
      this.isForgotPasswordLoading = true;
      this.forgotPasswordError = null;

      const email = this.forgotPasswordForm.value.email;

      this.UserService.ForgotPassword(email).subscribe(
        (response: any) => {
          if (response.status) {
            this.retrievedPassword = response.data;
            this.toastr.success('Password retrieved successfully');
          } else {
            this.toastr.error(response.data, 'Error');
            this.forgotPasswordError = response.data;
          }
          this.isForgotPasswordLoading = false;
        },
        (error) => {
          this.toastr.error('Failed to retrieve password. Please try again.', 'Error');
          this.forgotPasswordError = 'Failed to retrieve password. Please try again.';
          this.isForgotPasswordLoading = false;
        }
      );
    } else {
      Object.keys(this.forgotPasswordForm.controls).forEach(key => {
        const control = this.forgotPasswordForm.get(key);
        if (control?.invalid) {
          control.markAsTouched();
        }
      });
    }
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  copyPassword(): void {
    if (this.retrievedPassword) {
      navigator.clipboard.writeText(this.retrievedPassword).then(() => {
        this.toastr.success('Password copied to clipboard');
      });
    }
  }

getErrorMessage(controlName: string): string {
  const control = this.loginForm.get(controlName);
  if (control?.touched || control?.dirty) { 
    if (control?.hasError('required')) {
      return `${controlName.charAt(0).toUpperCase() + controlName.slice(1)} is required`;
    }
    if (control?.hasError('email')) {
      return 'Please enter a valid email address';
    }
    if (control?.hasError('minlength')) {
      return 'Password must be at least 8 characters long';
    }
  }
  return '';
}
getForgotPasswordErrorMessage(): string {
  const control = this.forgotPasswordForm.get('email');
  if (control?.touched || control?.dirty) {  
    if (control?.hasError('required')) {
      return 'Email is required';
    }
    if (control?.hasError('pattern')) {
      return 'Please enter a valid email address';
    }
  }
  return '';
}
}
