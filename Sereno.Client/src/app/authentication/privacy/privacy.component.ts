import { OnInit, Component } from '@angular/core';
import { AuthenticationService } from './../../shared/services/authentication.service';
import { Claim } from '../../_interfaces/response/Claim';
import { CommonModule } from '@angular/common';


@Component({
    selector: 'app-privacy',
    templateUrl: './privacy.component.html',
    styleUrl: './privacy.component.scss',
    standalone: false
})
export class PrivacyComponent implements OnInit {
  public claims: Claim[] = [];
  constructor(private authService: AuthenticationService) { }
  ngOnInit(): void {
    this.getClaims();
  }
  public getClaims(): void {
    this.authService.getClaims('api/accounts/privacy')
      .subscribe((res: Claim[]) => {
        this.claims = res;
      });
  }
}
