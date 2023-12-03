import { ConfirmationDialogComponent } from './../confirmation-dialog/confirmation-dialog.component';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ExpenseService } from '../services/expense.service';
import { ExpensesEntity } from '../models/expenses-entity-model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { HttpClient } from '@angular/common/http';
import { CategoryEntity } from '../models/category-entity-model';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { Toast, ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-shared-account-table-structure',
  standalone: false,
  
  templateUrl: './shared-account-table-structure.component.html',
  styleUrl: './shared-account-table-structure.component.scss'
})
export class SharedAccountTableStructureComponent implements OnInit{
  displayedColumns: string[] = ['categoryIcon', 'date', 'amount', 'categoryName', 'description', 'expenseType', 'receipt', 'actions'];
  categories: CategoryEntity[] = [];
  filterTags: string[] = ['Personal', 'Business', 'Recurring', 'One-time'];
  selectedFilters: Set<string> = new Set();
  editingExpense: ExpensesEntity | null = null;
  editForm: FormGroup;
  searchTerm: Subject<string> = new Subject<string>();
  
  @Input() userId: number | undefined;
  @Input() dataSource: MatTableDataSource<ExpensesEntity>;
  @Output() refreshData = new EventEmitter<void>();
  @Output() userUpdated = new EventEmitter<void>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    public expenseService: ExpenseService,
    public fb: FormBuilder,
    public dialog: MatDialog,
    public http: HttpClient,
    public toastr: ToastrService,

  ) {
    this.createEditForm();
    this.dataSource = new MatTableDataSource<ExpensesEntity>();
    this.editForm = this.createEditForm();
  }

  ngOnInit(): void {
    if (this.dataSource) {
            this.dataSource.paginator = this.paginator;
            this.dataSource.sort = this.sort;
          }
    this.searchTerm.pipe(
      debounceTime(300),
      distinctUntilChanged()
    ).subscribe(searchValue => {
      this.dataSource.filter = searchValue.trim().toLowerCase();
    });
    this.loadExpenses();
    this.loadCategories();
    this.setupSearchFilter();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  private createEditForm(): FormGroup {
    return this.fb.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: ['', Validators.required],
      categoryID: ['', Validators.required],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      expenseType: ['', Validators.required],
      receiptUrl: ['']
    });
  }

  loadExpenses() {
    this.expenseService.getExpenses().subscribe(
      expenses => {
        this.dataSource.data = expenses;
      },
      error => console.error('Error loading expenses:', error)
    );
  }

  loadCategories() {
    this.http.get<CategoryEntity[]>('https://localhost:7154/GetCategories').subscribe(
      categories => {
        this.categories = categories;
      },
      error => console.error('Error loading categories:', error)
    );
  }

  setupSearchFilter() {
    this.dataSource.filterPredicate = (data: ExpensesEntity, filter: string) => {
      const searchStr = filter.toLowerCase();
      const matchesSearch = data.description.toLowerCase().includes(searchStr) ||
                          data.expenseType.toLowerCase().includes(searchStr) ||
                          this.getCategoryName(data.categoryID).toLowerCase().includes(searchStr);
      
      if (this.selectedFilters.size === 0) return matchesSearch;
      
      return matchesSearch && this.selectedFilters.has(data.expenseType);
    };
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  toggleFilter(filter: string) {
    if (this.selectedFilters.has(filter)) {
      this.selectedFilters.delete(filter);
    } else {
      this.selectedFilters.add(filter);
    }
    this.dataSource.filter = this.dataSource.filter;
  }

  getCategoryName(categoryId: number): string {
    return this.categories.find(c => c.categoryID === categoryId)?.name || '';
  }

  getCategoryIcon(categoryId: number): string {
    const iconMap: { [key: number]: string } = {
      1: 'home',                
      2: 'restaurant',          
      3: 'medical_services',    
      4: 'school',              
      5: 'flight',             
      6: 'theaters',            
      7: 'spa',                 
      8: 'pets',                
      9: 'card_giftcard',       
      10: 'monetization_on', 
      11: 'credit_card',        
      12: 'more_horiz'          
  };
  
  return iconMap[categoryId] || 'category';
  }

  startEdit(expense: ExpensesEntity) {
    this.editingExpense = expense;
    if (!this.editForm) {
      this.createEditForm();
    }
    this.editForm.patchValue({
      amount: expense.amount,
      date: expense.date,
      categoryID: expense.categoryID,
      receiptUrl: expense.receiptUrl || '',
      description: expense.description || '',
      expenseType: expense.expenseType || '',
      userID: Number(localStorage.getItem('userId'))
    });
  }

  cancelEdit() {
    this.editingExpense = null;
    this.editForm.reset();
  }
  saveEdit() {
    if (this.editForm.valid && this.editingExpense) {
      const formDate = new Date(this.editForm.value.date);
      formDate.setDate(formDate.getDate() + 1);
      const utcDate = new Date(Date.UTC(formDate.getUTCFullYear(), formDate.getUTCMonth(), formDate.getUTCDate()));
        const updatedExpense: ExpensesEntity = {
            ...this.editingExpense,
            ...this.editForm.value,
            modifiedOn: new Date(),
            modifiedBy: '', 
            userID: Number(localStorage.getItem('userId')),
            date: utcDate 
        };

        this.expenseService.updateExpense(updatedExpense).subscribe(
            (response:any) => {
              if(response.status===true)
              {
                this.toastr.success(response.data, 'Success'); 
                this.userUpdated.emit();
                this.refreshData.emit();
              }
              else
              {
                this.toastr.error(response.data, 'Error'); 
              }
                this.loadExpenses();
                this.editingExpense = null;
                this.editForm.reset();
            },

            error =>{
               console.error('Error updating expense:', error);
            this.toastr.error('Expense not Updated successfully!', 'Error');
            }
        );
    }
}
  deleteExpense(expenseId: number) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '300px',
      data: { message: 'Are you sure you want to delete this expense?' }
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.expenseService.deleteExpense(expenseId).subscribe({
          next: (response: any) => { 
            if(response.status===true)
            {
              this.toastr.success(response.data, 'Success');
            }
            else
            {
              this.toastr.error(response.data, 'Error');
            }
            this.loadExpenses();
          },
          error: (error) => {
            console.error('Error deleting expense:', error);
            this.toastr.error('Expense not Deleted successfully!', 'Error');
          },
        });
      }
    });
  }

  onFileSelected(event: Event) {
    const target = event.target as HTMLInputElement;
    const file = target.files ? target.files[0] : null;
  
    if (file) {
      const formData = new FormData();
      formData.append('receipt', file);
  
      this.http.post<{ filePath: string }>('http://localhost:8080/uploads', formData).subscribe(
        response => {
          this.editForm.patchValue({ receiptUrl: response.filePath });
        }, 
        error => {
          console.error('Error uploading file', error);
        }
      );
    }
  }
  openReceipt(receiptUrl: string) {
    if (receiptUrl) {
      window.open(receiptUrl, '_blank');
    }
  }


}
