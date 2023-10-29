
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ValidationsServiceService {
  public messages: any;
  formErrorMszes: any;

  constructor(public http: HttpClient) { }

  initialize() {
    return this.http.get('./assets/Validation.json')
      .pipe(tap((response) => {
        this.messages = response;
      }));
  }

  getErrorMessage() {
    return this.messages['formIdentifiers']['validationMessages'];
  }

  validateControl(form: FormGroup, controlName: string) {
    const control = form.get(controlName);
    const errors = control?.errors;

    if (control && errors) {
      this.formErrorMszes = this.getErrorMessage();

      if (control.hasError('required')) {
        return this.formErrorMszes['required'];
      }
      if (control.hasError('minlength')) {
        const length = errors['minlength']['requiredLength'];
        return this.formErrorMszes['minlength'].replace('{0}', length);
      }
      if (control.hasError('maxlength')) {
        const length = errors['maxlength']['requiredLength'];
        return this.formErrorMszes['maxlength'].replace('{0}', length);
      }
      if (control.hasError('pattern')) {
        return this.formErrorMszes['pattern'];
      }
      if (control.hasError('min')) {
        const minValue = errors['min']['min'];
        return this.formErrorMszes['min'].replace('{0}', minValue);
      }
      if (control.hasError('max')) {
        const maxValue = errors['max']['max'];
        return this.formErrorMszes['max'].replace('{9999999999999999.99}', maxValue);
      }
      if (control.hasError('alphaPattern')) {
        return this.formErrorMszes['alphaPattern'];
      }
    }
    return null;
  }
}