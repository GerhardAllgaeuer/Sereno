import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RegisterUserComponent } from './register-user/register-user.component';
import { LoginComponent } from './login/login.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';
import { PrivacyComponent } from './privacy/privacy.component';
import { AdminGuard } from '../shared/guards/admin.guard';


const routes: Routes = [
  {
    path: 'registration',
    component: RegisterUserComponent,
  },
  {
    path: 'login',
    component: LoginComponent,
  },
  {
    path: 'forbidden',
    component: ForbiddenComponent,
  },
  {
    path: 'privacy',
    component: PrivacyComponent,
    canActivate: [AdminGuard],
  },

];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthenticationRoutingModule { }
