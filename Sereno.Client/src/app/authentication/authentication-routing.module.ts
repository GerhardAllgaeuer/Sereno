import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RegisterUserComponent } from './register-user/register-user.component';
import { LoginComponent } from './login/login.component';

const routes: Routes = [
  {
    path: 'registration',
    component: RegisterUserComponent,
  },
  {
    path: 'login',
    component: LoginComponent,
  }

];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthenticationRoutingModule { }
