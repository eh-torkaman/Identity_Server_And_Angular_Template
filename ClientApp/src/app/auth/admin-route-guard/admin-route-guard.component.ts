import { Component, Injectable, OnInit } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { User } from 'oidc-client';
import { Observable } from 'rxjs';
import { AuthService } from '../auth.service';

@Injectable({
  providedIn: "root"
})

@Component({
  selector: 'app-admin-route-guard',
  templateUrl: './admin-route-guard.component.html',
  styleUrls: ['./admin-route-guard.component.css']
})
export class AdminRouteGuardComponent implements CanActivate {

  constructor(private _authService: AuthService) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
    console.log('app-admin-route-guard : ' + this._authService.IsSuperAdmin)
    //if (!this._authService.isAuthenticated())      this._authService.login();
    return this._authService.IsSuperAdmin;
  }

}
