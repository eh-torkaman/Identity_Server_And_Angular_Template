import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject, throwError } from 'rxjs';
import { Constants } from './constants';
import { dbClaim,dbUser, UserNameAndPassword } from './interfaces/interfaces_usersOnIdp';
import { catchError, debounceTime, distinctUntilChanged, filter, map, switchMap, tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class IdPUsersService {

  init: boolean = false;
  constructor(private httpClient: HttpClient) {
    
  }

  public ClaimDataChanged() {
    this.getAllUsers().subscribe({
      next: (users => {
        this.dbUsersForClaims$.next(users);
      })
    });
  }

  public UserDataChanged() {
    this.getAllUsers().subscribe({
      next: (users => {
        this.dbUsers$.next(users);
        this.dbUsersForClaims$.next(users);
      })
    });
  }


  public dbUsers$: BehaviorSubject<dbUser[]> = new BehaviorSubject<dbUser[]>([]) ;
  public dbUsersForClaims$: BehaviorSubject<dbUser[]> = new BehaviorSubject<dbUser[]>([]);


  private getAllUsers(): Observable<dbUser[] >{
    return this.httpClient.get<dbUser[]>(`${Constants.stsAuthority}api/users`).pipe(
      tap(data => console.log('dbUsers[] : ', JSON.stringify(data))),
      catchError(this.handleError)
    );
  }

  CreateNewClaimForUser(dbClaim: dbClaim, userName: string): Observable<any> {
    return this.httpClient.post(`${Constants.stsAuthority}api/claims/${userName}`, dbClaim);
  }

  DeleteClaim(dbClaim: dbClaim, userName: string): Observable<any> {
    const httpOptions = {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' }), body: dbClaim
    };
    return this.httpClient.delete(`${Constants.stsAuthority}api/claims/${userName}`, httpOptions);
  }

  DeleteUser(userName: string): Observable<any> {
   
    return this.httpClient.delete(`${Constants.stsAuthority}api/users/${userName}`);
  }
  LockUnlockUser(userName: string): Observable<any> {

    return this.httpClient.put(`${Constants.stsAuthority}api/users/${userName}/Lock`,"");
  }
  CreateNewUser(userNameAndPassword: UserNameAndPassword ): Observable<any> {
    return this.httpClient.post(`${Constants.stsAuthority}api/users`, userNameAndPassword);
  }

  ResetUserPass(userName: string, password: string) {
    return this.httpClient.put(`${Constants.stsAuthority}api/users/`, {
      'userName': userName,'password': password });
  }






  private handleError(err: any) {
    // in a real world app, we may send the server to some remote logging infrastructure
    // instead of just logging it to the console
    let errorMessage: string;
    if (err.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      errorMessage = `An error occurred: ${err.error.message}`;
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      errorMessage = `Backend returned code ${err.status}: ${err.body.error}`;
    }
    console.error(err);
    return throwError(errorMessage);
  }
}

 
