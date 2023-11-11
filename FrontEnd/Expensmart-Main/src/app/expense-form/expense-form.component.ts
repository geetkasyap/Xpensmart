import { GlobalService } from './../services/global.service';
import { ActivatedRoute } from '@angular/router';
import { Component, ElementRef, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ViewChild } from '@angular/core';
import { Observable, of } from 'rxjs';
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
  selector: 'app-expense-form',
  standalone: false,
  templateUrl: './expense-form.component.html',
  styleUrls: ['./expense-form.component.scss']
})
export class ExpenseFormComponent implements OnInit {
  @Input() user1Id: number | undefined;
  @Input() user2Id: number | undefined;
  @Output() expenseAdded = new EventEmitter<void>();

  @ViewChild('fileInput') myInputVariable!: ElementRef;

  fileFormatError:boolean = false;
  expenseForm: FormGroup;
  categories: Category[] = [];
  isLoading = false;
  selectedCurrency = 'INR';
  originalAmount = 0;
  convertedAmount=0;
  locUserID = Number(localStorage.getItem("userId"));
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
    private router: Router,
    private GlobalService: GlobalService,
  ) {
    this.expenseForm = this.fb.group({
      amount: ['', [Validators.required, Validators.min(0),Validators.max(9999999999999.99)]],
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
    const userId = localStorage.getItem('userId') ;
    if (userId ==undefined || null) {
      this.router.navigate(['/Login']);
    }
    this.route.params.subscribe(params => {
      this.user1Id = +params['user1Id'];
      this.user2Id = +params['user2Id'];
    });

    this.loadCategories();
    this.loadCategoryKeywords();
  }
  getCategoryKeywords(): Observable<any[]> {
    return this.GlobalService.getCategoryKeywords();
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
        error => this.toastr.error('Error loading category keywords:', "error")
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

        const amountInOriginalCurrency = this.expenseForm.get('amount')?.value;

        this.convertAmountToSelectedCurrency(amountInOriginalCurrency).subscribe(
            convertedAmount => {
                const expenseData = {
                    ...this.expenseForm.value,
                    amount: convertedAmount,
                    userID: this.locUserID,
                    isActive: true,
                    createdOn: new Date(),
                    modifiedOn: new Date(),
                    createdBy: localStorage.getItem("userId"),
                    modifiedBy: localStorage.getItem("userId"),
                };

                    this.GlobalService.ExpenseFormAddExpense(expenseData)
                    .subscribe(
                        (response: any) => {
                            if (response.status === true) {
                                this.toastr.success(response.data, 'Success');
                            } else {
                                this.toastr.error(response.data, 'Error');
                            }
                            this.resetForm();
                        },
                        (error) => {
                            this.toastr.error('Error adding expense:', "error");
                            this.toastr.error('Expense not added successfully!', 'Error');
                        }
                    )
                    .add(() => {
                        this.isLoading = false;
                    });
                this.expenseAdded.emit();
            },
            error => {
                this.toastr.error('Error converting currency:', "error");
                this.isLoading = false;
            }
        );
    }
}
  
  resetForm() {
    this.expenseForm.reset({
      amount: '',
      date: new Date(),
      categoryID: '',
      receiptUrl: '',
      description: '',
      expenseType: 'One-time',
      isRecurring: false,
      userID: this.locUserID
    });
    this.selectedCurrency = 'INR'; 
    this.originalAmount = 0; 
    this.myInputVariable.nativeElement.value = "";
  }
  selectCurrency() {
        return this.currencies.find(c => c.code === this.selectedCurrency)?.symbol;
      }
    
      onFileSelected(event: Event) {
        const target = event.target as HTMLInputElement;
        const file = target.files ? target.files[0] : null;
    
        if (file) {
            const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'application/pdf', 'application/zip'];
            const maxSizeInBytes = 100 * 1024 * 1024;
    
            if (!allowedTypes.includes(file.type)) {
                this.toastr.error('Invalid file type. Only images, PDFs, and ZIP files are allowed.', 'Error');
                target.value = '';
                return;
            }
    
            if (file.size > maxSizeInBytes) {
                this.toastr.error('File size exceeds the maximum limit of 100 MB.', 'Error');
                target.value = '';
                return;
            }

            const formData = new FormData();
            formData.append('receipt', file);
    
              this.GlobalService.uploadFile(formData).subscribe(response => {
                this.expenseForm.patchValue({ receiptUrl: response.filePath });
            }, error => {
                this.toastr.error('Error uploading file', "error");
            });
        }
         else 
         {
            this.toastr.error('No file selected');
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
      getDateErrorMessage(): string {
        const control = this.expenseForm.get('date');
        if (control?.touched || control?.dirty) {
          if (control?.hasError('required')) {
            return 'Date is required';
          }
        }
        return '';
      }
      convertAmountToSelectedCurrency(amount: number): Observable<number> {
        if (this.selectedCurrency === 'INR') {
            return of(amount); 
        } else {
          return this.GlobalService.convertCurrency(amount, 'INR', this.selectedCurrency)
        }
      }
}