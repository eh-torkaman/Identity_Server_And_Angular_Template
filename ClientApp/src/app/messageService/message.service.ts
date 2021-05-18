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

  private flattenCustomMessage(customMessages: ICustomMessage[]): string {
    let rs = "";
    let i = 0;
    for (let cm of customMessages) {
      i++;
      rs +=   i+". "+cm.message+"      ";
    }
    return rs;
  }
  NotifyErr(err: Error, duration: number = 5000) {
    console.warn("---------NotifyErr>>>>-----")
    console.error(JSON.stringify(err));
    if (err instanceof HttpErrorResponse) {
      let error = err;
      let msg = "";
      let httpErr = (err as HttpErrorResponse);

      let customMessages=<Array<CustomMessage>>httpErr.error
      if (!!customMessages) {
        msg = this.flattenCustomMessage(customMessages)
      } else {
        msg = httpErr.message + "  " + typeof (httpErr.error) == "string" ? httpErr.error : JSON.stringify(httpErr.error)
      }
      this._snackBar.open(msg, "", { duration: duration, panelClass: "ErrorSnackBar" })
    }
    else {
      this._snackBar.open(err.message, "", { duration: duration })
    }
    console.warn("---------NotifyErr<<<<<<------")
  }

  
}
export class CustomMessage implements ICustomMessage {
    message: string;
    name: string;
    stack: string;
  isError: boolean;
  shouldLog: boolean;
}
export interface ICustomMessage {
  message: string;
  name: string;
  stack: string;
  isError: boolean;
  shouldLog: boolean;
}
