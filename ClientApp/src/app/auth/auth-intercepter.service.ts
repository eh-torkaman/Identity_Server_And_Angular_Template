import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpHeaders } from '@angular/common/http'
import { from, Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { Constants } from './constants';

@Injectable({
  providedIn: 'root'
})
export class AuthIntercepterService implements HttpInterceptor {

  constructor(private _authService: AuthService) { }
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.url.startsWith(Constants.stsAuthority)) {
      req = req.clone({
        withCredentials: true
      });
    }
    if (req.url.startsWith(Constants.apiRoot)) {
      return from(
        this._authService.getAccessToken().then(token => {
          const headers = new HttpHeaders().set("Authorization", `Bearer ${token}`);
          const authReq = req.clone({ headers });
          return next.handle(authReq).toPromise();
        })
      );
    }
    else {
      return next.handle(req);
    }
    
    }
}
