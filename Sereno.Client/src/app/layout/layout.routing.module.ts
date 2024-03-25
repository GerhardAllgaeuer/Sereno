// layout-routing.module.ts
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainWindowComponent } from './main-window/main-window.component';
import { ContactListComponent } from '../pages/contact-list/contact-list.component';
import { InboxViewComponent } from '../pages/inbox-view/inbox-view.component';
import { MasterComponent } from '../master/master.component'
import { AuthGuard } from '../shared/guards/auth.guard';

const layoutRoutes: Routes = [
  {
    path: '',
    component: MainWindowComponent,
    children: [
      {
        path: 'contacts',
        component: ContactListComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'inbox',
        component: InboxViewComponent,
        canActivate: [AuthGuard],
      },
      {
        path: 'master',
        component: MasterComponent,
        canActivate: [AuthGuard],
      }
    ]
  }
];


@NgModule({
  imports: [RouterModule.forChild(layoutRoutes)],
  exports: [RouterModule]
})
export class LayoutRoutingModule { }
