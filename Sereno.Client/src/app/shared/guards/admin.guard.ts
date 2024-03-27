import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthenticationService } from '../services/authentication.service';


export const AdminGuard: CanActivateFn = (route, state) => {

  const returnUrl = state.url;
  const authService = inject(AuthenticationService);

  if (authService.isUserAdmin()) {
    return true;
  } else {
    authService.navigateTo(returnUrl);
    return false;
  }
};
