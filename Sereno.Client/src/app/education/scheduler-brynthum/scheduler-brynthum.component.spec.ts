import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchedulerBrynthumComponent } from './scheduler-brynthum.component';

describe('SchedulerBrynthumComponent', () => {
  let component: SchedulerBrynthumComponent;
  let fixture: ComponentFixture<SchedulerBrynthumComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SchedulerBrynthumComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SchedulerBrynthumComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
