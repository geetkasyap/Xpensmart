import { TestBed } from '@angular/core/testing';

import { ExpenseService } from './expense.service';

describe('ExpenseService', () => {
  let service: ExpenseService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExpenseService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});




// import { TestBed } from '@angular/core/testing';
// import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
// import { ExpenseService } from './expense.service';
// import { ExpensesEntity } from '../models/expenses-entity-model';
// import { ConsizedExpense } from '../models/expense-entity-consizedModel';

// describe('ExpenseService', () => {
//   let service: ExpenseService;
//   let httpMock: HttpTestingController;

//   beforeEach(() => {
//     TestBed.configureTestingModule({
//       imports: [HttpClientTestingModule],
//       providers: [ExpenseService],
//     });
//     service = TestBed.inject(ExpenseService);
//     httpMock = TestBed.inject(HttpTestingController);
//   });

//   afterEach(() => {
//     httpMock.verify();
//   });

//   it('should be created', () => {
//     expect(service).toBeTruthy();
//   });

//   it('should get expenses', () => {
//     const mockExpenses: ExpensesEntity[] = [
//       { expenseID: 1, amount: 100, description: 'Test Expense 1', date: new Date(), categoryID: 1, receiptUrl: '', expenseType: 'One-time', isRecurring: false, userID: 1 },
//       { expenseID: 2, amount: 200, description: 'Test Expense 2', date: new Date(), categoryID: 2, receiptUrl: '', expenseType: 'One-time', isRecurring: false, userID: 1 },
//     ];

//     service.getExpenses().subscribe(expenses => {
//       expect(expenses).toEqual(mockExpenses);
//     });

//     const req = httpMock.expectOne(`${service['apiUrl']}/GetExpenseDetails/0?User ID=${service.UserID}`);
//     expect(req.request.method).toBe('GET');
//     req.flush(mockExpenses);
//   });

//   it('should handle error when getting expenses', () => {
//     service.getExpenses().subscribe(
//       () => fail('expected an error, not expenses'),
//       error => {
//         expect(error).toBeDefined();
//       }
//     );

//     const req = httpMock.expectOne(`${service['apiUrl']}/GetExpenseDetails/0?User ID=${service.UserID}`);
//     expect(req.request.method).toBe('GET');
//     req.flush('Error loading expenses', { status: 500, statusText: 'Server Error' });
//   });

//   it('should get expenses by user ID', () => {
//     const userId = 1;
//     const mockExpenses: ExpensesEntity[] = [
//       { expenseID: 1, amount: 100, description: 'Test Expense 1', date: new Date(), categoryID: 1, receiptUrl: '', expenseType: 'One-time', isRecurring: false, userID: userId },
//     ];

//     service.getExpensesByUserID(userId).subscribe(expenses => {
//       expect(expenses).toEqual(mockExpenses);
//     });

//     const req = httpMock.expectOne(`${service['apiUrl']}/GetExpenseDetails/0?User ID=${userId}`);
//     expect(req.request.method).toBe('GET');
//     req.flush(mockExpenses);
//   });

//   it('should handle error when getting expenses by user ID', () => {
//     const userId = 1;

//     service.getExpensesByUserID(userId).subscribe(
//       () => fail('expected an error, not expenses'),
//       error => {
//         expect(error).toBeDefined();
//       }
//     );

//     const req = httpMock.expectOne(`${service['apiUrl']}/GetExpenseDetails/0?User ID=${userId}`);
//     expect(req.request.method).toBe('GET');
//     req.flush('Error loading expenses', { status: 500, statusText: 'Server Error' });
//   });

//   it('should get expenses by date string', () => {
//     const mockConsizedExpenses: ConsizedExpense[] = [
//       { expenseID: 1, userID: 1, amount: 100, date: new Date(), categoryID: 1, receiptUrl: '', description: 'Test Expense 1', expenseType: 'One-time', isActive: true, isRecurring: false, createdOn: new Date(), modifiedOn: new Date(), createdBy: '', modifiedBy: '' },
//     ];

//     service.getExpensesByDateString().subscribe(expenses => {
//       expect(expenses).toEqual(mockConsizedExpenses);
//     });

//     const req = httpMock.expectOne(`${service['apiUrl']}/GetExpenseDetails/0?User ID=${service.UserIdNumber}`);
//     expect(req.request.method).toBe('GET');
//     req.flush(mockConsizedExpenses);
//   });

//   it('should handle error when getting expenses by date string', () => {
//     service.getExpensesByDateString().subscribe(
//       () => fail('expected an error, not expenses'),
//       error => {
//         expect(error).toBeDefined();
//       }
//     );

//     const req = httpMock.expectOne(`${service['apiUrl']}/GetExpenseDetails/0?User ID=${service.UserIdNumber}`);
//     expect(req.request.method).toBe('GET');
//     req.flush('Error loading expenses', { status: 500, statusText: 'Server Error' });
//   });

//   it('should add an expense', () => {
//     const mockExpense: ExpensesEntity = { expenseID: 1, amount: 100, description: 'Test Expense', date: new Date(), categoryID: 1, receiptUrl: '', expenseType: 'One-time', isRecurring: false, userID: 1 };
//     const mockResponse = { status: true, data: 'Expense added successfully' };

//     service.addExpense(mockExpense).subscribe(response => {
//       expect(response).toEqual(mockResponse);
//     });

//     const req = httpMock.expectOne(`${service['apiUrl']}/AddExpenses`);
//     expect(req.request.method).toBe('POST');
//     req.flush(mockResponse);
//   });

//   it('should handle error when adding an expense', () => {
//     const mockExpense: ExpensesEntity = { expenseID: 1, amount: 100, description: 'Test Expense', date: new Date(), categoryID: 1, receiptUrl: '', expenseType: 'One-time', isRecurring: false, userID: 1 };

//     service.addExpense(mockExpense).subscribe(
//       () => fail('expected an error, not an expense'),
//       error => {
//         expect(error).toBeDefined();
//       }
//     );

//     const req = httpMock.expectOne(`${service['apiUrl']}/AddExpenses`);
//     expect(req.request.method).toBe('POST');
//     req.flush('Error adding expense', { status: 500, statusText: 'Server Error' });
//   });

//   it('should update an expense', () => {
//     const mockExpense: ExpensesEntity = { expenseID: 1, amount: 100, description: 'Updated Expense', date: new Date(), categoryID: 1, receiptUrl: '', expenseType: 'One-time', isRecurring: false, userID: 1 };
//     const mockResponse = { message: 'Expense updated successfully' };

//     service.updateExpense(mockExpense).subscribe(response => {
//       expect(response).toEqual(mockResponse);
//     });

//     const req = httpMock.expectOne(`${service['apiUrl']}/UpdateExpenses/${mockExpense.expenseID}?User ID=${service.UserID}`);
//     expect(req.request.method).toBe('PUT');
//     req.flush(mockResponse);
//   });

//   it('should handle error when updating an expense', () => {
//     const mockExpense: ExpensesEntity = { expenseID: 1, amount: 100, description: 'Updated Expense', date: new Date(), categoryID: 1, receiptUrl: '', expenseType: 'One-time', isRecurring: false, userID: 1 };

//     service.updateExpense(mockExpense).subscribe(
//       () => fail('expected an error, not an expense'),
//       error => {
//         expect(error).toBeDefined();
//       }
//     );

//     const req = httpMock.expectOne(`${service['apiUrl']}/UpdateExpenses/${mockExpense.expenseID}?User ID=${service.UserID}`);
//     expect(req.request.method).toBe('PUT');
//     req.flush('Error updating expense', { status: 500, statusText: 'Server Error' });
//   });

//   it('should delete an expense', () => {
//     const expenseId = 1;
//     const mockResponse = { status: true, data: 'Expense deleted successfully' };

//     service.deleteExpense(expenseId).subscribe(response => {
//       expect(response).toEqual(mockResponse);
//     });

//     const req = httpMock.expectOne(`${service['apiUrl']}/DeleteExpenses/${expenseId}`);
//     expect(req.request.method).toBe('DELETE');
//     req.flush(mockResponse);
//   });

//   it('should handle error when deleting an expense', () => {
//     const expenseId = 1;

//     service.deleteExpense(expenseId).subscribe(
//       () => fail('expected an error, not an expense'),
//       error => {
//         expect(error).toBeDefined();
//       }
//     );

//     const req = httpMock.expectOne(`${service['apiUrl']}/DeleteExpenses/${expenseId}`);
//     expect(req.request.method).toBe('DELETE');
//     req.flush('Error deleting expense', { status: 500, statusText: 'Server Error' });
//   });

//   it('should get categories', () => {
//     const mockCategories = [
//       { categoryID: 1, name: 'Food', isActive: true, createdOn: new Date(), modifiedOn: new Date() },
//       { categoryID: 2, name: 'Housing', isActive: true, createdOn: new Date(), modifiedOn: new Date() },
//     ];

//     service.getCategories().subscribe(categories => {
//       expect(categories).toEqual(mockCategories);
//     });

//     const req = httpMock.expectOne('https://localhost:7154/GetCategories');
//     expect(req.request.method).toBe('GET');
//     req.flush(mockCategories);
//   });

//   it('should handle error when getting categories', () => {
//     service.getCategories().subscribe(
//       () => fail('expected an error, not categories'),
//       error => {
//         expect(error).toBeDefined();
//       }
//     );

//     const req = httpMock.expectOne('https://localhost:7154/GetCategories');
//     expect(req.request.method).toBe('GET');
//     req.flush('Error loading categories', { status: 500, statusText: 'Server Error' });
//   });
// });