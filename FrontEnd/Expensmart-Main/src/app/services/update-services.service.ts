import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class UpdateServicesService {
  constructor() { }
  private budgetAddedSource = new Subject<void>();
  private expenseAddedSource = new Subject<void>();
  budgetAdded$ = this.budgetAddedSource.asObservable();
  expenseAdded$ = this.expenseAddedSource.asObservable();

  notifyBudgetAdded() {
    this.budgetAddedSource.next();
  }
  notifyExpenseAdded() {
    this.expenseAddedSource.next();
  }

}
