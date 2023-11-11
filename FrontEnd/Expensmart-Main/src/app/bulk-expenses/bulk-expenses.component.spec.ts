import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BulkExpensesComponent } from './bulk-expenses.component';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { ConfirmationDialogComponent } from '../confirmation-dialog/confirmation-dialog.component';
import { throwError } from 'rxjs';
describe('BulkExpensesComponent', () => {
  let component: BulkExpensesComponent;
  let fixture: ComponentFixture<BulkExpensesComponent>;
  let toastrServiceSpy: jasmine.SpyObj<ToastrService>;

  beforeEach(async () => {
    const toastrSpy = jasmine.createSpyObj('ToastrService', ['success', 'error']);
    await TestBed.configureTestingModule({
      declarations: [BulkExpensesComponent],
      imports: [HttpClientTestingModule,
              RouterTestingModule,
              ToastrModule.forRoot(),],
      providers: [
              { provide: ToastrService, useValue: toastrSpy },
                  ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(BulkExpensesComponent);
    component = fixture.componentInstance;
    toastrServiceSpy = TestBed.inject(ToastrService) as jasmine.SpyObj<ToastrService>;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

    it('should run event meathod onDragLeave', () => {
      component.onDragLeave(event as any);
      expect(component.isDragging).toBe(false);
    });
    it('should run event meathod onDrop', () => {
      component.onDrop(event as any);
    });
    it('should run event meathod onFileSelected', () => {
      const mockFile = new File([''], 'test.csv', { type: 'text/csv' });
      const event = { target: { files: [mockFile] } };
      component.onFileSelected(event as any);
    });
    it('should run event meathod handleFile', () => {
      const mockFile = new File([''], 'test.csv', { type: 'text/csv' });
      component.handleFile(mockFile);
    });
    it('should run event meathod handleFile failed', () => {
      const mockFile = new File([''], 'test.jpg', { type: 'jpg' });
      component.handleFile(mockFile);
    });
    it('should upload files and return null', () => {
      component.selectedFile = null;
      component.uploadFile();
    });
    it('should upload files and return success', () => {
      const mockFile = new File([''], 'test.csv', { type: 'text/csv' });
      component.selectedFile = mockFile;
      component.uploadFile();
    });
    it('should clear file input meathod', () => {
      component.clearFileInput();
      expect(component.selectedFile).toBeNull();
    });
    it('should run event meathod OnDragOver', () => {
      component.onDragOver(event as DragEvent);
      expect(component.isDragging).toBe(true);
    });
    it('should call upload file meathod and return status', () => {
      const mockFile = new File([''], 'test.csv', { type: 'text/csv' });
      component.selectedFile = mockFile;
    spyOn(component['bulkImportService'], "uploadFile").and.returnValue(Promise.resolve({ status: true, data: 'Success' }));
    component.uploadFile();
      expect(component.isUploading ).toBe(true);
    });
    it('should call upload file meathod and return status failed', () => {
      const mockFile = new File([''], 'test.csv', { type: 'text/csv' });
      component.selectedFile = mockFile;
    spyOn(component['bulkImportService'], "uploadFile").and.returnValue(Promise.resolve({ status: false, data: 'Error' }));
    component.uploadFile();
    expect(component.isUploading ).toBe(true);
  });
  it('should call upload file meathod and return error ', () => {
    const mockFile = new File([''], 'test.csv', { type: 'text/csv' });
    component.selectedFile = mockFile;
  spyOn(component['bulkImportService'], "uploadFile").and.returnValue(Promise.reject({ message: 'Error' })); 
  component.uploadFile();
});

});
