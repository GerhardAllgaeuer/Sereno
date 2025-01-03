import { Component } from '@angular/core';
import { DxSchedulerModule, DxCalendarModule } from 'devextreme-angular';

@Component({
  selector: 'app-scheduler',
  imports: [
    DxSchedulerModule,
  ],
  templateUrl: './scheduler.component.html',
  styleUrl: './scheduler.component.scss'
})
export class SchedulerComponent {
  schedulerData = [
    {
      text: "Meeting",
      startDate: new Date(2025, 0, 3, 9, 0),
      endDate: new Date(2025, 0, 3, 10, 0),
    },
    {
      text: "Lunch",
      startDate: new Date(2025, 0, 3, 12, 0),
      endDate: new Date(2025, 0, 3, 13, 0),
    },
  ];
}
