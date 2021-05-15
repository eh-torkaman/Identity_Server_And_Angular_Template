import { Injectable } from '@angular/core';
import { User, UserManager } from 'oidc-client';
import { promise } from 'protractor';
import { Subject } from 'rxjs';
import { Constants} from './constants'
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _userManager: UserManager;
  private _user: User | null = null;

  private _loginChangedSubject = new Subject<boolean>();

  public loginChanged = this._loginChangedSubject.asObservable();

  constructor() {
    const stsSettings = {
      authority: Constants.stsAuthority,
      client_id: Constants.clientId,
      redirect_uri: `${Constants.clientRoot}signin-callback`,
      scope: 'openid profile IdPApi',
      response_type: 'code',
      post_logout_redirect_uri: `${Constants.clientRoot}signout-callback`,

    };
    this._userManager = new UserManager(stsSettings);
  }

  login() {
    return this._userManager.signinRedirect();
  }

  getUser(): Promise<User|null> {
    return this._userManager.getUser();
  }

  isLoggedIn(): Promise<boolean> {
    return this._userManager.getUser().then(user => {
      const isLoggedIn = !!user && !user.expired;

      this._user = user;

      if (this._user !== user) {
        this._loginChangedSubject.next(isLoggedIn)
      }

      return isLoggedIn;
    })
  }

  completeLogin() {
    return this._userManager.signinRedirectCallback().then(user => {
      this._user = user;
      this._loginChangedSubject.next(!!user && !user.expired);
      return user;
    })
  }

  logout() {
    this._userManager.signoutRedirect();
  }
  completeLogout() {
    this._user = null;
    return this._userManager.signoutRedirectCallback();
  }

  getAccessToken(): Promise<string | null> {
    return this.getUser().then(user => {
      if (!!user && !user.expired)
        return user.access_token
      else
        return null;
    });
  }


  public isAuthenticated(): boolean {
    let user = this._user;
    if ((user != null) && (!!user) && (!user.expired)) return true;
    else return false;
    //return this.getUser().then(user => {
    //  if ((!!user) && (!user.expired)) return true;
    //  else return false;
    //});
  }
  public isSuperAdmin(): boolean {
    let user = this._user as User;
    if (this.isAuthenticated()
      && (typeof (user?.profile["role"]) !== "undefined")
      && (
      (typeof (user?.profile["role"]) === "string" && user?.profile["role"] == "SuperAdmin") ||
      (typeof (user?.profile["role"]) !== "string" && user?.profile["role"].indexOf("SuperAdmin") != -1))
    ) return true;
    else return false;
    /*
    return this.getUser().then(user => {
      if ((!!user) && (!user.expired) && (user.profile["Role"] == "SuperAdmin")) return true;
      else return false;
    });
    */
  }
}
