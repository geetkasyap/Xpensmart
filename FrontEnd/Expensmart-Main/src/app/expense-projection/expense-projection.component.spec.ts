import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpenseProjectionComponent } from './expense-projection.component';

describe('ExpenseProjectionComponent', () => {
  let component: ExpenseProjectionComponent;
  let fixture: ComponentFixture<ExpenseProjectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ExpenseProjectionComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExpenseProjectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
