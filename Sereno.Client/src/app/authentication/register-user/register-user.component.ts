import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthenticationService } from './../../shared/services/authentication.service';
import { PasswordConfirmationValidatorService } from './../../shared/custom-validators/password-confirmation-validator.service';
import { UserForRegistrationDto } from './../../_interfaces/user/userForRegistrationDto.model';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
    selector: 'app-register-user',
    templateUrl: './register-user.component.html',
    styleUrls: ['./register-user.component.scss'],
    standalone: false
})
export class RegisterUserComponent implements OnInit {
  registerForm: FormGroup;
  errorMessage: string = '';
  showError: boolean;

  constructor(private authService: AuthenticationService,
    private passConfValidator: PasswordConfirmationValidatorService) {
  }

  ngOnInit(): void {
    this.registerForm = new FormGroup({
      firstName: new FormControl(''),
      lastName: new FormControl(''),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required]),
      confirm: new FormControl('')
    });

    const confirmControl = this.registerForm.get('confirm');
    const passwordControl = this.registerForm.get('password');

    if (confirmControl && passwordControl) {
      confirmControl.setValidators([
        Validators.required,
        this.passConfValidator.validateConfirmPassword(passwordControl)
      ]);
      confirmControl.updateValueAndValidity();
    } else {
      console.error('Form control for password or confirm not found');
    }
  }

  public validateControl = (controlName: string): boolean => {
    let result = this.registerForm.get(controlName)?.invalid && this.registerForm.get(controlName)?.touched;
    return result || false;
  }

  public hasError = (controlName: string, errorName: string): boolean => {
    let result = this.registerForm.get(controlName)?.hasError(errorName)
    return result || false;

  }

  public registerUser = (registerFormValue: any) => {
    this.showError = false;
    const formValues = { ...registerFormValue };

    const user: UserForRegistrationDto = {
      firstName: formValues.firstName,
      lastName: formValues.lastName,
      email: formValues.email,
      password: formValues.password,
      confirmPassword: formValues.confirm
    };

    this.authService.registerUser("api/accounts/registration", user)
      .subscribe({
        next: (_) => console.log("Successful registration"),
        error: (err: HttpErrorResponse) => {
          this.errorMessage = err.message;
          this.showError = true;
        }
      })
  }
}
