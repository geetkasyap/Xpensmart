import { ANIMATION_MODULE_TYPE, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';


import { MatRadioModule } from '@angular/material/radio';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { APP_INITIALIZER } from '@angular/core';
import {provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { HttpClientModule, provideHttpClient, withFetch } from '@angular/common/http';
import {ChangeDetectionStrategy, Component, OnDestroy, signal, inject} from '@angular/core';
import {MatButtonModule} from '@angular/material/button';
import {DateAdapter, MAT_DATE_FORMATS, provideNativeDateAdapter} from '@angular/material/core';
import {MatCalendar, MatDatepickerModule} from '@angular/material/datepicker';
import {Subject} from 'rxjs';
import {startWith, takeUntil} from 'rxjs/operators';
import { DatePipe } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  FormBuilder,
  FormControl,
} from '@angular/forms';
import {
  FloatLabelType
} from '@angular/material/form-field';
import { map } from 'rxjs/operators';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import{NgToastModule} from 'ng-angular-popup';
import { ToastrModule } from 'ngx-toastr';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import {MatTooltipModule} from '@angular/material/tooltip';

import { ValidationsServiceService } from './shared/validations-service.service';
import { PreloadFactory } from './shared/preload-service.factory';
import { ExpenseFormComponent } from './expense-form/expense-form.component';
import { LoginPageComponent } from './login-page/login-page.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import {MatCardModule} from '@angular/material/card';
import { MainDashBoardComponent } from './main-dash-board/main-dash-board.component';
import { CommonModule } from '@angular/common';
import { ExpensesTableComponent } from './expenses-table/expenses-table.component';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialogModule } from '@angular/material/dialog';
import {MatChipsModule} from '@angular/material/chips';
import { RecentExpensesComponent } from './recent-expenses/recent-expenses.component';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { forkJoin } from 'rxjs';

import * as echarts from 'echarts/core';
import {
  DatasetComponent,
  DatasetComponentOption,
  TitleComponent,
  TitleComponentOption,
  TooltipComponent,
  TooltipComponentOption,
  GridComponent,
  GridComponentOption,
  TransformComponent
} from 'echarts/components';
import { LineChart, LineSeriesOption } from 'echarts/charts';
import { LabelLayout, UniversalTransition } from 'echarts/features';
import { CanvasRenderer } from 'echarts/renderers';

import { NgxEchartsModule } from 'ngx-echarts';
import { DashBoardChartComponent } from './dash-board-chart/dash-board-chart.component';
import { BaseChartDirective } from 'ng2-charts';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { BudgetsComponent } from './budgets/budgets.component';
import { BudgetsTableComponent } from './budgets/budgets-table/budgets-table.component';
import { ReportComponent } from './report/report.component';
import { SplitExpensesFormComponent } from './split-expenses-form/split-expenses-form.component';
import { BulkExpensesComponent } from './bulk-expenses/bulk-expenses.component';
import { MatSnackBarModule } from '@angular/material/snack-bar'; 
import html2canvas from 'html2canvas';
import { jsPDF } from 'jspdf';  
import { MatNativeDateModule } from '@angular/material/core';
import { NavbarComponent } from './navbar/navbar.component';
import { SharedAccountFormComponent } from './shared-account-form/shared-account-form.component';
import { SideBarComponent } from './side-bar/side-bar.component';
import { AddRecurringExpenseComponent } from './add-recurring-expense/add-recurring-expense.component';
import { ViewRecurringExpensesComponent } from './view-recurring-expenses/view-recurring-expenses.component';
import { SharedAccountReportsComponent } from './shared-account-reports/shared-account-reports.component';
import { SharedAccountExpensesComponent } from './shared-account-expenses/shared-account-expenses.component';
import { SharedAccountTablesComponent } from './shared-account-tables/shared-account-tables.component';
import { ConfirmationDialogComponent } from './confirmation-dialog/confirmation-dialog.component';
import { GroupCreationComponent } from './group-creation/group-creation.component';
import { ViewGroupsComponent } from './view-groups/view-groups.component';
import { AddGroupExpenseComponent } from './add-group-expense/add-group-expense.component';
import { ExpenseProjectionComponent } from './expense-projection/expense-projection.component';
import { SharedAccountTableStructureComponent } from './shared-account-table-structure/shared-account-table-structure.component';
import { HomePageComponent } from './home-page/home-page.component';
import { ErrorPageComponent } from './error-page/error-page.component';

@NgModule({
  declarations: [
    AppComponent,
    ExpenseFormComponent,
    SignUpComponent,
    LoginPageComponent,
    MainDashBoardComponent,
    ExpensesTableComponent,
    RecentExpensesComponent,
    DashBoardChartComponent,
    BudgetsComponent,
    BudgetsTableComponent,
    ReportComponent,
    SplitExpensesFormComponent,
    BulkExpensesComponent,
    NavbarComponent,
    SharedAccountFormComponent,
    SideBarComponent,
    AddRecurringExpenseComponent,
    ViewRecurringExpensesComponent,
    SharedAccountReportsComponent,
    SharedAccountExpensesComponent,
    SharedAccountTablesComponent,
    ConfirmationDialogComponent,
    GroupCreationComponent,
    ViewGroupsComponent,
    AddGroupExpenseComponent,
    ExpenseProjectionComponent,
    SharedAccountTableStructureComponent,
    HomePageComponent,
    ErrorPageComponent,
      ],
  imports: [
    BrowserModule,
    AppRoutingModule,

    MatSnackBarModule,
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    MatInputModule,
    HttpClientModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatSelectModule,
    MatIconModule,
    MatRadioModule,
    MatButtonModule,
    MatDatepickerModule,
    MatFormFieldModule, MatInputModule, MatDatepickerModule, MatIconModule,
    MatToolbarModule,
    MatTableModule,MatPaginatorModule,MatSortModule,BrowserAnimationsModule,NgToastModule,
    ToastrModule.forRoot(),
    MatTooltipModule,
    MatCardModule,
    CommonModule,
    MatPaginator,
    MatSort,
    MatDialogModule,
    MatChipsModule,
    NoopAnimationsModule,
    NgxEchartsModule,
    BaseChartDirective,
    MatNativeDateModule,
    BrowserAnimationsModule,
    NoopAnimationsModule
  ],
  providers: [
    DatePipe,
    provideNativeDateAdapter(),
    provideAnimationsAsync(),
    provideAnimations(),
    provideToastr(),
    provideCharts(withDefaultRegisterables()),
    {
      provide: APP_INITIALIZER,
      deps:[
        ValidationsServiceService
      ],
      multi: true,
      useFactory:PreloadFactory
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
