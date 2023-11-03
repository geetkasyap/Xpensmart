import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BulkImportService {
  private readonly API_URL = 'https://localhost:7154/Expenses/bulk-import';

  constructor(private http: HttpClient) {}

  async uploadFile(file: File, userId: number): Promise<any> {
    const formData = new FormData();
    formData.append('file', file);
    try {
      return await firstValueFrom(this.http.post(`${this.API_URL}/${userId}`, formData));
    } catch (error: any) {
      throw new Error(this.getErrorMessage(error));
    }
  }

  private getErrorMessage(error: any): string {
    return error?.error?.message || 'An unexpected error occurred.';
  }
}