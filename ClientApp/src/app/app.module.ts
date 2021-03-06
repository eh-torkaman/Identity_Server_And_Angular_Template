import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { HttpClientModule } from '@angular/common/http'

import { SigninRedirectCallbackComponent } from './auth/signin-redirect-callback/signin-redirect-callback.component';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { SnotifyModule, SnotifyService, ToastDefaults } from 'ng-snotify';


import { AuthIntercepterService } from './auth/auth-intercepter.service';
import { ManageUsersComponent } from './auth/manage-users/manage-users.component';
import { SignoutRedirectCallbackComponent } from './auth/signout-redirect-callback/signout-redirect-callback.component';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { UserClaimsComponent } from './auth/manage-users/userClaims/user-claims.component';
import { AdminRouteGuardComponent } from './auth/admin-route-guard/admin-route-guard.component';
import { CreateNewUserComponent } from './auth/manage-users/create-new-user/create-new-user.component';
import { UploadComponent } from './upload/upload.component';
import { UserCardComponent } from './auth/manage-users/user-card/user-card.component';
import { CurrentUserManagerComponent } from './auth/currentUser/current-user-manager/current-user-manager.component';
import { ManageRolesComponent } from './auth/manage-roles/manage-roles.component';

 

@NgModule({
  declarations: [
    AppComponent,
    SigninRedirectCallbackComponent,
    SignoutRedirectCallbackComponent,
    ManageUsersComponent, UserClaimsComponent, AdminRouteGuardComponent, CreateNewUserComponent, UploadComponent
    , UserCardComponent, CurrentUserManagerComponent, ManageRolesComponent, 
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MaterialModule,
    FormsModule, ReactiveFormsModule
    ,SnotifyModule
  ],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: AuthIntercepterService, multi: true },
    { provide: 'SnotifyToastConfig', useValue: ToastDefaults }, SnotifyService],
  bootstrap: [AppComponent]
})
export class AppModule { }
