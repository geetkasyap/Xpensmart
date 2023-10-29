import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExpenseFormComponent } from './expense-form/expense-form.component';
import { LoginPageComponent } from './login-page/login-page.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { MainDashBoardComponent } from './main-dash-board/main-dash-board.component';
import { ExpensesTableComponent } from './expenses-table/expenses-table.component';
import { RecentExpensesComponent } from './recent-expenses/recent-expenses.component';
import { DashBoardChartComponent } from './dash-board-chart/dash-board-chart.component';
import { BudgetsComponent } from './budgets/budgets.component';
import { ReportComponent } from './report/report.component';
import { SplitExpensesFormComponent } from './split-expenses-form/split-expenses-form.component';
import { BulkExpensesComponent } from './bulk-expenses/bulk-expenses.component';
import { NavbarComponent } from './navbar/navbar.component';
import { AddRecurringExpenseComponent } from './add-recurring-expense/add-recurring-expense.component';
import { ViewRecurringExpensesComponent } from './view-recurring-expenses/view-recurring-expenses.component';
import { SharedAccountFormComponent } from './shared-account-form/shared-account-form.component';
import { SharedAccountReportsComponent } from './shared-account-reports/shared-account-reports.component';
import { SharedAccountExpensesComponent } from './shared-account-expenses/shared-account-expenses.component';
import { SharedAccountTablesComponent } from './shared-account-tables/shared-account-tables.component';
import { GroupCreationComponent } from './group-creation/group-creation.component';
import { ViewGroupsComponent } from './view-groups/view-groups.component';
import { ExpenseProjectionComponent } from './expense-projection/expense-projection.component';
import { HomePageComponent } from './home-page/home-page.component';
import { ErrorPageComponent } from './error-page/error-page.component';
const routes: Routes = [
  {path: "", redirectTo:"Home", pathMatch: "full"},
  {path: "ExpenseForm", component:ExpenseFormComponent},
  {path:"Login", component:LoginPageComponent},
  {path:"SignUp", component:SignUpComponent},
  {path:"Main", component:MainDashBoardComponent},
  {path:"ExpensesTable", component:ExpensesTableComponent},
  {path:"Budgets",component:BudgetsComponent},
  {path:"Report",component:ReportComponent},
  {path:"SplitExpenses",component:SplitExpensesFormComponent},
  {path:"SharedAcountForm",component:SharedAccountFormComponent},
  {path:"AddRecurringExpense",component:AddRecurringExpenseComponent},
  {path:"ViewRecurringExpenses",component:ViewRecurringExpensesComponent},
  {path:"SharedExpenseReports",component:SharedAccountReportsComponent},
  {path:"AddSharedExpenses",component:SharedAccountExpensesComponent},
  {path:"SharedExpenseTable",component:SharedAccountTablesComponent},
  {path:"GroupCreation",component:GroupCreationComponent},
  {path:"ViewGroups",component:ViewGroupsComponent},
  {path:"ExpenseProjection",component:ExpenseProjectionComponent},
  {path:"Home",component:HomePageComponent},
  {path:"Error",component:ErrorPageComponent}


];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
