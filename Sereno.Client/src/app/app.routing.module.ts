import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DefaultComponent } from './default/default.component';
import { DenyGuestsGuard } from './shared/guards/auth.guard';


const routes: Routes = [
  {
    path: 'mobile',
    component: DefaultComponent,
  },

  //{
  //  path: '', redirectTo: '/authentication/registration', pathMatch: 'full'
  //},

  {
    path: '',
    loadChildren: () => import('./layout/layout.module').then(m => m.LayoutModule),
    canActivate: [DenyGuestsGuard]
  },

  {
    path: 'authentication',
    loadChildren: () => import('./authentication/authentication.module').then(m => m.AuthenticationModule)
  },
]

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }
