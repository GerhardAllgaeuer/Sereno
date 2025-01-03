import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainWindowComponent } from '../layout/main-window/main-window.component';
import { SchedulerComponent } from './scheduler/scheduler.component';


const layoutRoutes: Routes = [
  {
    path: '',
    component: MainWindowComponent,
    children: [
      {
        path: 'scheduler',
        component: SchedulerComponent,
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(layoutRoutes)],
  exports: [RouterModule]
})
export class EducationRoutingModule { }
