import { HttpErrorResponse } from '@angular/common/http';
import { AuthResponseDto } from './../../_interfaces/response/authResponseDto.model';
import { UserForAuthenticationDto } from './../../_interfaces/user/userForAuthenticationDto.model';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from './../../shared/services/authentication.service';
import { Component, OnInit, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css'],
    standalone: false
})
export class LoginComponent implements OnInit, AfterViewInit {
  private returnUrl: string = '';

  loginForm!: FormGroup;
  errorMessage: string = '';
  showError: boolean = false;

  isLoggingIn: boolean = false;



  @ViewChild('username') usernameInput: ElementRef;

  constructor(private authService: AuthenticationService, private router: Router, private route: ActivatedRoute) {
    this.loginForm = new FormGroup({
      username: new FormControl('', [Validators.required]),
      password: new FormControl('', [Validators.required]) // Korrektur für den Namen des Feldes
    });
  }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }


  validateControl = (controlName: string): boolean => {
    return !this.loginForm.get(controlName)?.valid && (this.loginForm.get(controlName)?.touched ?? false);
  }

  hasError = (controlName: string, errorName: string): boolean => {
    return !!this.loginForm.get(controlName)?.hasError(errorName);
  }

  loginUser = (loginFormValue: { username: string, password: string }): void => {

    this.isLoggingIn = true;

    this.showError = false;
    const login = { ...loginFormValue };

    const userForAuth: UserForAuthenticationDto = {
      email: login.username,
      password: login.password
    }

    this.authService.loginUser('api/accounts/login', userForAuth)
      .subscribe({
        next: (res: AuthResponseDto) => {
          localStorage.setItem("token", res.token || '');
          this.authService.sendAuthStateChangeNotification(res.isAuthSuccessful || false);
          this.router.navigate([this.returnUrl]);
          this.isLoggingIn = false;
        },
        error: (err: HttpErrorResponse) => {
          this.errorMessage = err.message || 'Unknown error';
          this.showError = true;
          this.isLoggingIn = false;
        }
      });
  }

  ngAfterViewInit(): void {
    this.usernameInput.nativeElement.focus();
  }
}
