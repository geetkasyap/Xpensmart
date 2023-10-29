import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { DashBoardChartComponent } from './dash-board-chart/dash-board-chart.component';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.scss'
})
export class AppComponent  implements OnInit,AfterViewInit{
  title = 'Expensmart-Main';
  showNavbar = true;
  messageSent: string = '';
  suggestions: string[] = [];
  errorMessage: string = '';
  LocalUserID = localStorage.getItem('userId');
  currentSuggestionIndex: number = 0;
  intervalId: any;
  @ViewChild(DashBoardChartComponent) DashBoardChartComponent!: DashBoardChartComponent;
  constructor(private router: Router,private http: HttpClient, private toastr: ToastrService) {}

  ngOnInit(): void {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      const currentRoute = event.urlAfterRedirects;
      if (currentRoute === '/Login' || currentRoute === '/SignUp' || currentRoute==='/Home') {
        this.showNavbar = false;
      } else {
        this.showNavbar = true;
      }
    });
  }
ngAfterViewInit(): void {
  this.getSuggestions();
}


  getSuggestions() {
    const url = `https://localhost:7154/api/BudgetSuggestions/GetSuggestions/${this.LocalUserID}`;
    this.http.get<any>(url).subscribe(
      (response:any) => {
        if (response.status === true) {
          this.suggestions = response.data;
          this.currentSuggestionIndex = 0;
          this.startNotificationLoop();
        } else {
          this.errorMessage = response.data;
        }
      },
    );
  }

  startNotificationLoop() {
    this.intervalId = setInterval(() => {
      if (this.suggestions.length > 0) {
        this.showRandomNotification(this.suggestions[this.currentSuggestionIndex]);
        this.currentSuggestionIndex = (this.currentSuggestionIndex + 1) % this.suggestions.length;
      }
      },14000);
  }

  navigateToReports() {
    window.location.href = '/Report';
  }

  showRandomNotification(message: string) {
    if(localStorage.getItem('userId')!=null||undefined)
    {

      const toast = this.toastr.info(message, 'Personal Budget Suggestion', {
        closeButton: true,
        progressBar: true,
        newestOnTop: false,
        positionClass: "toast-bottom-right",
        timeOut: 2000,
        extendedTimeOut: 1000,
      });

  
      toast.onTap.subscribe(() => {
        this.navigateToReports();
      });
    }
  }

  Logout(event:any){
    this.DashBoardChartComponent.resetChartData();
  }

}

