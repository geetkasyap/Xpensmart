import { GlobalService } from './../services/global.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { UpdateServicesService } from '../services/update-services.service';
import { Router } from '@angular/router';

interface Category {
  categoryID: number;
  name: string;
  isActive: boolean;
  createdOn: Date;
  modifiedOn: Date;
}
@Component({
  selector: 'app-add-recurring-expense',
  standalone: false,
  
  templateUrl: './add-recurring-expense.component.html',
  styleUrl: './add-recurring-expense.component.scss'
})


export class AddRecurringExpenseComponent implements OnInit {
  recurringExpenseForm: FormGroup;
  categories: Category[] = [];
  isLoading = false;
  selectedCurrency = 'INR';
  originalAmount = 0;
  years: number[] = [];
  locUserID = localStorage.getItem("userId");
  currencies = [
    { code: 'INR', symbol: '₹' },
    { code: 'USD', symbol: '$' },
    { code: 'EUR', symbol: '€' },
    { code: 'GBP', symbol: '£' }
  ];
  currentYear: number = new Date().getFullYear();

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private toastr: ToastrService,
    private updateService:UpdateServicesService,
    private router: Router,
    private GlobalService:GlobalService,
  ) {
    this.recurringExpenseForm = this.fb.group({
      amount: ['', [Validators.required, Validators.min(0),Validators.max(9999999999999999.99)]],
      frequency: ['', Validators.required],
      fromDate: ['', Validators.required],
      toDate: ['', Validators.required],
      fromYear: ['', Validators.required],
      toYear: ['', Validators.required],
      categoryID: ['', Validators.required],
      receiptUrl: ['']
    }, { validators: this.dateValidator });

    this.recurringExpenseForm.get('frequency')?.valueChanges.subscribe(frequency => {
      if (frequency === 'Monthly Recurring') {
        this.recurringExpenseForm.get('fromYear')?.clearValidators();
        this.recurringExpenseForm.get('toYear')?.clearValidators();
        this.recurringExpenseForm.get('fromDate')?.setValidators([Validators.required]);
        this.recurringExpenseForm.get('toDate')?.setValidators([Validators.required]);
      } else if (frequency === 'Yearly Recurring') {
        this.recurringExpenseForm.get('fromDate')?.clearValidators();
        this.recurringExpenseForm.get('toDate')?.clearValidators();
        this.recurringExpenseForm.get('fromYear')?.setValidators([Validators.required]);
        this.recurringExpenseForm.get('toYear')?.setValidators([Validators.required]);
      }
      this.recurringExpenseForm.get('fromDate')?.updateValueAndValidity();
      this.recurringExpenseForm.get('toDate')?.updateValueAndValidity();
      this.recurringExpenseForm.get('fromYear')?.updateValueAndValidity();
      this.recurringExpenseForm.get('toYear')?.updateValueAndValidity();
    });
  }

  ngOnInit(): void {
    const userId = localStorage.getItem('userId') ;
    if (userId ==undefined || null) {
      this.router.navigate(['/Login']);
    }
    this.loadCategories();
    this.generateYears();
    this.recurringExpenseForm.get('amount')?.valueChanges
      .subscribe(amount => {
        if (amount && this.selectedCurrency !== 'INR') {
          this.convertToINR(amount);
        } else {
          this.originalAmount = amount;
        }
      });
  }

  loadCategories() {
    this.GlobalService.loadCategories()
      .subscribe(
        data => {
          this.categories = data;
        },
        error =>this.toastr.error('Error loading categories:', "error")
      );
  }
  generateYears() {
    const startYear = 2020; 
    const endYear = this.currentYear + 6; 
    for (let year = startYear; year <= endYear; year++) {
        this.years.push(year);
    }
}
  changeCurrency(currency: string) {
    if (currency === this.selectedCurrency) return;

    const currentAmount = this.recurringExpenseForm.get('amount')?.value;
    if (currentAmount) {
      if (currency === 'INR') {
        this.recurringExpenseForm.patchValue({ amount: this.originalAmount });
      } else {
        this.convertCurrency(currentAmount, 'INR', currency);
      }
    }
    this.selectedCurrency = currency;
  }

  convertToINR(amount: number) {
    this.GlobalService.convertCurrency(amount, this.selectedCurrency, 'INR')
      .subscribe(
        convertedAmount => {
          this.originalAmount = convertedAmount;
        },
        error => this.toastr.error('Error converting currency:', "error")
      );
  }

  convertCurrency(amount: number, fromCurrency: string, toCurrency: string) {
    this.GlobalService.convertCurrency(amount, fromCurrency, toCurrency)
      .subscribe(
        convertedAmount => {
          this.recurringExpenseForm.patchValue({ amount: convertedAmount });
        },
        error => this.toastr.error('Error converting currency:', "error")
      );
  }
onSubmit() {
    if (this.recurringExpenseForm.valid) {
      this.isLoading = true;

      const formData = this.recurringExpenseForm.value;
      let fromDateString: string;
        let toDateString: string;

        if (formData.frequency === 'Monthly Recurring') {
            fromDateString = formData.fromDate;
            toDateString = formData.toDate;
        } else {
            fromDateString = `${formData.fromYear}-01-01`;
            toDateString = `${formData.toYear}-12-31`;
        }
      const params = new HttpParams()
          .set('FromDate', fromDateString)
          .set('ToDate', toDateString)
          .set('ExpenseType', formData.frequency)
          .set('CategoryID', formData.categoryID.toString())
          .set('UserID', this.locUserID!)
          .set('Amount', this.originalAmount.toString())
          .set('RecieptUrl', formData.receiptUrl || 'null');

          this.GlobalService.addRecurringTransaction(params)
          .subscribe(
              (response:any) => {
                  if (response.status===true) {
                      this.recurringExpenseForm.reset();
                      this.toastr.success(response.data, 'Success');
                      this.updateService.notifyExpenseAdded();
                  }
                  else
                  {
                    this.recurringExpenseForm.reset();
                      this.toastr.error(response.data, 'Error');
                      this.updateService.notifyExpenseAdded();
                  }
              },
              (error) => {
                  this.toastr.error('AddTransactions not added successfully!', 'Error');
              }
          )
          .add(() => {
              this.isLoading = false;
          });
      }

}
  selectCurrency() {
    return this.currencies.find(c => c.code === this.selectedCurrency)?.symbol;
  }

  onFileSelected(event: Event) {
    const target = event.target as HTMLInputElement;
    const file = target.files ? target.files[0] : null;

    if (file) {
      const formData = new FormData();
      formData.append('receipt', file);

      this.GlobalService.uploadFile(formData).subscribe(response => {
        this.recurringExpenseForm.patchValue({ receiptUrl: response.filePath });
      }, error => {
        this.toastr.error('Error uploading file', "error");
      });
    } else {
      this.toastr.error('No file selected');
    }
  }

  dateValidator(form: FormGroup) {
    const frequency = form.get('frequency')?.value;
    const fromDate = form.get('fromDate')?.value;
    const toDate = form.get('toDate')?.value;
    const fromYear = form.get('fromYear')?.value;
    const toYear = form.get('toYear')?.value;

    if (frequency === 'Monthly Recurring') {
      if (!fromDate || !toDate) {
        return { dateInvalid: true };
      }
      const from = new Date(fromDate);
      const to = new Date(toDate);
      if (to <= from || to.getMonth() <= from.getMonth()) {
        return { dateInvalid: true };
      }
    } else if (frequency === 'Yearly Recurring') {
      if (!fromYear || !toYear) {
        return { dateInvalid: true };
      }
      if (toYear <= fromYear) {
        return { dateInvalid: true };
      }
    }
    if (frequency === 'Yearly Recurring') {
      if (!fromYear || !toYear) {
          return { dateInvalid: true };
      }
      if (toYear <= fromYear) {
          return { dateInvalid: true };
      }
  }

    return null;
  }
}