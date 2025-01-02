import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
@Component({
    selector: 'app-forbidden',
    templateUrl: './forbidden.component.html',
    styleUrls: ['./forbidden.component.scss'],
    standalone: false
})
export class ForbiddenComponent implements OnInit {
  private returnUrl: string;
  constructor(private router: Router, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  public navigateToLogin = () => {
    this.router.navigate(['/authentication/login'], { queryParams: { returnUrl: this.returnUrl } });
  }
}
