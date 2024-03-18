import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  moduleId: module.id,
  standalone: true,
  selector: 'fixedplugin-cmp',
  imports: [CommonModule],
  templateUrl: 'fixedplugin.component.html'
})

export class FixedPluginComponent implements OnInit {

  public sidebarColor: string = "white";
  public sidebarActiveColor: string = "danger";

  public state: boolean = true;

  changeSidebarColor(color: string) {
    var sidebar = <HTMLElement>document.querySelector('.sidebar');

    this.sidebarColor = color;
    if (sidebar != undefined) {
      sidebar.setAttribute('data-color', color);
    }
  }
  changeSidebarActiveColor(color: string) {
    var sidebar = <HTMLElement>document.querySelector('.sidebar');
    this.sidebarActiveColor = color;
    if (sidebar != undefined) {
      sidebar.setAttribute('data-active-color', color);
    }
  }
  ngOnInit() { }
}
