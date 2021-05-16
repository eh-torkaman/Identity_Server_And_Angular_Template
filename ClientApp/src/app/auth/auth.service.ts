import { Injectable } from '@angular/core';
import { User, UserManager } from 'oidc-client';
import { promise } from 'protractor';
import { BehaviorSubject, Subject } from 'rxjs';
import { Constants} from './constants'
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _userManager: UserManager;
  private _user: User | null = null;

  private _loginChangedSubject = new BehaviorSubject<boolean>(false);

  public loginChanged = this._loginChangedSubject.asObservable();

  constructor() {
    const stsSettings = {
      authority: Constants.stsAuthority,
      client_id: Constants.clientId,
      redirect_uri: `${Constants.clientRoot}signin-callback`,
      scope: Constants.scope,
      response_type: 'code',
      post_logout_redirect_uri: `${Constants.clientRoot}signout-callback`,
      automaticSilentRenew: true,
      silent_redirect_uri: `${Constants.clientRoot}assets/silent-callback.html`

    };
    this._userManager = new UserManager(stsSettings);
    this._userManager.events.addAccessTokenExpired(_ => { this._loginChangedSubject.next(false); })
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
      this.setIsSuperAdmin(isLoggedIn);
      console.log("isLoggedIn: " + isLoggedIn);
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
    this._loginChangedSubject.next(false);
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


  private setIsSuperAdmin(isLoggedIn: boolean):void {
    let user = this._user as User;
   // console.log(JSON.stringify(user));
    //console.log('(typeof (user?.profile["role"]) !== "undefined") :' + (typeof (user?.profile["role"]) !== "undefined"));
   // console.log('(typeof (user?.profile["role"]) === "string" && user?.profile["role"] == "SuperAdmin") : ' + (typeof (user?.profile["role"]) === "string" && user?.profile["role"] == "SuperAdmin") )
  //  console.log('(typeof (user?.profile["role"]) !== "string" && user?.profile["role"].indexOf("SuperAdmin") != -1) :' + (typeof (user?.profile["role"]) !== "string" && user?.profile["role"].indexOf("SuperAdmin") != -1))
    if (isLoggedIn
      && (typeof (user?.profile["role"]) !== "undefined")
      && (
        (typeof (user?.profile["role"]) === "string" && user?.profile["role"] == "SuperAdmin") ||
        (typeof (user?.profile["role"]) !== "string" && user?.profile["role"].indexOf("SuperAdmin") != -1))
    ) {
      console.log(" this.IsSuperAdmin : TRUE");
      this.IsSuperAdmin = true;
    }
    else {
      this.IsSuperAdmin = false;
    }
     
  }
  public IsSuperAdmin: boolean = false;

}
