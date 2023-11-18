import { GlobalService } from './../../services/global.service';
import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { BudgetsEntity } from '../../models/budget-entity-model';
import { ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationDialogComponent } from '../../confirmation-dialog/confirmation-dialog.component';
import { UpdateServicesService } from '../../services/update-services.service';
import { Router } from '@angular/router';

@Component({
  standalone: false,
  selector: 'app-budgets-table',
  templateUrl: './budgets-table.component.html',
  styleUrls: ['./budgets-table.component.scss']
})
export class BudgetsTableComponent implements OnInit {
  userID=localStorage.getItem("userId");
  dataSource: MatTableDataSource<BudgetsEntity>;
  displayedColumns: string[] = ['startDate', 'endDate', 'frequency', 'amount', 'actions'];
  editingBudget: BudgetsEntity | null = null;
    editForm: FormGroup;

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

  constructor(private http: HttpClient, private router: Router,private GlobalService: GlobalService,
    private fb: FormBuilder,private toastr: ToastrService,
    private dialog: MatDialog,private BudgetUpdateService:UpdateServicesService) {
    this.dataSource = new MatTableDataSource<BudgetsEntity>();
    this.editForm = this.createEditForm();
  }

  ngOnInit(): void {
    this.loadBudgets();
    this.BudgetUpdateService.budgetAdded$.subscribe(()=>
      {
        this.loadBudgets();
      });
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

    private createEditForm(): FormGroup {
      return this.fb.group({
        amount: ['', [Validators.required, Validators.min(0)]],

      });
    }

  loadBudgets() {
    this.GlobalService.getBudgetsByUser(this.userID!)
      .subscribe(
        budgets => {
          this.dataSource.data = budgets;
        },
        error => this.toastr.error('Error loading budgets:', "error")
      );
  }


  startEdit(budget: BudgetsEntity) {
    this.editingBudget = budget;
    this.editForm.patchValue({
      amount: budget.amount,
    });
  }

  cancelEdit() {
    this.editingBudget = null;
    this.editForm.reset();
  }

  saveEdit() {
    if (this.editForm.valid && this.editingBudget) {
      const budgetID = this.editingBudget.budgetID;
      const amount = this.editForm.get('amount')?.value;
  
      this.GlobalService.updateBudget(budgetID, amount)
        .subscribe(
          (response:any) => {
            if(response.status===true)
            {
              this.toastr.success(response.data, 'Success');
            }
            else
            {
              this.toastr.error(response.data, 'Error');
            }
            this.loadBudgets(); 
            this.editingBudget = null; 
            this.editForm.reset(); 
          },
          (error) => {
            this.toastr.error('Failed to update budget. Please try again.', 'Error');
          }
        );
    }
  }
  
  deleteBudget(budgetID: number) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '300px',
      data: { message: 'Are you sure you want to delete this budget?' }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.GlobalService.deleteBudget(budgetID)
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

          this.ngOnInit();
        },
        error: (error) => {
                    this.toastr.error('Budget not Deleted successfully!', 'Error');
        },
      });
    }
  });
}
}