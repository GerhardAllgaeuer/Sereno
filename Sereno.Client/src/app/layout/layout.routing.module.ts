import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainWindowComponent } from './main-window/main-window.component';
import { ContactListComponent } from '../pages/contact-list/contact-list.component';
import { InboxViewComponent } from '../pages/inbox-view/inbox-view.component';
import { MasterComponent } from '../master/master.component'
import { DenyGuestsGuard } from '../shared/guards/auth.guard';
import { AdminGuard } from '../shared/guards/admin.guard';

const layoutRoutes: Routes = [
  {
    path: '',
    component: MainWindowComponent,
    children: [
      {
        path: 'contacts',
        component: ContactListComponent,
        canActivate: [DenyGuestsGuard],
      },
      {
        path: 'inbox',
        component: InboxViewComponent,
        canActivate: [DenyGuestsGuard],
      },
      {
        path: 'master',
        component: MasterComponent,
        canActivate: [DenyGuestsGuard],
      }
    ]
  }
];


@NgModule({
  imports: [RouterModule.forChild(layoutRoutes)],
  exports: [RouterModule]
})
export class LayoutRoutingModule { }
