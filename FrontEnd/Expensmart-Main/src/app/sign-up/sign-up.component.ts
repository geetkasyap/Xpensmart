import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { ValidationsServiceService } from '../shared/validations-service.service';
import { ToastrService } from 'ngx-toastr';

export interface SignUpResponse {
  status: string;
  message?: string;
}

@Component({
  selector: 'app-sign-up',
  standalone: false,
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {
  signupForm: FormGroup;
  isLoading = false;
  showPassword = false;
  showConfirmPassword = false;
  usernameExists = false;
  emailExists = false;
  passwordStrengthValid = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private http: HttpClient,
    private validationService: ValidationsServiceService,
    private toastr:ToastrService
    
  ) {
    this.signupForm = this.fb.group({
      username: ['', [Validators.required,Validators.pattern(/^[a-zA-Z0-9]{3,20}$/)]],
      fullName: ['', [Validators.required,Validators.minLength(3), Validators.pattern(/^[a-zA-Z]+(?: [a-zA-Z]+)*$/), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.pattern(/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/), Validators.maxLength(100)]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(20), this.passwordStrengthValidator.bind(this)]],
      confirmPassword: ['', [Validators.required]]
    }, { validator: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    const userId = localStorage.getItem('userId') ;
    if (userId) {
      this.router.navigate(['/Login']);
    }
    this.signupForm.get('username')?.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap(username => this.checkUsername(username.trim()))
      )
      .subscribe(
        (response: any) => {
          const usernameControl = this.signupForm.get('username');
          this.usernameExists = response.status === false;

            if (this.usernameExists) {
              if(!this.signupForm.get('username')?.errors?.['pattern'])
                {
                  usernameControl?.setErrors({ usernameExists: true });                
                }
                else {}
          } else {
            usernameControl?.setErrors(null); 
          }
        }
      );
  
    this.signupForm.get('email')?.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap(email => {
          const emailControl = this.signupForm.get('email');
          if (emailControl?.valid) {
            return this.checkEmail(email.trim());
          } else {
            return new Promise(resolve => resolve({ status: false }));
          }
        })
      )
      .subscribe(
        (response: any) => {
          const emailControl = this.signupForm.get('email');
          this.emailExists = response.status === false;
  
           if (emailControl?.value.length > 100) {
            emailControl?.setErrors({ maxlength: true });
          } else if (this.emailExists) {
            emailControl?.setErrors({ emailExists: true });
          } else {
            emailControl?.setErrors(null);
          }
        }
      );
  
    this.signupForm.get('password')?.valueChanges.subscribe((value) => {
      const passwordControl = this.signupForm.get('password');
      if (passwordControl?.value.length > 20) {
        passwordControl?.setErrors({ maxlength: true });
      } else {
        passwordControl?.setErrors(null);
      }
      this.passwordStrengthValid = this.isPasswordStrong(value);
      this.signupForm.get('password')?.updateValueAndValidity({ emitEvent: false });
    });
  
    this.signupForm.get('confirmPassword')?.valueChanges.subscribe(() => {
      const confirmPasswordControl = this.signupForm.get('confirmPassword');
      if (confirmPasswordControl?.value.length > 20) {
        confirmPasswordControl?.setErrors({ maxlength: true });
      } else {
        confirmPasswordControl?.setErrors(null);
      }
    });
  }
  
  passwordMatchValidator(g: FormGroup) {
    return g.get('password')?.value === g.get('confirmPassword')?.value
      ? null : { 'passwordMismatch': true };
  }

  checkUsername(username: string) {
    if (!username) return new Promise(resolve => resolve({ status: false }));
    return this.http.get<{ status: string }>(`https://localhost:7154/Users/CheckUserName/${username}`);
  }

  checkEmail(email: string) {
    if (!email) return new Promise(resolve => resolve({ status: false }));
    return this.http.get<{ status: string }>(`https://localhost:7154/Users/CheckEmail/${email}`);
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  onSubmit(): void {
    if (this.signupForm.valid && !this.usernameExists && !this.emailExists) {
      this.isLoading = true;

      const userData = {
        userName: this.signupForm.value.username.trim(),
        email: this.signupForm.value.email.trim(),
        password: this.signupForm.value.password.trim(),
        fullName: this.signupForm.value.fullName.trim(),
        isActive: true,
        createdOn: new Date(),
        modifiedOn: new Date()
      };

      this.http.post('https://localhost:7154/Users/CreateUser ', userData)
        .subscribe(
          (response: any) => {
            if (response.status===true) {
              this.toastr.success(response.data,"Success");
              this.router.navigate(['/Login']);
            } else {
              this.toastr.error(response.data,"Error");
            }
          },
          (error) => {
            this.router.navigate(['/Error']);
          }
        )
        .add(() => {
          this.isLoading = false;
        });
    }
  }

  passwordStrengthValidator(control: any) {
    const value = control.value;
    if (value && !this.isPasswordStrong(value)) {
      return { passwordStrength: true };
    }
    return null;
  }

  isPasswordStrong(password: string): boolean {
    const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z0-9]{8,}$/;
    return regex.test(password);
  }

  getErrorMessage(controlName: string) {
    return this.validationService.validateControl(this.signupForm, controlName);
  }

}