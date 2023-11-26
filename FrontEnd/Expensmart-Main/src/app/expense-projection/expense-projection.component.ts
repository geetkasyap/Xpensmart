import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Chart } from 'chart.js/auto';
import { ChartOptions } from 'chart.js';  
import annotationPlugin from 'chartjs-plugin-annotation'; 
import zoomPlugin from 'chartjs-plugin-zoom';
import { ToastrService } from 'ngx-toastr';
Chart.register(zoomPlugin);
Chart.register(annotationPlugin);

interface ExpenseProjection {
  userId: number;
  projectedIncome: number;
  categories: CategoryBudget[];
}

interface CategoryBudget {
  categoryId: number;
  categoryName: string;
  desiredBudget: number;
  adjustedBudget: number;
  previousExpense: number;
  presentExpense:number;
  isOverSpent: boolean;
  totalAdjustedBudget: number;
}

@Component({
  selector: 'app-expense-projection',
  standalone: false,
  templateUrl: './expense-projection.component.html',
  styleUrls: ['./expense-projection.component.scss']
})
export class ExpenseProjectionComponent implements OnInit {
  income: number = 0;
  chart: any;

  constructor(private http: HttpClient,private toastr: ToastrService,) {}

  ngOnInit(): void {}

  projectExpenses() {
    if (this.income < 0 || this.income > 9999999999999999999999999999 || this.income === null || this.income === undefined) {
      this.toastr.error("Please enter a valid income between 0 and ten quadrillion");
      return;
    }  
    const userId = localStorage.getItem('userId');
    this.http.post<ExpenseProjection>('https://localhost:7154/Report/project-expenses', { userId, income: this.income })
      .subscribe(data => {
        this.renderChart(data);
      });
  }

  renderChart(projection: any) {
    const ctx = document.getElementById('expenseProjectionChart') as HTMLCanvasElement;
    if (this.chart) this.chart.destroy();
    
    const totalAdjustedBudget=this.income*0.665;
    const income = this.income;
    
    this.chart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: projection.categories.map((c: CategoryBudget) => c.categoryName),
        datasets: [
          {
            label: 'Desired Budget',
            data: projection.categories.map((c: CategoryBudget) => Math.min(c.desiredBudget, c.adjustedBudget)),
            backgroundColor: '#6C63FF', 
            stack: 'budget'
          },
          {
            label: 'Adjusted Budget Difference',
            data: projection.categories.map((c: CategoryBudget) => Math.abs(c.desiredBudget - c.adjustedBudget)),
            backgroundColor: (context) => {
              const dataIndex = context.dataIndex;
              const category = projection.categories[dataIndex];
              return category.adjustedBudget > category.desiredBudget ? '#4CAF50' : '#ff4d4d'; 
            },
            stack: 'budget'
          },
          {
            label: 'Previous Month Expense',
            data: projection.categories.map((c: CategoryBudget) => c.previousExpense),
            backgroundColor: '#44006b', 
            stack: 'previous' 
          },
          {
            label: 'Present Month Expense',
            data: projection.categories.map((c: CategoryBudget) => c.presentExpense),
            backgroundColor: '#FFA500', 
            stack: 'present' 
          }
        ]
      },
      options: {
        responsive: true,
        aspectRatio: 3,
        plugins: {
          zoom: {
            zoom: {
              wheel: {
                enabled: true,
              },
              pinch: {
                enabled: true
              },
              mode: 'xy',
            }
          },
          tooltip: {
            callbacks: {
              label: (context) => {
                if (!context || typeof context.dataIndex !== 'number') {
                  return '';
                }

                const category = projection.categories[context.dataIndex];
                if (!category) {
                  return '';
                }

                if (context.dataset.label === 'Desired Budget') {
                  return `Desired Budget: ₹${context.parsed.y}`;
                } else if (context.dataset.label === 'Adjusted Budget Difference') {
                  return `Adjusted Budget Difference: ₹${context.parsed.y}`;
                } else if (context.dataset.label === 'Previous Month Expense') {
                  return `Previous Month Expense: ₹${context.parsed.y} (${category.isOverSpent ? 'Over Spent' : 'Under Spent'})`;
                } else if (context.dataset.label === 'Present Month Expense') {
                  return `Present Month Expense: ₹${context.parsed.y}`;
                }

                return '';
              }
            }
          },
          annotation: {
            annotations: {
              incomeLine: {
                type: 'line',
                yMin: income,
                yMax: income,
                borderColor: '#FF6347',  
                borderWidth: 2,
                label: {
                  content: 'Monthly Income',
                  display: true, 
                  position: 'center',
                  font: {
                    size: 12,
                    weight: 'bold'
                  },
                  color: '#FF6347'
                }
              },
              budgetLine: {
                type: 'line',
                yMin: totalAdjustedBudget,
                yMax: totalAdjustedBudget,
                borderColor: '#4682B4',  
                borderWidth: 2,
                label: {
                  content: 'Total Adjusted Budget',
                  display: true, 
                  position: 'center',
                  font: {
                    size: 12,
                    weight: 'bold'
                  },
                  color: '#4682B4'
                }
              }
            }
          }
        },
        scales: {
          x: {
            stacked: false, 
            grid: { display: false },
            ticks: { color: '#b3b3b3' }
          },
          y: {
            stacked: false,
            beginAtZero: true,
            ticks: {
              callback: (value) => `₹${value}`,
              color: '#b3b3b3'
            },
            grid: { color: 'rgba(255, 255, 255, 0.1)' }
          }
        }
      }
    });
  }
}

