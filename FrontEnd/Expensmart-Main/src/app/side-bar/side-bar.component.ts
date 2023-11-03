import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-side-bar',
  standalone: false,
  templateUrl: './side-bar.component.html',
  styleUrls: ['./side-bar.component.scss']
})
export class SideBarComponent {
  menuItems = [
    { name: 'Dashboard', icon: 'dashboard', route: '/Main' },
    { name: 'Add Expenses', icon: 'note_add', route: '/ExpenseForm' },
    { name: 'View Expenses', icon: 'view_comfy', route: '/ExpensesTable' },
    { name: 'Budgets', icon: 'monetization_on', route: '/Budgets' },
    { name: 'Shared', icon: 'group', route: '/SharedAcountForm' },
    { name: 'Split Expenses', icon: 'call_split', route: '/SplitExpenses' },
    { name: 'Add Recurring', icon: 'autorenew', route: '/AddRecurringExpense' },
    { name: 'Groups', icon: 'diversity_3', route: '/GroupCreation' },
    { name: 'Projection', icon: 'trending_up', route: '/ExpenseProjection' },
    { name: 'Reports', icon: 'assessment', route: '/Report' },
  ];

  tooltipVisible = false;
  tooltipItem: string | null = null;

  constructor(private router: Router) {}

  showTooltip(name: string) {
    this.tooltipVisible = true;
    this.tooltipItem = name;
  }

  hideTooltip() {
    this.tooltipVisible = false;
    this.tooltipItem = null;
  }

  navigate(route: string) {
    this.router.navigate([route]);
  }
}
