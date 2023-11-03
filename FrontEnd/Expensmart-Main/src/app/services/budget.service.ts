import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BudgetsEntity } from '../models/budget-entity-model';

@Injectable({
  providedIn: 'root'
})
export class BudgetService {
  private apiUrl = 'https://localhost:7154/Budget';
  constructor(private http: HttpClient) { }

    
}
