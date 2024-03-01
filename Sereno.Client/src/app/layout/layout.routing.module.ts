// layout-routing.module.ts
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MainWindowComponent } from './main-window/main-window.component';
import { ContactListComponent } from '../pages/contact-list/contact-list.component';
import { InboxViewComponent } from '../pages/inbox-view/inbox-view.component';
import { MasterComponent } from '../master/master.component'

const layoutRoutes: Routes = [
  {
    path: '',
    component: MainWindowComponent,
    children: [
      {
        path: 'contacts',
        component: ContactListComponent,
      },
      {
        path: 'inbox',
        component: InboxViewComponent,
      },
      {
        path: 'master',
        component: MasterComponent,
      }
    ]
  }

];

@NgModule({
  imports: [RouterModule.forChild(layoutRoutes)],
  exports: [RouterModule]
})
export class LayoutRoutingModule { }
