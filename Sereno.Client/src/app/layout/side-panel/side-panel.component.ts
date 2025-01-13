import { Component, OnInit } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';

export interface RouteInfo {
  path: string;
  title: string;
  icon: string;
  class: string;
}

export const ROUTES: RouteInfo[] = [
  { path: '/education/scheduler-brynthum', title: 'Planer Brynthum', icon: 'icon-side-incoming', class: '' },
  { path: '/education/scheduler', title: 'Planer', icon: 'icon-side-incoming', class: '' },
  { path: '/contacts', title: 'Kontakte', icon: 'icon-side-incoming', class: '' },
  { path: '/inbox', title: 'Eingang', icon: 'icon-side-new', class: '' },
];

@Component({
    moduleId: module.id,
    imports: [
        RouterModule, CommonModule
    ],
    selector: 'sidebar-cmp',
    templateUrl: 'side-panel.component.html'
})

export class SidePanelComponent implements OnInit {
  public menuItems: any[];
  ngOnInit() {
    this.menuItems = ROUTES.filter(menuItem => menuItem);
  }
}
