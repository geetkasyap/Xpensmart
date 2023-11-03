
import { Component, OnInit, OnDestroy } from '@angular/core';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { Router } from '@angular/router';
interface CarouselItem {
  title: string;
  description: string;
  hoverText: string;
  imagePath: string;
}
@Component({
  selector: 'app-home-page',
  standalone: false,
  
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.scss',
animations: [
  trigger('cardAnimation', [
    state('active', style({
      transform: 'scale(1.1) translateZ(200px)',
      zIndex: 20,
      filter: 'brightness(1)'
    })),
    state('inactive', style({
      transform: '{{transform}}',
      zIndex: 10,
      filter: 'brightness(0.7)'
    }), { params: { transform: 'rotateY(0deg) translateZ(500px)' } }),
    transition('* => *', animate('800ms cubic-bezier(0.4, 0, 0.2, 1)'))
  ])
]
})
export class HomePageComponent implements OnInit, OnDestroy{
constructor(private router: Router) { }
  activeIndex = 0;
  isHovered = false;
  private autoRotateInterval: any;

  carouselItems: CarouselItem[] = [
    {
      title: "Expense Tracking",
      description: "Track your daily expenses with ease",
      hoverText: "Get detailed insights into your spending patterns with our advanced tracking features",
      imagePath: "/assets/Dashboard.png" 
    },
    {
      title: "Projection based on Income",
      description: "Generate a Projection and assess your Budget in every Category",
      hoverText: "Access detailed financial budgets in every category based on 50 , 30 , 20 rule",
      imagePath: "/assets/Projection.png"
    },
    {
      title: "Grouped Expenses",
      description: "Group Expenses ",
      hoverText: "Create groups and contribute your Expense with every person in the group",
      imagePath: "/assets/Groups.png"
    },
    {
      title: "Financial Reports",
      description: "What have you done this month?",
      hoverText: "Check all your detailed reports in the reports page",
      imagePath: "/assets/Reports.png"
    },
    {
      title: "Analyze Expenses",
      description: "Look keen into it",
      hoverText: "You can analyze your monthly expense and can view previous month expenses by setting budgets and can download your reports too",
      imagePath: "/assets/Reports2.png"
    },
  ];

  ngOnInit() {
    this.startAutoRotate();
  }

  ngOnDestroy() {
    this.stopAutoRotate();
  }

  startAutoRotate() {
    if (!this.isHovered) {
      this.autoRotateInterval = setInterval(() => {
        if (!this.isHovered) {
          this.nextSlide();
        }
      }, 3500);
    }
  }

  stopAutoRotate() {
    if (this.autoRotateInterval) {
      clearInterval(this.autoRotateInterval);
    }
  }
  getRotationDegree(index: number): number {
    const totalItems = this.carouselItems.length;
    const angleStep = 360 / totalItems;
    const degree = ((index - this.activeIndex) * angleStep) % 360;
    return degree > 180 ? degree - 360 : degree;
  }

  getTransform(index: number): string {
    const degree = this.getRotationDegree(index);
    const radius = 800; 
    const x = Math.sin(degree * Math.PI / 180) * radius;
    const z = Math.cos(degree * Math.PI / 180) * radius;
    return `translate3d(${x}px, 0, ${z}px) rotateY(${degree}deg)`;
  }

  getAnimationState(index: number) {
    return {
      value: index === this.activeIndex ? 'active' : 'inactive',
      params: { transform: this.getTransform(index) }
    };
  }

  prevSlide() {
    this.activeIndex = (this.activeIndex - 1 + this.carouselItems.length) % this.carouselItems.length;
  }

  nextSlide() {
    this.activeIndex = (this.activeIndex + 1) % this.carouselItems.length;
  }

  setHovered(value: boolean) {
    this.isHovered = value;
    if (value) {
      this.stopAutoRotate();
    } else {
      this.startAutoRotate();
    }
  }

  signUp(): void {
    this.router.navigate(['/SignUp']);
  }

  login(): void {
    this.router.navigate(['/Login']);
  }

}
