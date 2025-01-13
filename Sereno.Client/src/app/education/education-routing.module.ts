import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainWindowComponent } from '../layout/main-window/main-window.component';
import { SchedulerComponent } from './scheduler/scheduler.component';
import { SchedulerBrynthumComponent } from './scheduler-brynthum/scheduler-brynthum.component';


const layoutRoutes: Routes = [
  {
    path: '',
    component: MainWindowComponent,
    children: [
      {
        path: 'scheduler',
        component: SchedulerComponent,
      },
      {
        path: 'scheduler-brynthum',
        component: SchedulerBrynthumComponent,
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(layoutRoutes)],
  exports: [RouterModule]
})
export class EducationRoutingModule { }
