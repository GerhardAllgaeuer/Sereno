import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { RouterOutlet, RouterModule } from '@angular/router';
import { HeaderComponent } from './components/header/header.component';
import { StateResetService } from './services/state-reset.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    HeaderComponent,
    RouterModule
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(private http: HttpClient, private stateResetService: StateResetService) {
    // Beim App-Start alle gespeicherten Zustände löschen
    this.stateResetService.clearStoredStates();
  }

  title = 'Sereno.Documentation.Client';
}
