import { Component, OnInit, OnDestroy, ViewChild, ElementRef, ChangeDetectorRef } from '@angular/core';
import { ExpenseService } from '../services/expense.service';
import { Chart } from 'chart.js/auto';
import { Subscription } from 'rxjs';
import { MatSelect } from '@angular/material/select';
import { AuthService } from '../services/auth.service';
import { ToastrService } from 'ngx-toastr';

interface ExpenseData {
  expenseID: number;
  amount: number;
  date: string;
  categoryID: number;
  description: string;
}

interface CategoryData {
  categoryID: number;
  name: string;
}

@Component({
  selector: 'app-dash-board-chart',
  standalone: false,
  templateUrl: './dash-board-chart.component.html',
  styleUrls: ['./dash-board-chart.component.scss']
})
export class DashBoardChartComponent implements OnInit, OnDestroy {
  @ViewChild('expenseCanvas') expenseCanvas!: ElementRef;
  @ViewChild('monthSelect') monthSelect!: MatSelect;

  private chart: Chart | undefined;
  private subscriptions: Subscription[] = [];
  expenses: ExpenseData[] = [];
  categories: CategoryData[] = [];
  selectedMonth: number = new Date().getMonth();
  UserID=localStorage.getItem('userId');
  months = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
  ];
  categoryColors: string[] = ['#6C63FF', '#44006b', '#4CAF50', '#ff4d4d', '#b3b3b3'];
  hasData: boolean = false;
  isLoading: boolean = true;

  constructor(private expenseService: ExpenseService,private authService: AuthService,private cdr:ChangeDetectorRef,private toastr: ToastrService,) 
  {
    this.authService.userLoggedIn$.subscribe(isLoggedIn => {
      if (isLoggedIn) {
          this.loadData();
      } else {
          this.resetChartData();
      }
  });
  }
  resetChartData() {
    this.expenses = [];
    this.categories = [];
    this.hasData = false;
    this.isLoading = true;
}

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
    if (this.chart) {
      this.chart.destroy();
    }
  }

  private loadData(): void {
    this.isLoading = true;
    this.expenses = []; 
    this.categories = []; 
    if (this.UserID!=null || 0) {
      const expensesSub = this.expenseService.getExpensesByDateString().subscribe(
        expenses => {
          this.expenses = expenses;
          this.loadCategories();
        },
        error => this.toastr.error('Error loading expenses:', "error")
      );
      this.subscriptions.push(expensesSub);
      this.isLoading = false;
      this.cdr.detectChanges();
    }
    else {
      this.isLoading = false; 
    }
  }

  private loadCategories(): void {
    const categoriesSub = this.expenseService.getCategories().subscribe(
      categories => {
        this.categories = categories;
        this.processDataAndCreateChart();
      },
      error => this.toastr.error('Error loading categories:', "error")
    );
    this.subscriptions.push(categoriesSub);
  }


  onMonthChange(event: any): void {
    const selectedMonth = event.target.value;  
    if (selectedMonth === undefined || isNaN(selectedMonth)) {
      this.toastr.error('Invalid month selected:', selectedMonth); 
        return;
    }

    this.selectedMonth = +selectedMonth;
    if (this.chart) {
        this.chart.destroy();
        this.chart = undefined;
    }

    this.hasData = false;
    this.processDataAndCreateChart();
}
  private processDataAndCreateChart(): void {
    if (!this.expenses.length || !this.categories.length) {
        this.hasData = false;
        return;
    }

    this.hasData = true;

    const categoryMap = new Map(this.categories.map(cat => [cat.categoryID, cat.name]));

    const currentYear = new Date().getFullYear();

    const monthlyExpenses = this.expenses
        .filter(exp => {
            const expenseDate = new Date(exp.date);
            return expenseDate.getMonth() === this.selectedMonth && expenseDate.getFullYear() === currentYear; 
        })
        .map(exp => ({
            exp,
            CategoryName: categoryMap.get(exp.categoryID)
        }));

    if (!monthlyExpenses.length) {
        this.hasData = false;
        return;
    }

    const dailyData = new Map<number, any>();
    const daysInMonth = new Date(currentYear, this.selectedMonth + 1, 0).getDate(); 

    for (let day = 1; day <= daysInMonth; day++) {
        const dayData: any = { day };
        this.categories.forEach(cat => {
            dayData[cat.name] = 0;
        });
        dailyData.set(day, dayData);
    }

    monthlyExpenses.forEach(expense => {
        const expenseDate = new Date(expense.exp.date);
        const day = expenseDate.getDate();
        const dayData = dailyData.get(day);

        if (dayData && expense.CategoryName) {
            dayData[expense.CategoryName] += expense.exp.amount;
        }
    });

    const chartData = Array.from(dailyData.values()).sort((a, b) => a.day - b.day);

    this.createChart(chartData);
}
  private createChart(data: any[]): void {
    if (this.chart) {
      this.chart.destroy();
    }
  
    const ctx = this.expenseCanvas.nativeElement.getContext('2d');
  
    if (data.length === 0) {
      this.hasData = false; 
      return; 
    }
  
    this.hasData = true;
  
    this.chart = new Chart(ctx, {
      type: 'line',
      data: {
        labels: data.map(d => d.day),
        datasets: this.categories.map((category, index) => ({
          label: category.name,
          data: data.map(d => d[category.name]),
          borderColor: this.categoryColors[index % this.categoryColors.length],
          backgroundColor: this.categoryColors[index % this.categoryColors.length] + '33',
          tension: 0.4,
          fill: true
        }))
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'top',
            labels: {
              color: '#ffffff'
            }
          },
          tooltip: {
            mode: 'index',
            intersect: false,
            backgroundColor: '#1a1a1a',
            borderColor: '#6C63FF',
            borderWidth: 1,
            titleColor: '#ffffff',
            bodyColor: '#ffffff',
            callbacks: {
              label: function (context) {
                return ` ${context.dataset.label}: ₹${context.parsed.y.toFixed(2)}`;
              }
            }
          }
        },
        scales: {
          x: {
            title: {
              display: true,
              text: 'Day of Month',
              color: '#b3b3b3'
            },
            grid: {
              color: '#2d2d2d'
            },
            ticks: {
              color: '#b3b3b3'
            }
          },
          y: {
            title: {
              display: true,
              text: 'Amount (₹)',
              color: '#b3b3b3'
            },
            grid: {
              color: '#2d2d2d'
            },
            ticks: {
              color: '#b3b3b3',
              callback: function (value) {
                return '₹' + value;
              }
            }
          }
        },
        interaction: {
          intersect: false,
          mode: 'index'
        }
      }
    });
  }
}