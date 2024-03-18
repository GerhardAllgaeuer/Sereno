import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SidePanelComponent } from '../side-panel/side-panel.component';
import { FooterPanelComponent } from '../footer-panel/footer-panel.component';
import { NavPanelComponent } from '../nav-panel/nav-panel.component';
import { FixedPluginComponent } from '../fixedplugin/fixedplugin.component';

@Component({
  selector: 'main-wnd',
  imports: [
    RouterModule,
    SidePanelComponent,
    FooterPanelComponent, 
    NavPanelComponent,
    FixedPluginComponent,
  ],
  standalone: true,
  templateUrl: './main-window.component.html',
  styleUrl: './main-window.component.scss'
})
export class MainWindowComponent {

}
