import { Component } from '@angular/core';
import { AuthenticationService } from './shared/services/authentication.service';
import deMessages from "devextreme/localization/messages/de.json";
import { locale, loadMessages } from "devextreme/localization";

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: false
})
export class AppComponent {
  title = 'connexia';

  constructor(private authService: AuthenticationService)
  {
    loadMessages(deMessages);
    locale(navigator.language);
  }

  ngOnInit(): void {
    if (this.authService.isUserAuthenticated())
      this.authService.sendAuthStateChangeNotification(true);
  }
}
