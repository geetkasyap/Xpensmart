import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedAccountFormComponent } from './shared-account-form.component';

describe('SharedAccountFormComponent', () => {
  let component: SharedAccountFormComponent;
  let fixture: ComponentFixture<SharedAccountFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SharedAccountFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SharedAccountFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
