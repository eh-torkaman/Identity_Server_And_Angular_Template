import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private _snackBar: MatSnackBar) { }

  Notify(input: string, duration: number=1000) {
    this._snackBar.open(input, "", { duration: duration })
  }

  NotifyErr(err: Error, duration: number = 5000) {
    console.warn(err.message);
    console.warn(err);
    if (err instanceof HttpErrorResponse) {
      let error = err;
      //console.warn(error.error[Object.keys(error.error)[0]]);

      let httpErr = (err as HttpErrorResponse);
      let msg = httpErr.message + "  " +
        typeof (httpErr.error) == "string" ? httpErr.error : JSON.stringify(httpErr.error)
      this._snackBar.open(msg, "", { duration: duration, panelClass: "ErrorSnackBar" })
      console.error(msg);
    }
    else {
      this._snackBar.open(err.message, "", { duration: duration })

    }
   
  }
}
