import { GlobalService } from './../services/global.service';
import { Component, OnInit, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import {ChangeDetectionStrategy, inject} from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import {FormControl, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {FloatLabelType, MatFormFieldModule} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInputModule} from '@angular/material/input';
import {MatRadioModule} from '@angular/material/radio';
import {MatSelectModule} from '@angular/material/select';
import {map} from 'rxjs/operators';
import {provideNativeDateAdapter} from '@angular/material/core';
import {MatDatepickerModule} from '@angular/material/datepicker';
import { DatePipe } from '@angular/common';
import { AbstractControl} from '@angular/forms';
import{NgToastModule} from 'ng-angular-popup';
import { ToastrService } from 'ngx-toastr';
import { CommonModule } from '@angular/common';
import { ViewChild } from '@angular/core';
import { Observable } from 'rxjs';

interface UserWithUserName {
  userID: number;
  userName: string;
}

interface Category {
  categoryID: number;
  name: string;
  isActive: boolean;
  createdOn: Date;
  modifiedOn: Date;
}
interface CategoryKeywordResponse {
  categoryKeywordID: number;
  categoryID: number;
  keyword: string;
  categoriesEntity: {
    categoryID: number;
    name: string;
    isActive: boolean;
    createdOn: Date;
    modifiedOn: Date;
    expensesEntities: any;
    categoryKeywordsEntities: any;
  };
}

@Component({
  selector: 'app-shared-account-expenses',
  standalone: false,
  
  templateUrl: './shared-account-expenses.component.html',
  styleUrl: './shared-account-expenses.component.scss'
})
export class SharedAccountExpensesComponent implements OnInit{
  @ViewChildren('fileInput') fileInputs!: QueryList<ElementRef>;
  users: any[] = [];
  expenseForm1: FormGroup;
  expenseForm2: FormGroup;
  categories: any[] = [];
  isLoading = false;
  selectedCurrency1 = 'INR';
  selectedCurrency2 = 'INR';
  originalAmount1 = 0;
  originalAmount2 = 0;
  userId1 = localStorage.getItem("userId");
  userId2?: number;
  selectedUser:string='';
  currencies = [
    { code: 'INR', symbol: '₹' },
    { code: 'USD', symbol: '$' },
    { code: 'EUR', symbol: '€' },
    { code: 'GBP', symbol: '£' }
  ];

  expenseTypes = [
    'One-time',
    'UpComing',
    'Business',
    'Personal',
    'Credited',
    'Need To Be Paid'
  ];

  categoryKeywords: { [categoryName: string]: string[] } = {};

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private toastr: ToastrService,
    public GlobalService: GlobalService,
  ) {
    this.expenseForm1 = this.createForm();
    this.expenseForm2 = this.createForm();
  }

  loadUsers() {
    this.GlobalService.getSharedUserDetails(this.userId1).subscribe(users => {
      this.users = users;
    });
  }
  onUserChange() {
    this.userId2 = Number(this.selectedUser);
    this.createForm();
  }

  private createForm(): FormGroup {
    return this.fb.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: [new Date(), Validators.required],
      categoryID: ['', Validators.required],
      receiptUrl: [''],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      expenseType: ['One-time', Validators.required],
      isRecurring: [false]
    });
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadCategories();
    this.loadCategoryKeywords();
    
    this.setupAmountListener(this.expenseForm1, 1);
    this.setupAmountListener(this.expenseForm2, 2);
  }

  private setupAmountListener(form: FormGroup, userNum: number) {
    form.get('amount')?.valueChanges
      .pipe(debounceTime(500), distinctUntilChanged())
      .subscribe(amount => {
        const currency = userNum === 1 ? this.selectedCurrency1 : this.selectedCurrency2;
        if (amount && currency !== 'INR') {
          this.convertToINR(amount, userNum);
        } else {
          if (userNum === 1) this.originalAmount1 = amount;
          else this.originalAmount2 = amount;
        }
      });
  }

  selectCurrency(userNum: number) {
    const currency = userNum === 1 ? this.selectedCurrency1 : this.selectedCurrency2;
    return this.currencies.find(c => c.code === currency)?.symbol;
  }

  changeCurrency(currency: string, userNum: number) {
    const currentCurrency = userNum === 1 ? this.selectedCurrency1 : this.selectedCurrency2;
    const form = userNum === 1 ? this.expenseForm1 : this.expenseForm2;
    
    if (currency === currentCurrency) return;

    const currentAmount = form.get('amount')?.value;
    if (currentAmount) {
      if (currency === 'INR') {
        form.patchValue({ amount: userNum === 1 ? this.originalAmount1 : this.originalAmount2 });
      } else {
        this.convertCurrency(currentAmount, 'INR', currency, userNum);
      }
    }
    
    if (userNum === 1) this.selectedCurrency1 = currency;
    else this.selectedCurrency2 = currency;
  }

  formReset(userNum:number) {
    const form = userNum === 1 ? this.expenseForm1 : this.expenseForm2;
    form.reset({
      amount: '',
      date: new Date(),
      categoryID: '',
      receiptUrl: '',
      description: '',
      expenseType: 'One-time',
      isRecurring: false,
    });
    this.selectedCurrency1 = 'INR'; 
    this.originalAmount1 = 0; 
  }
  
  onSubmit(userNum: number) {
    const form = userNum === 1 ? this.expenseForm1 : this.expenseForm2;
    const userId = userNum === 1 ? this.userId1 : this.userId2;
    const originalAmount = userNum === 1 ? this.originalAmount1 : this.originalAmount2;

    if((userId==this.userId2) && (userId==0 || null || undefined))
    {
      this.toastr.error("First select a shared user");
    }else{
      if (form.valid) {
        this.isLoading = true;
  
        const expenseData = {
          ...form.value,
          amount: originalAmount,
          userID: userId,
          isActive: true,
          createdOn: new Date(),
          modifiedOn: new Date(),
          createdBy: '',
          modifiedBy: '',
        };
  
        this.GlobalService.addExpense(expenseData).subscribe(
          (response:any) => {
              if (response.status===true) {
                this.toastr.success(response.data, 'Success');
                this.toastr.success(`Expense added for User ${userNum}!`, 'Success');
              }
              else
              {
                this.toastr.error(response.data, 'Error');
              }
              form.reset();
              this.formReset(userNum);
              this.fileInputs.toArray()[userNum - 1].nativeElement.value = "";
                
            },
            error => {
              console.error('Error adding expense:', error);
              this.toastr.error(`Expense not added successfully for User ${userNum}!`, 'Error');
            }
          )
          .add(() => {
            this.isLoading = false;
          });
    }

    }
  }

  loadCategories() {
    this.GlobalService.loadCategories().subscribe(
        data => {
          this.categories = data;
        },
        error => console.error('Error loading categories:', error)
      );
  }

  convertToINR(amount: number, userNum: number) {
    if(userNum==1)
    {
      this.GlobalService.convertCurrency(amount, this.selectedCurrency1, 'INR')
      .subscribe(
        convertedAmount => {
          this.originalAmount1 = convertedAmount;
        },
        error => console.error('Error converting currency:', error)
      );
    }
    else
    {
      this.GlobalService.convertCurrency(amount, this.selectedCurrency2, 'INR')
      .subscribe(
        convertedAmount => {
          this.originalAmount2 = convertedAmount;
        },
        error => console.error('Error converting currency:', error)
      );
    }

  }
  convertCurrency(amount: number, fromCurrency: string, toCurrency: string, userNum: number) {
    if(userNum==1)
    {
      this.GlobalService.convertCurrency(amount, fromCurrency, toCurrency)
      .subscribe(
        convertedAmount => {
          this.expenseForm1.patchValue({ amount: convertedAmount });
        },
        error => console.error('Error converting currency:', error)
      );
    }
    else
    {
      this.GlobalService.convertCurrency(amount, fromCurrency, toCurrency)
      .subscribe(
        convertedAmount => {
          this.expenseForm2.patchValue({ amount: convertedAmount });
        },
        error => console.error('Error converting currency:', error)
      );
    }

  }
  loadCategoryKeywords() {
    this.GlobalService.getCategoryKeywords()
      .subscribe(
        data => {
          this.categoryKeywords = data.reduce((acc, item) => {
            const categoryName = item.categoriesEntity.name;
            const keyword = item.keyword;
  
            if (!acc[categoryName]) {
              acc[categoryName] = [];
            }
            acc[categoryName].push(keyword);
  
            return acc;
          }, {} as { [categoryName: string]: string[] });
        },
        error => console.error('Error loading category keywords:', error)
      );
  }
  onFileSelected1(event: Event) {
    const target = event.target as HTMLInputElement;
    const file = target.files ? target.files[0] : null;

    if (file) {
      const formData = new FormData();
      formData.append('receipt', file);

      this.GlobalService.uploadFile(formData).subscribe(response => {
        this.expenseForm1.patchValue({ receiptUrl: response.filePath });
      }, error => {
        console.error('Error uploading file', error);
      });
    } else {
      console.error('No file selected');
    }
  }
  onFileSelected2(event: Event) {
    const target = event.target as HTMLInputElement;
    const file = target.files ? target.files[0] : null;

    if (file) {
      const formData = new FormData();
      formData.append('receipt', file);

      this.GlobalService.uploadFile(formData).subscribe(response => {
        this.expenseForm2.patchValue({ receiptUrl: response.filePath });
      }, error => {
        console.error('Error uploading file', error);
      });
    } else {
      console.error('No file selected');
    }
  }
}