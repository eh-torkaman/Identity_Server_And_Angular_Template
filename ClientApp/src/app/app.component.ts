import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { Constants } from './auth/constants';
import { MessageService } from './messageService/message.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'appIS';
  isLoggedIn = false;
  _user: any;
  isSuperAdmin: boolean=false;
  
 
    
  constructor(private _authService: AuthService, private httpClient: HttpClient,private msgSrv:MessageService) { }

  ngOnInit(): void {
    this._authService.loginChanged.subscribe(loggedIn => {
      this._authService.isLoggedIn().then(loggedIn => {
        this.isLoggedIn = loggedIn;
        this.isSuperAdmin = this._authService.IsSuperAdmin;
        this._authService.getUser().then(user => { this._user = user; })
      });
    });
  }

  login() {
    this._authService.login();
  }

  logOut() {
    this._authService.logout();
  }

  
}
