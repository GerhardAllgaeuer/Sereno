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
    { path: '/contacts',      title: 'Kontakte',          icon:'nc-bank',       class: '' },
    { path: '/inbox',         title: 'Eingang',           icon:'nc-diamond',    class: '' },
];

@Component({
    moduleId: module.id,
    imports: [
        RouterModule, CommonModule
      ],
        standalone: true,
    selector: 'sidebar-cmp',
    templateUrl: 'side-panel.component.html',
})

export class SidePanelComponent implements OnInit {
    public menuItems: any[];
    ngOnInit() {
        this.menuItems = ROUTES.filter(menuItem => menuItem);
    }
}
