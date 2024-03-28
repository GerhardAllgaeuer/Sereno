import { Component } from '@angular/core';
import { AuthenticationService } from './shared/services/authentication.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'connexia';

  constructor(private authService: AuthenticationService) { }

  ngOnInit(): void {
    if (this.authService.isUserAuthenticated())
      this.authService.sendAuthStateChangeNotification(true);
  }
}
