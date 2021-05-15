import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { AdminRouteGuardComponent } from './auth/admin-route-guard/admin-route-guard.component';
import { ManageUsersComponent } from './auth/manage-users/manage-users.component';
import { SigninRedirectCallbackComponent } from './auth/signin-redirect-callback/signin-redirect-callback.component';
import { SignoutRedirectCallbackComponent } from './auth/signout-redirect-callback/signout-redirect-callback.component';

const routes: Routes = [
 // { path: '', component: AppComponent },
  { path: 'signin-callback', component: SigninRedirectCallbackComponent },
  { path: 'signout-callback', component: SignoutRedirectCallbackComponent },
  { path: 'manage-users', component: ManageUsersComponent, canActivate: [AdminRouteGuardComponent]}

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
