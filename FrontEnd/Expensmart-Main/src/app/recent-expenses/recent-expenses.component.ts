import { GlobalService } from './../services/global.service';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { Component, HostListener, OnInit } from '@angular/core';
import { ExpenseService } from '../services/expense.service';
import { ExpensesEntity } from '../models/expenses-entity-model';
import { HttpClient } from '@angular/common/http';
import { interval } from 'rxjs';
import { startWith, switchMap } from 'rxjs/operators';
import { CategoryEntity } from '../models/category-entity-model';
import {ViewChild, ElementRef } from '@angular/core';
import { trigger, transition, style, animate } from '@angular/animations';

@Component({
  selector: 'app-recent-expenses',
  standalone: false,
  templateUrl: './recent-expenses.component.html',
  styleUrl: './recent-expenses.component.scss',
  animations: [
    trigger('fadeInOut', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(10px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' }))
      ]),
      transition(':leave', [
        animate('300ms ease-in', style({ opacity: 0, transform: 'translateY(10px)' }))
      ])
    ])
  ]
})
export class RecentExpensesComponent implements OnInit{

  @ViewChild('recentExpensesList') recentExpensesList!: ElementRef;
  @ViewChild('upcomingExpensesList') upcomingExpensesList!: ElementRef;
  @ViewChild('remainingExpensesList') remainingExpensesList!: ElementRef;

  recentExpenses: ExpensesEntity[] = [];
  upcomingExpenses: ExpensesEntity[] = [];
  remainingExpenses: ExpensesEntity[] = [];
  categories: CategoryEntity[] = [];
  activeCardIndex = 0;
  userId = localStorage.getItem('userId');
  isLoading: boolean = true;
  autoScrollInterval: any;
  isUserScrolling: boolean = false;

  constructor(
    private http: HttpClient,
    private expenseService: HttpClient,
    private GlobalService: GlobalService,
  ) {}

  ngOnInit() {
    this.loadCategories();
    this.setupAutoRefresh();
  }
  ngAfterViewInit() {
    this.startAutoScroll();
  }
  private loadCategories() {
    this.GlobalService.loadCategories().subscribe(
      categories => {
        this.categories = categories;
        this.loadAllExpenses();
      }
    );
  }

  private setupAutoRefresh() {
       interval(60000)
      .pipe(startWith(0))
      .subscribe(() => {
        this.loadAllExpenses();
      });

  }

  private loadAllExpenses() {
    this.isLoading = true;
    this.loadRecentExpenses();
    this.loadUpcomingExpenses();
    this.loadRemainingExpenses();
  }

  private loadRecentExpenses() {
    this.GlobalService.getRecentExpenses(this.userId).subscribe(
        expenses => {
          this.recentExpenses = expenses;
          this.isLoading = false;
        },
        error => 
          {
            console.error('Error loading recent expenses:', error);
            this.isLoading = false;
          }

      );
  }

  private loadUpcomingExpenses() {
    this.GlobalService.getUpcomingExpenses(this.userId).subscribe(
        expenses => {
          this.upcomingExpenses = expenses;
        },
        error => console.error('Error loading upcoming expenses:', error)
      );
  }

  private loadRemainingExpenses() {
    this.GlobalService.getRemainingExpenses(this.userId).subscribe(
        expenses => {
          this.remainingExpenses = expenses;
        },
        error => console.error('Error loading remaining expenses:', error)
      );
  }

  getCategoryName(categoryId: number): string {
    return this.categories.find(c => c.categoryID === categoryId)?.name || '';
  }

  rotateCards() {
    const cards = document.querySelectorAll('.expense-card');
    cards.forEach((card, index) => {
      const rotation = (index - this.activeCardIndex + cards.length) % cards.length;
      (card as HTMLElement).style.setProperty('--rotation', `${rotation}`);
    });
    this.activeCardIndex = (this.activeCardIndex + 1) % cards.length;
  }

  startAutoScroll() {
    this.autoScrollInterval = setInterval(() => {
      if (!this.isUserScrolling) {
        this.scrollToNextItem(this.recentExpensesList.nativeElement);
        this.scrollToNextItem(this.upcomingExpensesList.nativeElement);
        this.scrollToNextItem(this.remainingExpensesList.nativeElement);
      }
    }, 3000);
  }

  scrollToNextItem(list: HTMLElement) {
    const itemHeight = 75;
    const scrollHeight = list.scrollHeight;
    const clientHeight = list.clientHeight;
    const scrollTop = list.scrollTop;
  
    if (scrollTop + clientHeight >= scrollHeight) {
      list.scrollTo({
        top: 0,
        behavior: 'smooth'
      });
    } else {
      list.scrollTo({
        top: scrollTop + itemHeight,
        behavior: 'smooth'
      });
    }
  }

  @HostListener('mouseenter', ['$event'])
  onMouseEnter() {
    this.isUserScrolling = true;
  }

  @HostListener('mouseleave', ['$event'])
  onMouseLeave() {
    this.isUserScrolling = false;
  }

}

