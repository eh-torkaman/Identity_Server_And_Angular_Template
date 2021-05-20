import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
 
  
import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { SnotifyPosition, SnotifyService, SnotifyToastConfig } from 'ng-snotify';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(private _snackBar: MatSnackBar,private snotifyService: SnotifyService) { }

  Notify(input: string | Array<CustomMessage>, duration: number = 1000) {
    let msg = input as string;

    console.info("Notify: msg: ", msg,)

    let customMessages = <Array<CustomMessage>>input;

    if ((!!customMessages) && (typeof (input) !== "string")) {
      msg = this.flattenCustomMessage(customMessages);
    }

    console.log("input : ",JSON.stringify(input));
    console.log(!!customMessages)
    console.info("Notify: ",msg)
    this._snackBar.open(msg, "", { duration: duration })
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

  ///////////////***************/////////////////
  style = 'material';
  title = 'Snotify title!';
  body = 'Lorem ipsum dolor sit amet!';
  timeout = 30000;
  position: SnotifyPosition = SnotifyPosition.rightBottom;
  progressBar = true;
  closeClick = true;
  newTop = true;
  filterDuplicates = false;
  backdrop = -1;
  dockMax = 8;
  blockMax = 6;
  pauseHover = true;
  titleMaxLength = 15;
  bodyMaxLength = 80;

   /*
  Change global configuration
   */
  getConfig(): SnotifyToastConfig {
    this.snotifyService.setDefaults({
      global: {
        newOnTop: this.newTop,
        maxAtPosition: this.blockMax,
        maxOnScreen: this.dockMax,
        // @ts-ignore
        filterDuplicates: this.filterDuplicates
      }
    });
    return {
      bodyMaxLength: this.bodyMaxLength,
      titleMaxLength: this.titleMaxLength,
      backdrop: this.backdrop,
      position: this.position,
      timeout: this.timeout,
      showProgressBar: this.progressBar,
      closeOnClick: this.closeClick,
      pauseOnHover: this.pauseHover
    };
  }

  onSuccess() {
    this.snotifyService.success(this.body, this.title, this.getConfig());
  }
  onInfo() {
    this.snotifyService.info(this.body, this.title, this.getConfig());
  }
  onError() {
    this.snotifyService.error(this.body, this.title, this.getConfig());
  }
  onWarning() {
    this.snotifyService.warning(this.body, this.title, this.getConfig());
  }
  onSimple() {
    // const icon = `assets/custom-svg.svg`;
    const icon = `https://placehold.it/48x100`;

    this.snotifyService.simple(this.body, this.title, {
      ...this.getConfig(),
      icon
    });
  }

  onConfirmation() {
    /*
    Here we pass an buttons array, which contains of 2 element of type SnotifyButton
     */
    const { timeout, closeOnClick, ...config } = this.getConfig(); // Omit props what i don't need
    this.snotifyService.confirm(this.body, this.title, {
      ...config,
      buttons: [
        { text: 'Yes', action: () => console.log('Clicked: Yes'), bold: false },
        { text: 'No', action: () => console.log('Clicked: No') },
        {
          text: 'Later',
          action: toast => {
            console.log('Clicked: Later');
            this.snotifyService.remove(toast.id);
          }
        },
        {
          text: 'Close',
          action: toast => {
            console.log('Clicked: Close');
            this.snotifyService.remove(toast.id);
          },
          bold: true
        }
      ]
    });
  }

  onPrompt() {
    /*
     Here we pass an buttons array, which contains of 2 element of type SnotifyButton
     At the action of the first buttons we can get what user entered into input field.
     At the second we can't get it. But we can remove this toast
     */
    const { timeout, closeOnClick, ...config } = this.getConfig(); // Omit props what i don't need
    this.snotifyService
      .prompt(this.body, this.title, {
        ...config,
        buttons: [
          {
            text: 'Yes',
            action: toast => console.log('Said Yes: ' + toast.value)
          },
          {
            text: 'No',
            action: toast => {
              console.log('Said No: ' + toast.value);
              this.snotifyService.remove(toast.id);
            }
          }
        ],
        placeholder: 'Enter "ng-snotify" to validate this input' // Max-length = 40
      })
      .on('input', toast => {
        console.log(toast.value);
        toast.valid = !!toast.value.match('ng-snotify');
      });
  }

  onHtml() {
    const html = `<div class="snotifyToast__title"><b>Html Bold Title</b></div>
    <div class="snotifyToast__body"><i>Html</i> <b>toast</b> <u>content</u></div>`;
    this.snotifyService.html(html, this.getConfig());
  }

  onClear() {
    this.snotifyService.clear();
  }


  ///////////////***************/////////////////
}

//  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
//  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //       
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
