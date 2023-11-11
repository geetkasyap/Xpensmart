import { Component, ElementRef, ViewChild } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ToastrService } from 'ngx-toastr';
import { BulkImportService } from '../services/bulk-import.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-bulk-expenses',
  standalone: false,
  templateUrl: './bulk-expenses.component.html',
  styleUrls: ['./bulk-expenses.component.scss'],
})
export class BulkExpensesComponent {
  selectedFile: File | null = null;
  isDragging = false;
  isUploading = false;
  LocUserID = localStorage.getItem('userId') as unknown as number;

  @ViewChild('fileInput') fileInput!: ElementRef;

  constructor(
    private bulkImportService: BulkImportService,
    private snackBar: MatSnackBar,
    private toastr: ToastrService,
    private router: Router
  ) {}

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      this.handleFile(files[0]);
    }
  }

  onFileSelected(event: any): void {
    const file = event.target.files[0];
    this.handleFile(file);
  }

  handleFile(file: File): void {
    if (file.type !== 'text/csv') {
      this.toastr.error('Please upload a CSV file', 'Error');
      return;
    }
    this.selectedFile = file;
    this.isDragging = false;
  }

  async uploadFile(): Promise<void> {
    if (!this.selectedFile) return;
    this.isUploading = true;
    try {
      const result = await this.bulkImportService.uploadFile(this.selectedFile, this.LocUserID);
      if(result.status===true)
      {
        this.toastr.success(result.data,"Success");
      }
      else
      {
        this.toastr.error(result.data,"Error");
      }
      this.clearFileInput();
    } catch (error: any) {
      this.toastr.error(error.message || 'An error occurred during file upload.', 'Error');
    } finally {
      this.isUploading = false;
    }
  }

  clearFileInput(): void {
    this.selectedFile = null;
    if (this.fileInput && this.fileInput.nativeElement) {
      this.fileInput.nativeElement.value = '';
    }
  }
}