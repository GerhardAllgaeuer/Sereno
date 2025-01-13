import { Component } from '@angular/core';
import { BryntumSchedulerProModule } from '@bryntum/schedulerpro-angular';
import { BryntumSchedulerProProps, BryntumSchedulerProProjectModelProps } from '@bryntum/schedulerpro-angular';

@Component({
  selector: 'app-scheduler-brynthum',
  imports: [
    BryntumSchedulerProModule
  ],
  templateUrl: './scheduler-brynthum.component.html',
  styleUrls: ['./scheduler-brynthum.component.scss']
})
export class SchedulerBrynthumComponent {
  resources = [
    { id: 1, name: 'Resource 1' },
    { id: 2, name: 'Resource 2' }
  ];

  events = [
    { id: 1, resourceId: 1, startDate: new Date(2022, 0, 1), endDate: new Date(2022, 0, 2), name: 'Event 1' },
    { id: 2, resourceId: 2, startDate: new Date(2022, 0, 3), endDate: new Date(2022, 0, 4), name: 'Event 2' }
  ];

  assignments = [
    { id: 1, eventId: 1, resourceId: 1 },
    { id: 2, eventId: 2, resourceId: 2 }
  ];

  dependencies = [
    { id: 1, from: 1, to: 2 }
  ];

  projectProps: BryntumSchedulerProProjectModelProps = {
    // Projektkonfiguration, falls benötigt
  };

  schedulerProProps: BryntumSchedulerProProps = {
    columns: [
      { text: 'Name', field: 'name', width: 160 }
    ],
    startDate: new Date(2022, 0, 1),
    endDate: new Date(2022, 0, 10)
  };
}
