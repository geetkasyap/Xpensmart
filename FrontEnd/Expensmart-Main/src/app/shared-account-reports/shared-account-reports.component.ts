import { GlobalService } from './../services/global.service';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Chart } from 'chart.js/auto';
import moment from 'moment';
import { DatePipe } from '@angular/common';
import { CategoryExpenseEntity } from '../models/category-expense-entity-model';
import { CategoryEntity } from '../models/category-entity-model';
import { IncludeConsizedExpense } from '../models/expense-includes-entity-model';
import { trigger, state, style, transition, animate } from '@angular/animations';

export interface UserWithUserName {
  userID: number;
  userName: string;
}

interface CategoryBudget {
  amount: number;
  categoryId: number;
  budgetAmount: number;
  percentage: number;
}

interface MonthlyExpense {
  month: string;
  totalAmount: number;
  budgetAmount: number;
}
interface CategoryPercentageInfo {
  min: number;
  max: number;
  name: string;
}
interface UserDetails{
  userID: number;
  userName : string;
  email : string;
  password : string;
  fullName : string;
  groupID : number;
  isActive : boolean;
  createdOn : Date;
  modifiedOn : Date;
  transactionsEntities: null;
  budgetsEntities: null;
  customCategoriesEntities: null;
  expenseProjectionEntities: null;
  expensesEntities: null;
  notificationsEntities: null;
  reportsEntities: null;
  sentRequest: null;
  receivedRequest: null;
}

@Component({
  selector: 'app-shared-account-reports',
  standalone: false,
  
  templateUrl: './shared-account-reports.component.html',
  styleUrl: './shared-account-reports.component.scss',
  animations: [
    trigger('slideInOut', [
      state('void', style({ transform: 'translateX(100%)', opacity: 0 })),
      state('*', style({ transform: 'translateX(0)', opacity: 1 })),
      transition('void => *', [
        animate('300ms ease-in')
      ]),
      transition('* => void', [
        animate('300ms ease-out')
      ])
    ])
  ]
})
export class SharedAccountReportsComponent implements OnInit{
  selectedMonth: string = moment().format('YYYY-MM');
  monthsList: string[] = [];
  users: any[] = [];
  usernames: any[] = [];
  onchangeuserdetails:UserDetails|null=null;
  numberOfUsers:number=0;
  selectedUser2: string = localStorage.getItem('userId') as unknown as string;
  user1Budget: number = 0;
  user2Budget: number = 0;
  user1Expenses: number = 0;
  user2Expenses: number = 0;
  totalSharedAmount: number = 0;
  user1Share: number = 0;
  user2Share: number = 0;
  charts: { [key: string]: Chart } = {};
  private baseUrl = 'https://localhost:7154/Report';
  public currentUserId = localStorage.getItem('userId');
  private colors = [
    '#6C63FF', '#44006b', '#4CAF50', 
    '#ff4d4d', '#FFA500', '#00CED1'
  ];

  constructor(
    public http: HttpClient,
    private datePipe: DatePipe,
    public  cdr:ChangeDetectorRef,
    private GlobalService: GlobalService,
  ) {
    this.generateMonthsList();
  }
  private generateMonthsList() {
    const startDate = moment().subtract(2, 'years');
    const endDate = moment();
    
    while (startDate.isSameOrBefore(endDate, 'month')) {
      this.monthsList.push(startDate.format('YYYY-MM'));
      startDate.add(1, 'month');
    }
  }
  onMonthChange() {
    this.loadData();
  }
  
    ngOnDestroy() {
    Object.values(this.charts).forEach(chart => chart.destroy());
  }
  private getDateRange(monthStr: string): { start: string, end: string } {
    const date = moment(monthStr);
    return {
      start: date.startOf('month').format('YYYY-MM-DD'),
      end: date.endOf('month').format('YYYY-MM-DD')
    };
  }
  ngOnInit() {
    this.loadUsers();
    this.loadData();
  }
  ngAfterViewInit() {
    this.initializeCharts();
}

  private loadUsers() {
    this.GlobalService.getSharedUserDetails(this.currentUserId!).subscribe(users => {
            this.users = users ;
            this.cdr.detectChanges();
        });
}


  onUser2Change() {
    if (this.selectedUser2) {
      this.loadData();
    }
  }

  private initializeCharts() {
    this.initializeRadialChart('1');
    this.initializeTimeScaleChart('1');
    this.initializePolarAreaChart('1');
    this.initializeDailyExpenseChart('1');
    
    if (this.selectedUser2) {
      this.initializeRadialChart('2');
      this.initializeTimeScaleChart('2');
      this.initializePolarAreaChart('2');
      this.initializeDailyExpenseChart('2');
    }
  }

  private initializeRadialChart(userSuffix: string) {
    const ctx = document.getElementById(`radialChart${userSuffix}`) as HTMLCanvasElement;
    if (!ctx) return;

    this.charts[`radial${userSuffix}`] = new Chart(ctx, {
      type: 'doughnut',
      data: {
        datasets: [{
          data: [0, 100],
          backgroundColor: ['#6C63FF', '#2d2d2d']
        }]
      },
      options: {
        cutout: '80%',
        responsive: true,
        plugins: {
          legend: { display: false },
          tooltip: { enabled: true }
        }
      }
    });
  }
  private initializeTimeScaleChart(userSuffix: string) {
    const ctx = document.getElementById(`timeScaleChart${userSuffix}`) as HTMLCanvasElement;
    if (!ctx) {
      console.error('TimeScale chart canvas element not found');
      return;
    }
  
    this.charts[`timeScale${userSuffix}`] = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: [],
        datasets: [
          {
            type: 'bar',
            label: 'Expenses',
            data: [],
            backgroundColor: '#6C63FF',
            borderRadius: 6,
            order: 2
          },
          {
            type: 'line',
            label: 'Budget',
            data: [],
            borderColor: '#44006b',
            borderWidth: 2,
            pointRadius: 4,
            pointHoverRadius: 6,
            fill: false,
            tension: 0.4,
            order: 1
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        interaction: {
          intersect: false,
          mode: 'index'
        },
        scales: {
          y: {
            beginAtZero: true,
            grid: {
              color: 'rgba(255, 255, 255, 0.1)'
            },
            ticks: {
              color: '#b3b3b3',
              callback: (value) => {
                return '₹' + value.toLocaleString();
              }
            }
          },
          x: {
            grid: {
              display: false
            },
            ticks: {
              color: '#b3b3b3'
            }
          }
        },
        plugins: {
          legend: {
            labels: {
              color: '#ffffff'
            }
          },
          tooltip: {
            callbacks: {
              label: (context) => {
                return `${context.dataset.label}: ₹${context.parsed.y.toLocaleString()}`;
              }
            }
          }
        }
      }
    });
  }
  private initializePolarAreaChart(userSuffix: string) {
    const ctx = document.getElementById(`polarChart${userSuffix}`) as HTMLCanvasElement;
    this.charts[`polar${userSuffix}`] = new Chart(ctx, {
      type: 'polarArea',
      data: {
        labels: [],
        datasets: [{
          data: [],
          backgroundColor: this.colors,
          borderColor: 'rgba(255, 255, 255, 0.2)'
        }]
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            position: 'right',
            labels: {
              color: '#ffffff'
            }
          }
        },
        scales: {
          r: {
            ticks: {
              color: '#b3b3b3'
            },
            grid: {
              color: 'rgba(255, 255, 255, 0.1)'
            }
          }
        }
      }
    });
  }
  private initializeDailyExpenseChart(userSuffix: string) {
    const ctx = document.getElementById(`dailyExpenseChart${userSuffix}`) as HTMLCanvasElement;
    this.charts[`daily${userSuffix}`] = new Chart(ctx, {
      type: 'line',
      data: {
        labels: [],
        datasets: []
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            position: 'top',
            labels: {
              color: '#ffffff'
            }
          },
          title: {
            display: true,
            text: 'Daily Expenses by Category',
            color: '#ffffff'
          }
        },
        scales: {
          y: {
            beginAtZero: true,
            stacked: true,
            grid: {
              color: 'rgba(255, 255, 255, 0.1)'
            },
            ticks: {
              color: '#b3b3b3'
            }
          },
          x: {
            grid: {
              display: false
            },
            ticks: {
              color: '#b3b3b3'
            }
          }
        }
      }
    });
  }

  private loadData() {
    const { start, end } = this.getDateRange(this.selectedMonth);
    
    this.loadUserData(this.currentUserId!, '1');
    
    if (this.selectedUser2) {
      this.loadUserData(this.selectedUser2, '2');
  
    }
  }

  private loadUserData(userId: string, userSuffix: string) {
    const { start, end } = this.getDateRange(this.selectedMonth);
    
    forkJoin({
      expenses: this.http.get<IncludeConsizedExpense[]>(
        `${this.baseUrl}/GetExpensesByDates?userId=${userId}&startDate=${start}&endDate=${end}`
      ),
      budget: this.http.get<number>(
        `${this.baseUrl}/GetBudgetByDates?userId=${userId}&startDate=${start}&endDate=${end}`
      ),
      totalExpenses: this.http.get<any>(
        `${this.baseUrl}/GenerateTotalExpenseReport?userId=${userId}&startDate=${start}&endDate=${end}&categoryId=0`
      )
    }).subscribe(({ expenses, budget, totalExpenses }) => {
      if (userSuffix === '1') {
        this.user1Budget = budget || 0;
        this.user1Expenses = totalExpenses.totalAmount || 0;
      } else {
        this.user2Budget = budget || 0;
        this.user2Expenses = totalExpenses.totalAmount || 0;
      }
      
      this.updateCharts(userSuffix);
    });
  }
  private groupExpensesByMonth(data: any[],userSuffix: string): Map<string, MonthlyExpense> {
    if(userSuffix='1')
    {
      const monthlyData = new Map<string, MonthlyExpense>();
    
      data.forEach(expense => {
        const monthKey = moment(expense.date).format('YYYY-MM');
    
        if (!monthlyData.has(monthKey)) {
          monthlyData.set(monthKey, {
            month: monthKey,
            totalAmount: 0,
            budgetAmount: this.user1Budget || 0
          });
        }
    
        const monthData = monthlyData.get(monthKey)!;
        monthData.totalAmount += expense.amount || 0;
      });
    
      return monthlyData;
    }
    else
    {
      const monthlyData = new Map<string, MonthlyExpense>();
    
      data.forEach(expense => {
        const monthKey = moment(expense.date).format('YYYY-MM');
    
        if (!monthlyData.has(monthKey)) {
          monthlyData.set(monthKey, {
            month: monthKey,
            totalAmount: 0,
            budgetAmount: this.user2Budget || 0
          });
        }
    
        const monthData = monthlyData.get(monthKey)!;
        monthData.totalAmount += expense.amount || 0;
      });
    
      return monthlyData;
    }
      
    }
  private updateCharts(userSuffix: string) {
    this.updateRadialChart('1');
    this.updateTimeScaleChart('1');
    this.updatePolarAreaChart('1');
    this.updateDailyExpenseChart('1');
    
    if (this.selectedUser2) {
      this.updateRadialChart('2');
      this.updateTimeScaleChart('2');
      this.updatePolarAreaChart('2');
      this.updateDailyExpenseChart('2');
    }
  }
  private updateRadialChart(userSuffix: string) {
    if(userSuffix=='1')
    {
      const percentage = (this.user1Expenses / this.user1Budget) * 100;
      const chart = Chart.getChart(`radialChart${userSuffix}`);
    if (chart) {
      chart.data.datasets[0].data = [percentage, 100 - percentage];
      chart.update();
    }
    }
    else
      {
        const percentage = (this.user2Expenses / this.user2Budget) * 100;
        const chart = Chart.getChart(`radialChart${userSuffix}`);
    if (chart) {
      chart.data.datasets[0].data = [percentage, 100 - percentage];
      chart.update();
    }
      }
  }
  private updateTimeScaleChart(userSuffix: string) {
    if(userSuffix=="1")
    {
      const { start, end } = this.getDateRange(this.selectedMonth);
      const startDate = moment(start).subtract(6, 'months').startOf('month').format('YYYY-MM-DD');
      const endDate = moment(start).endOf('month').format('YYYY-MM-DD');
      forkJoin({
        expensesData: this.http.get<any[]>(`${this.baseUrl}/GetExpensesByDates?userId=${this.currentUserId}&startDate=${startDate}&endDate=${end}`),
        monthlyBudgets: this.http.get<{ month: string, amount: number }[]>(`${this.baseUrl}/GetMonthlyBudgets?userId=${this.currentUserId}&startDate=${startDate}&endDate=${end}`)
      }).subscribe(({ expensesData, monthlyBudgets }) => {
        const monthlyData = this.groupExpensesByMonth(expensesData,userSuffix);
        const budgetMap = new Map(
          monthlyBudgets.map(b => [b.month, b.amount])
        );
    
        const labels = [];
        const expenseAmounts = [];
        const budgets = [];
        
        for (let i = 5; i >= 0; i--) {
          const month = moment().subtract(i, 'months').format('YYYY-MM');
          labels.push(month);
          expenseAmounts.push(monthlyData.get(month)?.totalAmount || 0);
          budgets.push(budgetMap.get(month) || 0);
        }
        
        if (this.charts[`timeScale${userSuffix}`]) {
          this.charts[`timeScale${userSuffix}`].data.labels = labels;
          this.charts[`timeScale${userSuffix}`].data.datasets[0].data = expenseAmounts;
          this.charts[`timeScale${userSuffix}`].data.datasets[1].data = budgets;
          this.charts[`timeScale${userSuffix}`].update();
        }
      });
    }
    else
    {
      const { start, end } = this.getDateRange(this.selectedMonth);
      const startDate = moment(start).subtract(6, 'months').startOf('month').format('YYYY-MM-DD');
      const endDate = moment(start).endOf('month').format('YYYY-MM-DD');
      forkJoin({
        expensesData: this.http.get<any[]>(`${this.baseUrl}/GetExpensesByDates?userId=${this.selectedUser2}&startDate=${startDate}&endDate=${end}`),
        monthlyBudgets: this.http.get<{ month: string, amount: number }[]>(`${this.baseUrl}/GetMonthlyBudgets?userId=${this.selectedUser2}&startDate=${startDate}&endDate=${end}`)
      }).subscribe(({ expensesData, monthlyBudgets }) => {
        const monthlyData = this.groupExpensesByMonth(expensesData,userSuffix);
        const budgetMap = new Map(
          monthlyBudgets.map(b => [b.month, b.amount])
        );
    
        const labels = [];
        const expenseAmounts = [];
        const budgets = [];
        
        for (let i = 5; i >= 0; i--) {
          const month = moment().subtract(i, 'months').format('YYYY-MM');
          labels.push(month);
          expenseAmounts.push(monthlyData.get(month)?.totalAmount || 0);
          budgets.push(budgetMap.get(month) || 0);
        }
        
        if (this.charts[`timeScale${userSuffix}`]) {
          this.charts[`timeScale${userSuffix}`].data.labels = labels;
          this.charts[`timeScale${userSuffix}`].data.datasets[0].data = expenseAmounts;
          this.charts[`timeScale${userSuffix}`].data.datasets[1].data = budgets;
          this.charts[`timeScale${userSuffix}`].update();
        }
      });
    }
    
  }
  private updatePolarAreaChart(userSuffix: string) {
    if(userSuffix=='1')
    {
      const { start, end } = this.getDateRange(this.selectedMonth);
      this.http.get<CategoryExpenseEntity[]>(`${this.baseUrl}/GetCategorySummary?userId=${this.currentUserId}&startDate=${start}&endDate=${end}`)
          .subscribe(data => {
              const labels = data.map(d => d.categoryName);
              const values = data.map(d => Math.abs(d.amount)); 
              this.charts[`polar${userSuffix}`].data.labels = labels;
              this.charts[`polar${userSuffix}`].data.datasets[0].data = values;
              this.charts[`polar${userSuffix}`].update();
          });
    }
    else
    {
      const { start, end } = this.getDateRange(this.selectedMonth);
      this.http.get<CategoryExpenseEntity[]>(`${this.baseUrl}/GetCategorySummary?userId=${this.selectedUser2}&startDate=${start}&endDate=${end}`)
          .subscribe(data => {
              const labels = data.map(d => d.categoryName);
              const values = data.map(d => Math.abs(d.amount)); 
              this.charts[`polar${userSuffix}`].data.labels = labels;
              this.charts[`polar${userSuffix}`].data.datasets[0].data = values;
              this.charts[`polar${userSuffix}`].update();
          });
    }

  }
  private updateDailyExpenseChart(userSuffix: string) {
    if(userSuffix=='1')
    {
      const { start, end } = this.getDateRange(this.selectedMonth);
      this.http.get<CategoryEntity[]>(`https://localhost:7154/GetCategories`).subscribe(categories => {
          const categoryMap = new Map(categories.map(c => [c.categoryID, c.name]));
          this.http.get<IncludeConsizedExpense[]>(`${this.baseUrl}/GetExpensesByDates?userId=${this.currentUserId}&startDate=${start}&endDate=${end}`)
              .subscribe(data => {
                  const expensesByCategory = data.reduce((acc, expense) => {
                      const date = moment(expense.date).format('YYYY-MM-DD');
                      const categoryId = expense.categoryID;
                     
                      if (!acc[categoryId]) {
                          acc[categoryId] = {};
                      }
                     
                      if (!acc[categoryId][date]) {
                          acc[categoryId][date] = 0;
                      }
                     
                      acc[categoryId][date] += expense.expenseType?.toLowerCase() === 'credited'
                          ? -expense.amount
                          : expense.amount;
                     
                      return acc;
                  }, {} as { [key: number]: { [key: string]: number } });
  
                  const dates = [...new Set(data.map(d =>
                      moment(d.date).format('YYYY-MM-DD')
                  ))].sort();
  
                  const datasets = Object.entries(expensesByCategory).map(([categoryId, expenses], index) => ({
                      label: categoryMap.get(Number(categoryId)) || `Category ${categoryId}`,
                      data: dates.map(date => expenses[date] || 0),
                      borderColor: this.colors[index % this.colors.length],
                      backgroundColor: `${this.colors[index % this.colors.length]}80`,
                      fill: true
                  }));
  
                  this.charts[`daily${userSuffix}`].data.labels = dates.map(d => moment(d).format('DD MMM'));
                  this.charts[`daily${userSuffix}`].data.datasets = datasets;
                  this.charts[`daily${userSuffix}`].update();
              });
      });
    }
    else
    {
      const { start, end } = this.getDateRange(this.selectedMonth);
      this.http.get<CategoryEntity[]>(`https://localhost:7154/GetCategories`).subscribe(categories => {
          const categoryMap = new Map(categories.map(c => [c.categoryID, c.name]));
          this.http.get<IncludeConsizedExpense[]>(`${this.baseUrl}/GetExpensesByDates?userId=${this.selectedUser2}&startDate=${start}&endDate=${end}`)
              .subscribe(data => {
                  const expensesByCategory = data.reduce((acc, expense) => {
                      const date = moment(expense.date).format('YYYY-MM-DD');
                      const categoryId = expense.categoryID;
                     
                      if (!acc[categoryId]) {
                          acc[categoryId] = {};
                      }
                     
                      if (!acc[categoryId][date]) {
                          acc[categoryId][date] = 0;
                      }
                     
                      acc[categoryId][date] += expense.expenseType?.toLowerCase() === 'credited'
                          ? -expense.amount
                          : expense.amount;
                     
                      return acc;
                  }, {} as { [key: number]: { [key: string]: number } });
  
                  const dates = [...new Set(data.map(d =>
                      moment(d.date).format('YYYY-MM-DD')
                  ))].sort();
  
                  const datasets = Object.entries(expensesByCategory).map(([categoryId, expenses], index) => ({
                      label: categoryMap.get(Number(categoryId)) || `Category ${categoryId}`,
                      data: dates.map(date => expenses[date] || 0),
                      borderColor: this.colors[index % this.colors.length],
                      backgroundColor: `${this.colors[index % this.colors.length]}80`,
                      fill: true
                  }));
  
                  this.charts[`daily${userSuffix}`].data.labels = dates.map(d => moment(d).format('DD MMM'));
                  this.charts[`daily${userSuffix}`].data.datasets = datasets;
                  this.charts[`daily${userSuffix}`].update();
              });
      });
    }

      }
}



