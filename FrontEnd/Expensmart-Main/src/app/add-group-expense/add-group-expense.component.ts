import { GlobalService } from './../services/global.service';
import { ActivatedRoute } from '@angular/router';
import { Component, ElementRef, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
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
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
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
  selector: 'app-add-group-expense',
  standalone: false,
  
  templateUrl: './add-group-expense.component.html',
  styleUrl: './add-group-expense.component.scss'
})
export class AddGroupExpenseComponent implements OnInit {
 @Input() user1Id: number | undefined;
  @Input() user2Id: number | undefined;
  @Input() groupID: number | undefined; 
  @Output() expenseAdded = new EventEmitter<void>();

  @ViewChild('fileInput') myInputVariable!: ElementRef;

  expenseForm: FormGroup;
  categories: Category[] = [];
  isLoading = false;
  selectedCurrency = 'INR';
  originalAmount = 0;
  locUserID = localStorage.getItem("userId") as unknown as number;
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
    private route: ActivatedRoute,
    private GlobalService: GlobalService, 
  ) {
    this.expenseForm = this.fb.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: [new Date(), Validators.required],
      categoryID: ['', Validators.required],
      receiptUrl: [''],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      expenseType: ['One-time', Validators.required],
      isRecurring: [false],
      userID: [this.locUserID, Validators.required]
    });

    
    this.expenseForm.get('description')?.valueChanges
      .pipe(debounceTime(500), distinctUntilChanged())
      .subscribe(description => {
        this.autoSelectCategory(description);
      });
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.user1Id = +params['user1Id'];
      this.user2Id = +params['user2Id'];
    });

    this.loadCategories();
    this.loadCategoryKeywords();
    this.expenseForm.get('amount')?.valueChanges
      .pipe(debounceTime(500), distinctUntilChanged())
      .subscribe(amount => {
        if (amount && this.selectedCurrency !== 'INR') {
          this.convertToINR(amount);
        } else {
          this.originalAmount = amount;
        }
      });
  }
  getCategoryKeywords(): Observable<any[]> {
    return this.GlobalService.getCategoryKeywords();  }
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
        error => this.toastr.error('Error loading category keywords:', 'error')
      );
  }

  loadCategories() {
    this.GlobalService.loadCategories()
      .subscribe(
        data => {
          this.categories = data;
        },
        error => this.toastr.error('Error loading categories:', "error")
      );
  }
  changeCurrency(currency: string) {
    if (currency === this.selectedCurrency) return;

    const currentAmount = this.expenseForm.get('amount')?.value;
    if (currentAmount) {
      if (currency === 'INR') {
        this.expenseForm.patchValue({ amount: this.originalAmount });
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
          this.expenseForm.patchValue({ amount: convertedAmount });
        },
        error => this.toastr.error('Error converting currency:', "error")
      );
  }
  onSubmit() {
    if (this.expenseForm.valid) {
      this.isLoading = true;

      const expenseData = {
        ...this.expenseForm.value,
        amount: this.originalAmount,
        userID: this.locUserID,
        isActive: true,
        createdOn: new Date(),
        modifiedOn: new Date(),
        createdBy: localStorage.getItem("userId"),
        modifiedBy: localStorage.getItem("userId"),
      };
      const obj = {
        groupID : this.groupID,
        expense :expenseData
      }
      this.GlobalService.addGroupExpense(this.locUserID, this.groupID!, expenseData)
      .subscribe(
          (response:any) => {
            if (response && response.status===true) {
              this.expenseForm.reset();
              this.myInputVariable.nativeElement.value = "";
              this.toastr.success(response.data,'Success');
            }
            else
            {
              this.toastr.error(response.data || 'An unexpected error occurred.', 'Error'); 
              this.expenseForm.reset();
              this.myInputVariable.nativeElement.value = "";
            }
          },
          (error) => {
            this.toastr.error(error.message, 'Error');
            this.expenseForm.reset();
              this.myInputVariable.nativeElement.value = "";
          }
    )
        .add(() => {
          this.isLoading = false;
        });
      this.expenseAdded.emit();
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
            this.expenseForm.patchValue({ receiptUrl: response.filePath });
          }, error => {
            console.error('Error uploading file', error);
          });
        } else {
          console.error('No file selected');
        }
      }
      autoSelectCategory(description: string) {
        for (const [category, keywords] of Object.entries(this.categoryKeywords)) {
          if (keywords.some(keyword => description.toLowerCase().includes(keyword))) {
            const categoryID = this.categories.find(cat => cat.name === category)?.categoryID;
            if (categoryID) {
              this.expenseForm.patchValue({ categoryID });
              break;
            }
          }
        }
      }
}
