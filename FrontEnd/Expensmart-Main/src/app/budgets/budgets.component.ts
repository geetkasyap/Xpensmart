import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { UpdateServicesService } from '../services/update-services.service';
import { Router } from '@angular/router';

interface Category {
    categoryID: number,
    name: String,
    isActive: boolean,
    createdOn: Date,
    modifiedOn: Date
}
@Component({
  standalone: false,
  selector: 'app-budgets',
  templateUrl: './budgets.component.html',
  styleUrls: ['./budgets.component.scss']
})
export class BudgetsComponent implements OnInit {
  budgetForm!: FormGroup;
  isLoading = false;
  isMonthly = false;
  isYearly = false;
  months = [
    { name: 'January', value: 1 },
    { name: 'February', value: 2 },
    { name: 'March', value: 3 },
    { name: 'April', value: 4 },
    { name: 'May', value: 5 },
    { name: 'June', value: 6 },
    { name: 'July', value: 7 },
    { name: 'August', value: 8 },
    { name: 'September', value: 9 },
    { name: 'October', value: 10 },
    { name: 'November', value: 11 },
    { name: 'December', value: 12 }
  ];
  years: number[] = [];
  
  constructor(private fb: FormBuilder, private http: HttpClient,
    private toastr: ToastrService,private BudgetUpdateService:UpdateServicesService,
    private router: Router) {
    this.initializeForm();
  }

  ngOnInit(): void {
    const userId = localStorage.getItem('userId') ;
    if (userId ==undefined || null) {
      this.router.navigate(['/Login']);
    }

    this.loadYears();
  }

  private initializeForm(): void {
    this.budgetForm = this.fb.group({
      frequency: ['', Validators.required],
      month: [{ value: '', disabled: true }],
      year: [{ value: '', disabled: true }],
      startDate: [''],
      endDate: [''],
      amount: ['', [Validators.required, Validators.min(0),Validators.max(9999999999999999.99)]]
    });

    this.budgetForm.get('frequency')?.valueChanges.subscribe(value => {
      this.onFrequencyChange();
    });
  }

  loadYears() {
    const currentYear = new Date().getFullYear();
    this.years = Array.from({ length: 6 }, (_, i) => currentYear + i);
  }

  onFrequencyChange() {
    const frequency = this.budgetForm.get('frequency')?.value;
    this.isMonthly = frequency === 'monthly';
    this.isYearly = frequency === 'yearly';
    
    if (this.isMonthly) {
      this.budgetForm.get('month')?.enable();
      this.budgetForm.get('year')?.enable();
      this.budgetForm.get('month')?.setValidators([Validators.required]);
      this.budgetForm.get('year')?.setValidators([Validators.required]);
    } else if (this.isYearly) {
      this.budgetForm.get('month')?.disable();
      this.budgetForm.get('year')?.enable();
      this.budgetForm.get('month')?.clearValidators();
      this.budgetForm.get('year')?.setValidators([Validators.required]);
    } else {
      this.budgetForm.get('month')?.disable();
      this.budgetForm.get('year')?.disable();
      this.budgetForm.get('month')?.clearValidators();
      this.budgetForm.get('year')?.clearValidators();
    }
    
    this.budgetForm.patchValue({
      month: '',
      year: '',
      startDate: '',
      endDate: ''
    });

    this.budgetForm.get('month')?.updateValueAndValidity();
    this.budgetForm.get('year')?.updateValueAndValidity();
  }

  setDates() {
    const frequency = this.budgetForm.get('frequency')?.value;
    const year = this.budgetForm.get('year')?.value;
    
    if (frequency === 'monthly' && year) {
      const month = this.budgetForm.get('month')?.value;
      if (month) {
        const startDate = new Date(year, month - 1, 1);
        startDate.setDate(startDate.getDate() + 1);
        
        const endDate = new Date(year, month, 0); 
        endDate.setDate(endDate.getDate() + 1);
        
        this.budgetForm.patchValue({
          startDate: startDate.toISOString().split('T')[0],
          endDate: endDate.toISOString().split('T')[0]
        });
      }
    } else if (frequency === 'yearly' && year) {
      const startDate = new Date(year, 0, 1);
      startDate.setDate(startDate.getDate() + 1);  
      
      const endDate = new Date(year, 11, 31);
      endDate.setDate(endDate.getDate() + 1);  
  
      this.budgetForm.patchValue({
        startDate: startDate.toISOString().split('T')[0],
        endDate: endDate.toISOString().split('T')[0]
      });
    }
  }
  
  

  isFormValid(): boolean {
    if (!this.budgetForm.valid) return false;
    
    const frequency = this.budgetForm.get('frequency')?.value;
    const amount = this.budgetForm.get('amount')?.value;
    const year = this.budgetForm.get('year')?.value;
    
    if (!frequency || !amount || amount <= 0 || !year) return false;
    
    if (frequency === 'monthly') {
      return !!this.budgetForm.get('month')?.value;
    }
    
    return true;
  }

  onSubmit() {
    if (this.isFormValid()) {
        this.isLoading = true;
        const formValue = this.budgetForm.getRawValue(); 
        if(formValue.frequency === "monthly") {
            formValue.frequency = "Monthly";
        } else if(formValue.frequency === "yearly") {
            formValue.frequency = "Yearly";
        }
        
        const budgetData = {
            frequency: formValue.frequency,
            startDate: formValue.startDate,
            endDate: formValue.endDate,
            amount: formValue.amount,
            createdOn: new Date(),
            modifiedOn: new Date(),
            createdBy: " ",
            modifiedBy: " ",
            userID: localStorage.getItem("userId"),
        };
  
        this.http.post('https://localhost:7154/Budget/AddBudget', budgetData)
            .subscribe({
                next: (response: any) => { 
                    if(response.status==false)
                    {
                      this.toastr.error(response.data, 'Budget Already Exists'); 
                    }
                    else
                    {
                      this.toastr.success(response.data, 'Success'); 
                    }
                    this.budgetForm.reset();
                    this.initializeForm();
                    this.BudgetUpdateService.notifyBudgetAdded();
                },
                error: (error) => {
                    this.toastr.error('Failed to add budget. Please try again.', 'Error'); 
                    this.budgetForm.reset();
                    this.initializeForm();
                    this.BudgetUpdateService.notifyBudgetAdded();
                    this.isLoading = false;
                },
                complete: () => {
                    this.isLoading = false;
                }
            });
    }
}
}

