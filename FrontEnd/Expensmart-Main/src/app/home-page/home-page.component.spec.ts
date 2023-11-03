import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';

import { HomePageComponent } from './home-page.component';
import { BrowserAnimationsModule, NoopAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';
describe('HomePageComponent', () => {
  let component: HomePageComponent;
  let fixture: ComponentFixture<HomePageComponent>;
  let router: Router;

  beforeEach(async () => {
    const routerMock = {
      navigate: jasmine.createSpy('navigate')
    };
    await TestBed.configureTestingModule({
      declarations: [HomePageComponent],
      imports: [BrowserAnimationsModule,NoopAnimationsModule,HttpClientTestingModule,RouterTestingModule],
      providers: [{ provide: Router, useValue: routerMock }]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HomePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  it('should run previous slide', () => {
    component.prevSlide();
  });
  it('should run next Slide', () => {
    component.nextSlide();
  });
  it('should run set hovered success', () => {
    component.setHovered(true);
  });
  it('should run set hovered fail', () => {
    component.setHovered(false);
  });
  it('should signup', () => {
    component.signUp();
  });
  it('should login', () => {
    component.login();
  });
  it('should return set Interval for next slide', fakeAsync(() => {
    spyOn(component, "nextSlide");
    component.startAutoRotate();
    tick(3500);
    expect(component.nextSlide).toHaveBeenCalled();
  }));
});
