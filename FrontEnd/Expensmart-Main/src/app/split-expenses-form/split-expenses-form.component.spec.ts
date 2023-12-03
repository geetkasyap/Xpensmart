import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SplitExpensesFormComponent } from './split-expenses-form.component';

describe('SplitExpensesFormComponent', () => {
  let component: SplitExpensesFormComponent;
  let fixture: ComponentFixture<SplitExpensesFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SplitExpensesFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SplitExpensesFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
