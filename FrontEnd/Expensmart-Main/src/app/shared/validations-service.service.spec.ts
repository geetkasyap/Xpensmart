import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ValidationsServiceService } from './validations-service.service';

describe('ValidationsServiceService', () => {
  let service: ValidationsServiceService;
  let httpMock: HttpTestingController;
  let formBuilder: FormBuilder;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ValidationsServiceService, FormBuilder]
    });
    service = TestBed.inject(ValidationsServiceService);
    httpMock = TestBed.inject(HttpTestingController);
    formBuilder = TestBed.inject(FormBuilder);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should initialize messages from JSON file', () => {
    const mockResponse = {
      formIdentifiers: {
        validationMessages: {
          required: 'This field is required',
          minlength: 'Minimum length is {0}',
          maxlength: 'Maximum length is {0}',
          pattern: 'Invalid format',
          min: 'Minimum value is {0}',
          max: 'Maximum value is {9999999999999999.99}',
          alphaPattern: 'Only alphabets are allowed'
        }
      }
    };

    service.initialize().subscribe(() => {
      expect(service.messages).toEqual(mockResponse);
    });

    const req = httpMock.expectOne('./assets/Validation.json');
    expect(req.request.method).toBe('GET');
    req.flush(mockResponse);
  });

  it('should return error messages', () => {
    const mockResponse = {
      formIdentifiers: {
        validationMessages: {
          required: 'This field is required',
          minlength: 'Minimum length is {0}',
          maxlength: 'Maximum length is {0}',
          pattern: 'Invalid format',
          min: 'Minimum value is {0}',
          max: 'Maximum value is {9999999999999999.99}',
          alphaPattern: 'Only alphabets are allowed'
        }
      }
    };

    service.messages = mockResponse;

    const errorMessages = service.getErrorMessage();
    expect(errorMessages).toEqual(mockResponse.formIdentifiers.validationMessages);
  });

  it('should validate control and return error messages', () => {
    const form: FormGroup = formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(3)]]
    });

    service.messages = {
      formIdentifiers: {
        validationMessages: {
          required: 'This field is required',
          minlength: 'Minimum length is {0}',
          maxlength: 'Maximum length is {0}',
          pattern: 'Invalid format',
          min: 'Minimum value is {0}',
          max: 'Maximum value is {9999999999999999.99}',
          alphaPattern: 'Only alphabets are allowed'
        }
      }
    };

    form.controls['name'].setValue('');
    let errorMessage = service.validateControl(form, 'name');
    expect(errorMessage).toBe('This field is required');

    form.controls['name'].setValue('ab');
    errorMessage = service.validateControl(form, 'name');
    expect(errorMessage).toBe('Minimum length is 3');

    form.controls['name'].setValue('abc');
    errorMessage = service.validateControl(form, 'name');
    expect(errorMessage).toBeNull();
  });

  it('should return appropriate error messages for other validations', () => {
    const form: FormGroup = formBuilder.group({
      age: ['', [Validators.min(18), Validators.max(65)]]
    });
  
    service.messages = {
      formIdentifiers: {
        validationMessages: {
          required: 'This field is required',
          minlength: 'Minimum length is {0}',
          maxlength: 'Maximum length is {0}',
          pattern: 'Invalid format',
          min: 'Minimum value is {0}',
          max: 'Maximum value is {9999999999999999.99}',
          alphaPattern: 'Only alphabets are allowed'
        }
      }
    };
  
    form.controls['age'].setValue(16);
    let errorMessage = service.validateControl(form, 'age');
    expect(errorMessage).toBe('Minimum value is 18');
  
    form.controls['age'].setValue(70);
    errorMessage = service.validateControl(form, 'age');
    expect(errorMessage).toBe('Maximum value is 65');
  
    form.controls['age'].setValue(30);
    errorMessage = service.validateControl(form, 'age');
    expect(errorMessage).toBeNull();
  });

  it('should return error message for pattern validation', () => {
    const form: FormGroup = formBuilder.group({
      username: ['', [Validators.pattern('^[a-zA-Z]*$')]]
    });

    service.messages = {
      formIdentifiers: {
        validationMessages: {
          required: 'This field is required',
          minlength: 'Minimum length is {0}',
          maxlength: 'Maximum length is {0}',
          pattern: 'Invalid format',
          min: 'Minimum value is {0}',
          max: 'Maximum value is {9999999999999999.99}',
          alphaPattern: 'Only alphabets are allowed'
        }
      }
    };

    form.controls['username'].setValue('user123');
    let errorMessage = service.validateControl(form, 'username');
    expect(errorMessage).toBe('Invalid format');

    form.controls['username'].setValue('username');
    errorMessage = service.validateControl(form, 'username');
    expect(errorMessage).toBeNull();
  });
  it('should return error message for maxlength validation', () => {
    const form: FormGroup = formBuilder.group({
      name: ['', [Validators.maxLength(5)]]
    });
  
    service.messages = {
      formIdentifiers: {
        validationMessages: {
          required: 'This field is required',
          minlength: 'Minimum length is {0}',
          maxlength: 'Maximum length is {0}',
          pattern: 'Invalid format',
          min: 'Minimum value is {0}',
          max: 'Maximum value is {9999999999999999.99}',
          alphaPattern: 'Only alphabets are allowed'
        }
      }
    };
  
    form.controls['name'].setValue('abcdef'); 
    const errorMessage = service.validateControl(form, 'name');
    expect(errorMessage).toBe('Maximum length is 5');
  });
  
  it('should return error message for max validation', () => {
    const form: FormGroup = formBuilder.group({
      age: ['', [Validators.max(65)]]
    });
  
    service.messages = {
      formIdentifiers: {
        validationMessages: {
          required: 'This field is required',
          minlength: 'Minimum length is {0}',
          maxlength: 'Maximum length is {0}',
          pattern: 'Invalid format',
          min: 'Minimum value is {0}',
          max: 'Maximum value is {9999999999999999.99}',
          alphaPattern: 'Only alphabets are allowed'
        }
      }
    };
  
    form.controls['age'].setValue(70);
    const errorMessage = service.validateControl(form, 'age');
    expect(errorMessage).toBe('Maximum value is 65');
  });
  
  it('should return error message for alphaPattern validation', () => {
    const form: FormGroup = formBuilder.group({
      username: ['', [Validators.pattern('^[a-zA-Z]*$')]]
    });
  
    service.messages = {
      formIdentifiers: {
        validationMessages: {
          required: 'This field is required',
          minlength: 'Minimum length is {0}',
          maxlength: 'Maximum length is {0}',
          pattern: 'Invalid format',
          min: 'Minimum value is {0}',
          max: 'Maximum value is {9999999999999999.99}',
          alphaPattern: 'Only alphabets are allowed'
        }
      }
    };
  
    form.controls['username'].setValue('user123');
    const errorMessage = service.validateControl(form, 'username');
    expect(errorMessage).toBe('Invalid format');
  });
});