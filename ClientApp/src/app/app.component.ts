import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { Constants } from './auth/constants';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'appIS';
  isLoggedIn = false;
  _user: any;

  
  public get isSuperAdmin(): boolean {
    return this._authService.isSuperAdmin();
    }
    
  constructor(private _authService: AuthService, private httpClient: HttpClient ) {
    this._authService.loginChanged.subscribe(loggedIn => {
      this.isLoggedIn = loggedIn;
      this._authService.getUser().then(user => { this._user = user; })
    });

   
  }
  ngOnInit(): void {
    this._authService.isLoggedIn().then(loggedIn => { this.isLoggedIn = loggedIn; });
    this._authService.getUser().then(user => { this._user = user; });
  }

  login() {
    this._authService.login();
  }

  logOut() {
    this._authService.logout();
  }
}
