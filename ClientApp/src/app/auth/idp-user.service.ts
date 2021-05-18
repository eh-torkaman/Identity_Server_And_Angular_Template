import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { EventEmitter, Injectable } from '@angular/core';
import { BehaviorSubject, empty, Observable, Subject, throwError } from 'rxjs';
import { Constants } from './constants';
import { dbClaim, dbUser, UserNameAndPassword, ChangeUserPass } from './interfaces/interfaces_usersOnIdp';
import { catchError, debounceTime, distinctUntilChanged, filter, map, shareReplay, startWith, switchMap, tap } from 'rxjs/operators';
import { MessageService } from '../messageService/message.service';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class IdPUsersService {

  init: boolean = false;
  constructor(private httpClient: HttpClient, private messageService: MessageService, private authService: AuthService) {


    this.authService.isLoggedIn().then(loggedIn => { if (loggedIn) this.loadCurrentUser(); });
    this.authService.loginChanged.subscribe(loggedIn => { if (loggedIn) { this.loadCurrentUser(); } else { this.CurrentDbUser.next(null); } });
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

  ///////////////////////////////
  private CurrentDbUser: BehaviorSubject<dbUser | null> = new BehaviorSubject<dbUser | null>(null);
  public CurrentDbUser$: Observable<dbUser | null> = this.CurrentDbUser.asObservable();//.pipe(  shareReplay(1));
  private loadCurrentUser() {
     this.httpClient.get<dbUser>(`${Constants.stsAuthority}api/CurrentUser`).pipe(
      tap(data => console.log('CurrentUser : ', JSON.stringify(data))),
       catchError(this.handleError),
      // catchError(p => { return empty; })
     )
       .subscribe(dbUser => { this.CurrentDbUser.next(dbUser); }
         , (err) => this.messageService.NotifyErr(err));
  }
  public ChangeUserPass(changeUserPass: ChangeUserPass) {
    this.httpClient.put<any>(`${Constants.stsAuthority}api/CurrentUser/`, changeUserPass)
       .pipe(catchError(x =>  this.handleError(x) ))
    //  .pipe(tap(rs => console.log(rs)),        catchError(this.handleError  ))
      .subscribe(
         rs => {
          this.messageService.Notify(rs.message );
            this.loadCurrentUser();
          },
           (err) => { console.warn(JSON.stringify( err)); this.messageService.NotifyErr(err) }
        
    );
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${JSON.stringify( error.error)}`);
    }
    // Return an observable with a user-facing error message.
    console.log("err.error instanceof ErrorEvent ", error.error instanceof ErrorEvent)
    return throwError(error);
  }
  ///////////////////////////////



  private handleError__(err: any) {
    console.log("in handleError Func", err)
    // in a real world app, we may send the server to some remote logging infrastructure
    // instead of just logging it to the console
    try {
      let errorMessage: string;
      if (err.error instanceof ErrorEvent ) {
         
        let er = err.error as ErrorEvent;
          
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
    catch {
      return throwError(err)
    }
  }
}

 
