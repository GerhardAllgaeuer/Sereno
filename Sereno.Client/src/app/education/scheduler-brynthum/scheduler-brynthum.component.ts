import { Component } from '@angular/core';
import { BryntumSchedulerProModule } from '@bryntum/schedulerpro-angular';
import { BryntumSchedulerProProps, BryntumSchedulerProProjectModelProps } from '@bryntum/schedulerpro-angular';
import { LocaleManager } from '@bryntum/schedulerpro';

@Component({
  selector: 'app-scheduler-brynthum',
  imports: [
    BryntumSchedulerProModule
  ],
  templateUrl: './scheduler-brynthum.component.html',
  styleUrls: ['./scheduler-brynthum.component.scss']
})


export class SchedulerBrynthumComponent {

  ngOnInit(): void {
    LocaleManager.locale = 'Ru';
  }


  resources = [
    { id: 1, name: 'Beate Steixner-Bartl' },
    { id: 2, name: 'Christine Huber' },
    { id: 3, name: 'Dietmar Illmer' },
  ];

  events = [
    { id: 1, resourceId: 1, startDate: new Date(2025, 0, 1), endDate: new Date(2025, 0, 2), name: 'Grundzüge der Kommunikation und Konfliktbewältigung' },
    { id: 2, resourceId: 1, startDate: new Date(2025, 0, 1), endDate: new Date(2025, 0, 2), name: 'Grundzüge der Kommunikation und Konfliktbewältigung' },
    { id: 3, resourceId: 2, startDate: new Date(2025, 0, 2), endDate: new Date(2025, 0, 4), name: '' },
    { id: 4, resourceId: 3, startDate: new Date(2025, 0, 3), endDate: new Date(2025, 0, 4), name: 'Handlungsfeld der Praxisanleitung - Spannungsfeld und Rollen' },
  ];

  assignments = [
    { id: 1, eventId: 1, resourceId: 1 },
    { id: 2, eventId: 2, resourceId: 1 },
    { id: 3, eventId: 3, resourceId: 2 },
    { id: 4, eventId: 4, resourceId: 3 },
  ];

  dependencies = [
    {  }
  ];

  projectProps: BryntumSchedulerProProjectModelProps = {
    // Projektkonfiguration, falls benötigt
  };

  schedulerProProps: BryntumSchedulerProProps = {
    columns: [
      { text: 'Name', field: 'name', width: 160 }
    ],
    startDate: new Date(2025, 0, 1),
    endDate: new Date(2025, 0, 31)
  };
}
