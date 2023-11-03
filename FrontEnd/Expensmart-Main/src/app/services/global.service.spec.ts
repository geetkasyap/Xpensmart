import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { GlobalService } from './global.service';
import { of, throwError } from 'rxjs';

describe('GlobalService', () => {
  let service: GlobalService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [GlobalService],
    });
    service = TestBed.inject(GlobalService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should add an expense', () => {
    const mockExpenseData = { amount: 100, description: 'Test Expense' };
    const mockResponse = { status: true, data: 'Expense added successfully' };

    service.ExpenseFormAddExpense(mockExpenseData).subscribe(response => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(`${service['baseUrl']}/Expenses/AddExpenses`);
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
  });

  it('should handle error when adding an expense', () => {
    const mockExpenseData = { amount: 100, description: 'Test Expense' };
    const mockErrorResponse = { message: 'Error adding expense' };

    service.ExpenseFormAddExpense(mockExpenseData).subscribe(
      () => fail('expected an error, not expenses'),
      error => {
      }
    );

    const req = httpMock.expectOne(`${service['baseUrl']}/Expenses/AddExpenses`);
    expect(req.request.method).toBe('POST');
    req.flush(mockErrorResponse, { status: 500, statusText: 'Server Error' });
  });

  it('should load categories', () => {
    const mockCategories = [
      { categoryID: 1, name: 'Food', isActive: true, createdOn: new Date(), modifiedOn: new Date() },
      { categoryID: 2, name: 'Housing', isActive: true, createdOn: new Date(), modifiedOn: new Date() },
    ];

    service.loadCategories().subscribe(categories => {
      expect(categories).toEqual(mockCategories);
    });

    const req = httpMock.expectOne(`${service['baseUrl']}/GetCategories`);
    expect(req.request.method).toBe('GET');
    req.flush(mockCategories);
  });

  it('should handle error when loading categories', () => {
    service.loadCategories().subscribe(
      () => fail('expected an error, not categories'),
      error => {
        expect(error).toBeDefined();
      }
    );

    const req = httpMock.expectOne(`${service['baseUrl']}/GetCategories`);
    expect(req.request.method).toBe('GET');
    req.flush('Error loading categories', { status: 500, statusText: 'Server Error' });
  });

  it('should convert currency', () => {
    const mockAmount = 100;
    const mockConvertedAmount = 75;

    service.convertCurrency(mockAmount, 'USD', 'INR').subscribe(amount => {
      expect(amount).toBe(mockConvertedAmount);
    });

    const req = httpMock.expectOne(`${service['baseUrl']}/Expenses/ConvertCurrency?amount=${mockAmount}&fromCurrency=USD&toCurrency=INR`);
    expect(req.request.method).toBe('GET');
    req.flush(mockConvertedAmount);
  });

  it('should handle error when converting currency', () => {
    service.convertCurrency(100, 'USD', 'INR').subscribe(
      () => fail('expected an error, not a currency conversion'),
      error => {
        expect(error).toBeDefined();
      }
    );

    const req = httpMock.expectOne(`${service['baseUrl']}/Expenses/ConvertCurrency?amount=100&fromCurrency=USD&toCurrency=INR`);
    expect(req.request.method).toBe('GET');
    req.flush('Error converting currency', { status: 500, statusText: 'Server Error' });
  });

  it('should upload a file', () => {
    const mockFilePath = 'uploads/test.jpg';
    const formData = new FormData();
    formData.append('receipt', new Blob());

    service.uploadFile(formData).subscribe(response => {
      expect(response.filePath).toBe(mockFilePath);
    });

    const req = httpMock.expectOne('http://localhost:8080/uploads');
    expect(req.request.method).toBe('POST');
    req.flush({ filePath: mockFilePath });
  });

  it('should handle error when uploading a file', () => {
    const formData = new FormData();
    formData.append('receipt', new Blob());

    service.uploadFile(formData).subscribe(
      () => fail('expected an error, not a file upload'),
      error => {
        expect(error).toBeDefined();
      }
    );

    const req = httpMock.expectOne('http://localhost:8080/uploads');
    expect(req.request.method).toBe('POST');
    req.flush('Error uploading file', { status: 500, statusText: 'Server Error' });
  });
});