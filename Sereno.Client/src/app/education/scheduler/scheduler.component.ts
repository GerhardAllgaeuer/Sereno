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

  prioritiesData = [
    {
      text: 'Low Priority',
      id: 1,
      color: '#1e90ff',
    }, {
      text: 'High Priority',
      id: 2,
      color: '#ff9747',
    },
  ];

  appointmentsData = [
    {
      text: 'Website Re-Design Plan',
      priorityId: 2,
      startDate: new Date(2025, 0, 3, 16, 30),
      endDate: new Date(2025, 0, 4, 18, 30),
    },
    {
      text: 'Website Re-Design Plan',
      priorityId: 2,
      startDate: new Date(2025, 0, 3, 16, 30),
      endDate: new Date(2025, 0, 4, 18, 30),
    },
    {
      text: 'Website Re-Design Plan',
      priorityId: 2,
      startDate: new Date(2025, 0, 3, 16, 30),
      endDate: new Date(2025, 0, 4, 18, 30),
    },
    {
      text: "Meeting",
      priorityId: 1,
      startDate: new Date(2025, 0, 3, 9, 0),
      endDate: new Date(2025, 0, 3, 10, 0),
    },
    {
      text: "Lunch",
      priorityId: 2,
      startDate: new Date(2025, 0, 3, 12, 0),
      endDate: new Date(2025, 0, 3, 13, 0),
    },
    {
      text: 'Website Re-Design Plan',
      priorityId: 2,
      startDate: new Date(2025, 0, 19, 16, 30),
      endDate: new Date(2025, 0, 19, 18, 30),
    },
    {
      text: 'Book Flights to San Fran for Sales Trip',
      priorityId: 1,
      startDate: new Date(2025, 0, 22, 17, 0),
      endDate: new Date(2025, 0, 22, 19, 0),
    },
    {
      text: 'Install New Router in Dev Room',
      priorityId: 1,
      startDate: new Date(2025, 0, 19, 20, 0),
      endDate: new Date(2025, 0, 19, 22, 30),
    },
    {
      text: 'Approve Personal Computer Upgrade Plan',
      priorityId: 2,
      startDate: new Date(2025, 0, 20, 17, 0),
      endDate: new Date(2025, 0, 20, 18, 0),
    },
    {
      text: 'Final Budget Review',
      priorityId: 2,
      startDate: new Date(2025, 0, 20, 19, 0),
      endDate: new Date(2025, 0, 20, 20, 35),
    },
    {
      text: 'New Brochures',
      priorityId: 2,
      startDate: new Date(2025, 0, 19, 20, 0),
      endDate: new Date(2025, 0, 19, 22, 15),
    },
    {
      text: 'Install New Database',
      priorityId: 1,
      startDate: new Date(2025, 0, 20, 16, 0),
      endDate: new Date(2025, 0, 20, 19, 15),
    },
    {
      text: 'Approve New Online Marketing Strategy',
      priorityId: 2,
      startDate: new Date(2025, 0, 21, 19, 0),
      endDate: new Date(2025, 0, 21, 21, 0),
    },
    {
      text: 'Upgrade Personal Computers',
      priorityId: 1,
      startDate: new Date(2025, 0, 19, 16, 0),
      endDate: new Date(2025, 0, 19, 18, 30),
    },
    {
      text: 'Prepare 2021 Marketing Plan',
      priorityId: 2,
      startDate: new Date(2025, 0, 22, 18, 0),
      endDate: new Date(2025, 0, 22, 20, 30),
    },
    {
      text: 'Brochure Design Review',
      priorityId: 1,
      startDate: new Date(2025, 0, 21, 18, 0),
      endDate: new Date(2025, 0, 21, 20, 30),
    },
    {
      text: 'Create Icons for Website',
      priorityId: 2,
      startDate: new Date(2025, 0, 23, 17, 0),
      endDate: new Date(2025, 0, 23, 18, 30),
    },
    {
      text: 'Upgrade Server Hardware',
      priorityId: 1,
      startDate: new Date(2025, 0, 23, 16, 0),
      endDate: new Date(2025, 0, 23, 22, 0),
    },
    {
      text: 'Submit New Website Design',
      priorityId: 2,
      startDate: new Date(2025, 0, 23, 23, 30),
      endDate: new Date(2025, 0, 24, 1, 0),
    },
    {
      text: 'Launch New Website',
      priorityId: 2,
      startDate: new Date(2025, 0, 23, 19, 20),
      endDate: new Date(2025, 0, 23, 21, 0),
    },
    {
      text: 'Google AdWords Strategy',
      priorityId: 1,
      startDate: new Date(2025, 0, 26, 16, 0),
      endDate: new Date(2025, 0, 26, 19, 0),
    },
    {
      text: 'Rollout of New Website and Marketing Brochures',
      priorityId: 1,
      startDate: new Date(2025, 0, 26, 20, 0),
      endDate: new Date(2025, 0, 26, 22, 30),
    },
    {
      text: 'Non-Compete Agreements',
      priorityId: 2,
      startDate: new Date(2025, 0, 27, 20, 0),
      endDate: new Date(2025, 0, 27, 22, 45),
    },
    {
      text: 'Approve Hiring of John Jeffers',
      priorityId: 2,
      startDate: new Date(2025, 0, 27, 16, 0),
      endDate: new Date(2025, 0, 27, 19, 0),
    },
    {
      text: 'Update NDA Agreement',
      priorityId: 1,
      startDate: new Date(2025, 0, 27, 18, 0),
      endDate: new Date(2025, 0, 27, 21, 15),
    },
    {
      text: 'Update Employee Files with New NDA',
      priorityId: 1,
      startDate: new Date(2025, 0, 30, 16, 0),
      endDate: new Date(2025, 0, 30, 18, 45),
    },
    {
      text: 'Submit Questions Regarding New NDA',
      priorityId: 1,
      startDate: new Date(2025, 0, 28, 17, 0),
      endDate: new Date(2025, 0, 28, 18, 30),
    },
    {
      text: 'Submit Signed NDA',
      priorityId: 1,
      startDate: new Date(2025, 0, 28, 20, 0),
      endDate: new Date(2025, 0, 28, 22, 0),
    },
    {
      text: 'Review Revenue Projections',
      priorityId: 2,
      startDate: new Date(2025, 0, 28, 18, 0),
      endDate: new Date(2025, 0, 28, 21, 0),
    },
    {
      text: 'Comment on Revenue Projections',
      priorityId: 2,
      startDate: new Date(2025, 0, 26, 17, 0),
      endDate: new Date(2025, 0, 26, 20, 0),
    },
    {
      text: 'Provide New Health Insurance Docs',
      priorityId: 2,
      startDate: new Date(2025, 0, 30, 19, 0),
      endDate: new Date(2025, 0, 30, 22, 0),
    },
    {
      text: 'Review Changes to Health Insurance Coverage',
      priorityId: 2,
      startDate: new Date(2025, 0, 29, 16, 0),
      endDate: new Date(2025, 0, 29, 20, 0),
    },
    {
      text: 'Review Training Course for any Omissions',
      priorityId: 1,
      startDate: new Date(2025, 0, 29, 18, 0),
      endDate: new Date(2025, 0, 29, 21, 0),
    },
  ];
}
