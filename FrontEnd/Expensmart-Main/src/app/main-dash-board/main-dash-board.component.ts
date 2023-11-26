import { ExpenseService } from './../services/expense.service';
import { Component,OnInit} from '@angular/core';
import { CommonModule } from '@angular/common';
import { trigger, transition, style, animate, state } from '@angular/animations';
import { ConsizedExpense } from '../models/expense-entity-consizedModel';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
@Component({
  selector: 'app-main-dash-board',
  standalone: false,
  
  templateUrl: './main-dash-board.component.html',
  styleUrl: './main-dash-board.component.scss',
  animations: [
    trigger('slideIn', [
      transition(':enter', [
        style({ transform: 'translateX(-100%)', opacity: 0 }),
        animate('500ms ease-out', style({ transform: 'translateX(0)', opacity: 1 }))
      ])
    ]),
    trigger('expandVertical', [
      state('void', style({ height: '0', opacity: 0 })),
      transition(':enter', [
        animate('300ms ease-out', style({ height: '*', opacity: 1 }))
      ])
    ])
  ]
})

export class MainDashBoardComponent implements OnInit {
  expenses: ConsizedExpense[] = [];
  currentMonth: Date = new Date();
  weeks: any[] = [];
  selectedWeek: any = null;
  weeklyExpenses: ConsizedExpense[] = [];
  isLoading: boolean = true;

  constructor(private ExpenseService: ExpenseService,private Router: Router,private toastr:ToastrService) { }

  ngOnInit() {
    if(localStorage.getItem('userId')==null)
    {
      this.Router.navigate(['/Login']);
      this.toastr.error('You have been Logged Out', 'Login Again');

    }
    this.loadExpenses();
    this.generateWeeks();
    const userId = localStorage.getItem('userId');
  }

  loadExpenses() {
    this.isLoading = true;
    this.ExpenseService.getExpensesByDateString().subscribe(data => {
      this.expenses = data;
      this.organizeExpensesByWeek();
      this.isLoading = false;
    });
  }

  generateWeeks() {
    const firstDay = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth(), 1);
    const lastDay = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth() + 1, 0);
    
    let currentDate = firstDay;
    let weekNumber = 1;
    
    while (currentDate <= lastDay) {
      const week = {
        number: weekNumber,
        startDate: new Date(currentDate),
        endDate: new Date(currentDate.setDate(currentDate.getDate() + 6)),
        expenses: []
      };
      
      this.weeks.push(week);
      currentDate.setDate(currentDate.getDate() + 1);
      weekNumber++;
    }
  }

  organizeExpensesByWeek() {
    this.weeks.forEach(week => {
      week.expenses = this.expenses.filter(expense => {
        const expenseDate = new Date(expense.date);
        return expenseDate >= week.startDate && expenseDate <= week.endDate;
      });
    });
  }

  selectWeek(week: any) {
    this.selectedWeek = this.selectedWeek === week ? null : week;
    if (this.selectedWeek) {
      this.weeklyExpenses = week.expenses.sort((a: ConsizedExpense, b: ConsizedExpense) => 
        new Date(a.date).getTime() - new Date(b.date).getTime()
      );
    }
  }

  navigateMonth(direction: number) {
    this.currentMonth = new Date(this.currentMonth.setMonth(this.currentMonth.getMonth() + direction));
    this.weeks = [];
    this.generateWeeks();
    this.organizeExpensesByWeek();
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric'
    });
  }

  formatAmount(amount: number): string {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR'
    }).format(amount);
  }
}