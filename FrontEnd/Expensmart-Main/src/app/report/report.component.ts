import { GlobalService } from './../services/global.service';
import { ReportService } from './../services/report.service';
import { Component, OnInit,AfterViewInit} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Chart } from 'chart.js/auto';
import moment from 'moment';
import { CategoryExpenseEntity } from '../models/category-expense-entity-model';
import { CategoryEntity } from '../models/category-entity-model';
import { IncludeConsizedExpense } from '../models/expense-includes-entity-model';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import {Context} from 'chartjs-plugin-datalabels';
import { ChartConfiguration, ChartType } from 'chart.js';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import html2canvas from 'html2canvas';
import { jsPDF } from 'jspdf';
import { number } from 'echarts/core';
import { DatePipe } from '@angular/common';
import { TooltipItem } from 'chart.js';
import { Router } from '@angular/router';
import { environment } from '../environments/environments';

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

@Component({
  selector: 'app-report',
  standalone: false,
  
  templateUrl: './report.component.html',
  styleUrl: './report.component.scss'
})
export class ReportComponent implements OnInit{
  selectedMonth: string = moment().format('YYYY-MM');
  monthsList: string[] = [];
  totalExpenses: number = 0;
  monthlyBudget: number = 0;
  expenses: IncludeConsizedExpense[] = [];
  categoryBudgets: CategoryBudget[] = [];
  charts: { [key: string]: Chart } = {};
  totalExpensesByCategory: number =0;
  TotalExpensesByCategory: number =0;
  expensesTot:number = 0;
  LocalUserID=localStorage.getItem("userId");
  categoryPercentages: { [key: number]: CategoryPercentageInfo } = environment.categoryPercentages;
  
  private baseUrl = 'https://localhost:7154/Report';
  private colors = [
        '#6C63FF', '#44006b', '#4CAF50', 
        '#ff4d4d', '#FFA500', '#00CED1'
      ];

  constructor(private http: HttpClient,private ReportService: ReportService,private GlobalService:GlobalService,
    public DatePipe: DatePipe,private router: Router) {
    this.generateMonthsList();
  }

  ngOnInit() {
    this.loadData();
  }
  ngAfterViewInit() {
    const userId = localStorage.getItem('userId') ;
    if (userId ==undefined || null) {
      this.router.navigate(['/Login']);
    }
    setTimeout(() => {
      this.initializeCharts();
      this.updateCharts();
    }, 0);
}
  private initializeTimeScaleChart() {
    const ctx = document.getElementById('timeScaleChart') as HTMLCanvasElement;
    if (!ctx) {
      console.error('TimeScale chart canvas element not found');
      return;
    }
  
    this.charts['timeScale'] = new Chart(ctx, {
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

  private initializePolarAreaChart() {
    const ctx = document.getElementById('polarChart') as HTMLCanvasElement;
    this.charts['polar'] = new Chart(ctx, {
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
          },
          tooltip: {
            callbacks: {
              label: (context: TooltipItem<'polarArea'>) => {
                const value = context.raw as number;
                return `${context.label}: ₹${value.toLocaleString()}`;
              }
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
    private initializeDailyExpenseChart() {
    const ctx = document.getElementById('dailyExpenseChart') as HTMLCanvasElement;
    this.charts['daily'] = new Chart(ctx, {
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
          },
          tooltip: {
            callbacks: {
              label: (context) => {
                return `${context.dataset.label}: ₹${context.parsed.y.toLocaleString()}`;
              }
            }
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

  private updateTimeScaleChart() {
    const { start, end } = this.getDateRange(this.selectedMonth);
    const startDate = moment(start).subtract(6, 'months').startOf('month').format('YYYY-MM-DD');
    const endDate = moment(start).endOf('month').format('YYYY-MM-DD');
    forkJoin({
      expensesData: this.http.get<any[]>(`${this.baseUrl}/GetExpensesByDates?userId=${this.LocalUserID}&startDate=${startDate}&endDate=${end}`),
      monthlyBudgets: this.http.get<{ month: string, amount: number }[]>(`${this.baseUrl}/GetMonthlyBudgets?userId=${this.LocalUserID}&startDate=${startDate}&endDate=${end}`)
    }).subscribe(({ expensesData, monthlyBudgets }) => {
      const monthlyData = this.groupExpensesByMonth(expensesData);
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
      
      if (this.charts['timeScale']) {
        this.charts['timeScale'].data.labels = labels;
        this.charts['timeScale'].data.datasets[0].data = expenseAmounts;
        this.charts['timeScale'].data.datasets[1].data = budgets;
        this.charts['timeScale'].update();
      }
    });
  }

  
  private groupExpensesByMonth(data: any[]): Map<string, MonthlyExpense> {
    const monthlyData = new Map<string, MonthlyExpense>();
  
    data.forEach(expense => {
      const monthKey = moment(expense.date).format('YYYY-MM');
  
      if (!monthlyData.has(monthKey)) {
        monthlyData.set(monthKey, {
          month: monthKey,
          totalAmount: 0,
          budgetAmount: this.monthlyBudget || 0
        });
      }
  
      const monthData = monthlyData.get(monthKey)!;
      monthData.totalAmount += expense.amount || 0;
    });
  
    return monthlyData;
  }

  private updatePolarAreaChart() {
    const { start, end } = this.getDateRange(this.selectedMonth);
    this.http.get<CategoryExpenseEntity[]>(`${this.baseUrl}/GetCategorySummary?userId=${this.LocalUserID}&startDate=${start}&endDate=${end}`)
        .subscribe(data => {
            const labels = data.map(d => d.categoryName);
            const values = data.map(d => Math.abs(d.amount)); 
            this.charts['polar'].data.labels = labels;
            this.charts['polar'].data.datasets[0].data = values;
            this.charts['polar'].update();
        });
  }
  private updateDailyExpenseChart() {
        const { start, end } = this.getDateRange(this.selectedMonth);
        this.http.get<CategoryEntity[]>(`https://localhost:7154/GetCategories`).subscribe(categories => {
            const categoryMap = new Map(categories.map(c => [c.categoryID, c.name]));
            this.http.get<IncludeConsizedExpense[]>(`${this.baseUrl}/GetExpensesByDates?userId=${this.LocalUserID}&startDate=${start}&endDate=${end}`)
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
    
                    this.charts['daily'].data.labels = dates.map(d => moment(d).format('DD MMM'));
                    this.charts['daily'].data.datasets = datasets;
                    this.charts['daily'].update();
                });
        });
    }
    
  private updateBudgetProgress() {
    // if (!this.charts['radial']) {
    //   this.initializeRadialChart();
    // }
    const percentSpent = (this.totalExpenses / this.monthlyBudget) * 100;

    const remaining = Math.max(0, 100 - percentSpent);
    
    this.charts['radial'].data.datasets[0].data = [percentSpent, remaining];

    this.charts['radial'].data.datasets[0].backgroundColor = [
      percentSpent > 100 ? this.colors[3] : this.colors[0], 
      '#2d2d2d'
    ];
    
    this.charts['radial'].update();
  }

private updateCategoryBudgets() {
  const { start, end } = this.getDateRange(this.selectedMonth);
  const categoryPercentages: { [key: number]: CategoryPercentageInfo } = environment.categoryPercentages;

  if (this.monthlyBudget <= 0) {
    console.error('Monthly budget is not set');
    return;
  }

  const categoryExpenses: number[] = new Array(12).fill(0);

  const categoryRequests = Array.from({length: 12}, (_, index) => 
    this.http.get<any>(`${this.baseUrl}/GenerateTotalExpenseReport?userId=${this.LocalUserID}&startDate=${start}&endDate=${end}&categoryId=${index + 1}`)
  );

  forkJoin(categoryRequests).subscribe(
    results => {
      results.forEach((result, index) => {
        categoryExpenses[index] = result.totalAmount || 0;
      });
      for (let index = 0; index < 12; index++) {
        const chartId = `categoryChart${index + 1}`;
        const chart = this.charts[chartId];
        
        if (chart) {
          const categoryAmount = categoryExpenses[index];
          const percentageSpent = (categoryAmount / this.monthlyBudget) * 100;
          
          const categoryRange = categoryPercentages[index + 1];
          
          if (!categoryRange) {
            console.error(`No category range found for index ${index + 1}`);
            continue;
          }
          
          const midPoint = (categoryRange.min + categoryRange.max) / 2;
          
          const color = percentageSpent > categoryRange.max 
            ? '#ff4d4d' 
            : percentageSpent < categoryRange.min 
              ? '#FFA500' 
              : '#4CAF50';

          chart.data.datasets[0].data = [
            percentageSpent,
            midPoint 
          ];
          chart.data.datasets[0].backgroundColor = [
            color,
            '#44006b'
          ];

          if (chart.options.plugins) {
            (chart.options.plugins as any).title = {
              display: true,
              text: `${categoryRange.name} (₹${categoryAmount.toLocaleString()})`,
              color: '#ffffff'
            };
          }
          chart.update();
        } else {
          console.error(`Chart not found: ${chartId}`);
        }
      }
    },
    error => {
      console.error('Error fetching category expenses', error);
    }
  );
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

  // private loadData() {
  //   const { start, end } = this.getDateRange(this.selectedMonth);
  //   forkJoin({
  //     expenses: this.http.get<IncludeConsizedExpense[]>(`${this.baseUrl}/GetExpensesByDates?userId=${this.LocalUserID}&startDate=${start}&endDate=${end}`),
  //     budgets: this.http.get<CategoryBudget[]>(`${this.baseUrl}/expenses/category-budgets?userId=${this.LocalUserID}&start=${start}&end=${end}`),
  //     monthlyBudget: this.http.get<number>(`${this.baseUrl}/GetBudgetByDates?userId=${this.LocalUserID}&startDate=${start}&endDate=${end}`),
  //     totalExpenses: this.http.get<any>(`${this.baseUrl}/GenerateTotalExpenseReport?userId=${this.LocalUserID}&startDate=${start}&endDate=${end}&categoryId=0`)
  //   }).subscribe(({
  //     expenses, 
  //     budgets, 
  //     monthlyBudget, 
  //     totalExpenses
  //   }) => {
  //     this.expenses = expenses;
  //     this.categoryBudgets = budgets;
  //     this.monthlyBudget = monthlyBudget||0;
  //     this.totalExpenses = totalExpenses.totalAmount||0;
  

  //     this.initializeCategoryChart();
  //     this.updateCharts();
  //   });
  // }
  private loadData() {
    const { start, end } = this.getDateRange(this.selectedMonth);
    forkJoin({
      expenses: this.http.get<IncludeConsizedExpense[]>(`${this.baseUrl}/GetExpensesByDates?userId=${this.LocalUserID}&startDate=${start}&endDate=${end}`).pipe(catchError(() => of([]))),
      budgets: this.http.get<CategoryBudget[]>(`${this.baseUrl}/expenses/category-budgets?userId=${this.LocalUserID}&start=${start}&end=${end}`).pipe(catchError(() => of([]))),
      monthlyBudget: this.http.get<number>(`${this.baseUrl}/GetBudgetByDates?userId=${this.LocalUserID}&startDate=${start}&endDate=${end}`).pipe(catchError(() => of(0))),
      totalExpenses: this.http.get<any>(`${this.baseUrl}/GenerateTotalExpenseReport?userId=${this.LocalUserID}&startDate=${start}&endDate=${end}&categoryId=0`).pipe(catchError(() => of({ totalAmount: 0 })))
    }).subscribe({
      next: ({ expenses, budgets, monthlyBudget, totalExpenses }) => {
        this.expenses = expenses;
        this.categoryBudgets = budgets;
        this.monthlyBudget = monthlyBudget || 0;
        this.totalExpenses = totalExpenses.totalAmount || 0;
        this.initializeCategoryChart();
        this.updateCharts();
      },
      error: (err) => console.error('Load data failed:', err)
    });
  }
  private initializeCharts() {
    this.initializeRadialChart();
    this.initializeCategoryChart();
    this.initializeTimeScaleChart();
    this.initializeDailyExpenseChart();
    this.initializePolarAreaChart();
  }
  private initializeRadialChart() {
    const ctx = document.getElementById('radialChart') as HTMLCanvasElement;
    if(!ctx) return;
    
    this.charts['radial'] = new Chart(ctx, {
      type: 'doughnut',
      data: {
        datasets: [{
          data: [0, 100],
          backgroundColor: [
            '#6C63FF',
            '#2d2d2d'
          ]
        }]
      },
      options: {
        cutout: '80%',
        responsive: true,
        plugins: {
          legend: {
            display: false
          },
          title: {
            display: false
          },
          tooltip: {
            enabled: true
          }
        }
      }
    });
  }

private initializeCategoryChart() {
  const categoryPercentages: { [key: number]: CategoryPercentageInfo } = environment.categoryPercentages;

  for (let i = 1; i <= 12; i++) {
    const ctx = document.getElementById(`categoryChart${i}`) as HTMLCanvasElement;
    if (ctx) {

      const existingChart = Chart.getChart(ctx);
      if (existingChart) {
        existingChart.destroy();
      }
      
      const categoryRange = categoryPercentages[i];
      const midPoint = (categoryRange.min + categoryRange.max) / 2;

      const config: ChartConfiguration<'bar', number[], string> = {
        type: 'bar',
        data: {
          labels: ['Spent', 'Desired'],
          datasets: [{
            label: 'Budget Progress',
            data: [0, midPoint], 
            backgroundColor: [
              '#4CAF50',
              '#2d2d2d' 
            ]
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            y: {
              beginAtZero: true,
              max: 100,
              ticks: {
                callback: function(value) {
                  return `${value}%`;
                }
              }
            }
          },
          plugins: {
            legend: {
              display: false
            },
            title: {
              display: true,
              text: categoryRange.name,
              color: '#ffffff'
            },
            tooltip: {
              callbacks: {
                label: function(context) {
                  return `${context.dataset.label}: ${context.parsed.y}%`;
                }
              }
            }
          }
        }
      };

      this.charts[`categoryChart${i}`] = new Chart(ctx, config);
    } else {
      console.error(`Canvas not found: categoryChart${i}`);
    }
  }
}
  private updateCharts() {
    this.updateRadialChart();
    this.updateBudgetProgress();
    this.updateCategoryBudgets();
    this.updateTimeScaleChart();
    this.updatePolarAreaChart();
    this.updateDailyExpenseChart();
    Object.values(this.charts).forEach(chart => chart?.resize());
  }

  private updateRadialChart() {
    const percentage = (this.totalExpenses / this.monthlyBudget) * 100;
    const chart = Chart.getChart('radialChart');
    if (chart) {
      chart.data.datasets[0].data = [percentage , 100 - percentage];
      chart.update();
    }
  }
  
  async exportAsPDF() {
    try {
      const content = document.getElementById('report-content');
      if (!content) {
        console.error('Report content element not found');
        return;
      }

      const pdf = new jsPDF('p', 'mm', 'a4');
      const pageWidth = pdf.internal.pageSize.getWidth();
      const pageHeight = pdf.internal.pageSize.getHeight();
      const margin = 10;

      const canvas = await html2canvas(content, {
        useCORS: true,
        logging: false,
        allowTaint: true,
        height: content.scrollHeight
      });

      const imgWidth = pageWidth - (2 * margin);
      const imgHeight = (canvas.height * imgWidth) / canvas.width;

      let heightLeft = imgHeight;
      let position = margin;
      let page = 1;

      pdf.addImage(
        canvas.toDataURL('image/png'),
        'PNG',
        margin,
        position,
        imgWidth,
        imgHeight
      );

      while (heightLeft >= pageHeight) {
        position = -(pageHeight * page) + margin;
        pdf.addPage();
        pdf.addImage(
          canvas.toDataURL('image/png'),
          'PNG',
          margin,
          position,
          imgWidth,
          imgHeight
        );
        heightLeft -= pageHeight;
        page++;
      }

      const timestamp = new Date().toISOString().slice(0,10);
      pdf.save(`expense-report-${timestamp}.pdf`);

    } catch (error) {
      console.error('PDF export failed:', error);
    }
  }
}