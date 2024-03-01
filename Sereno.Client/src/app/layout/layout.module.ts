import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MasterComponent } from '../master/master.component';
import { MainWindowComponent } from './main-window/main-window.component';
import { LayoutRoutingModule } from './layout.routing.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [
  ],
  imports: [
    MasterComponent,    
    MainWindowComponent,
    LayoutRoutingModule,
    CommonModule,
    RouterModule,
  ]

})
export class LayoutModule { }
