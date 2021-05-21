import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
//import { MatSnackBar } from '@angular/material/snack-bar';
 
  
//import { Component } from '@angular/core';
//import { Observable } from 'rxjs';
import { SnotifyPosition, SnotifyService, SnotifyToastConfig } from 'ng-snotify';
//import { Tracing } from 'trace_events';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  constructor(/*private _snackBar: MatSnackBar,*/private snotifyService: SnotifyService) { }

  Notify(input: string | Array<CustomMessage>, duration: number = 1000) {
    let msg = input as string;

    console.info("Notify: msg: ", msg,)

    let customMessages = <Array<CustomMessage>>input;

    if ((!!customMessages) && (typeof (input) !== "string")) {
      for(let customErr of customMessages){
        this.showCustomMessage(customErr,duration);
      }
    }
    let conf=this.getConfig();
    this.snotifyService.info(msg, "" ,{...conf,timeout:duration});
   // this._snackBar.open(msg, "", { duration: duration })
  }
   
  
  showCustomMessage(cm:CustomMessage,duration:number){
    const icon = `https://placehold.it/48x100`;
    let conf=this.getConfig();
    switch (cm.msgType){
      case MsgType.Error:
        this.snotifyService.error(cm.message, cm.generaterdTimePersianStr +"  " + cm.title ,{...conf,timeout:duration});
        break;

     case MsgType.Info:
        this.snotifyService.info(cm.message, cm.generaterdTimePersianStr +"  " +cm.title ,{...conf,timeout:duration});
        break;

     case MsgType.Success:
        this.snotifyService.success(cm.message, cm.generaterdTimePersianStr +"  " +cm.title ,{...conf,timeout:duration});
        break;
    case MsgType.Warning:
          this.snotifyService.warning(cm.message,cm.generaterdTimePersianStr +"  " + cm.title ,{...conf,timeout:duration});
          break;
    default:
            this.snotifyService.simple(cm.message,cm.generaterdTimePersianStr +"  " + cm.title ,{...conf,timeout:duration,icon});
            break;
    }
    
  }
  NotifyErr(err: Error, duration: number = 5000) {
    let conf=this.getConfig();
    console.warn("---------NotifyErr>>>>-----")
    console.error(JSON.stringify(err));
    if (err instanceof HttpErrorResponse) {
      let msg = "";
      let httpErr = (err as HttpErrorResponse);
      let customMessages=<Array<CustomMessage>>httpErr.error
      if (!!customMessages) {
        for(let customErr of customMessages){
          this.showCustomMessage(customErr,duration);
        }
      } else {
        msg = httpErr.message + "  " + typeof (httpErr.error) == "string" ? httpErr.error : JSON.stringify(httpErr.error)
        this.snotifyService.error(msg, "خطا" ,{...conf,timeout:duration});
    //  this._snackBar.open(msg, "", { duration: duration, panelClass: "ErrorSnackBar" })
      }
    }
    else {
     // this._snackBar.open(err.message, "", { duration: duration, panelClass: "ErrorSnackBar"  })
     this.snotifyService.error(err.message, "خطا" ,{...conf,timeout:duration});
    }
    console.warn("---------NotifyErr<<<<<<------")
  }

  ///////////////***************/////////////////
  style = 'material';
  title = '----------';
  body = '-----------';
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
  titleMaxLength = 150;
  bodyMaxLength = 800;

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
        placeholder: 'محل ورود' // Max-length = 40
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

  ClearAll() {
    this.snotifyService.clear();
  }


  ///////////////***************/////////////////
}

//  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  
//  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //       
export class CustomMessage implements ICustomMessage {
  generaterdTime: Date;
  msgType: MsgType;
  message: string;
  name: string;
  stack: string;
  isError: boolean;
  shouldLog: boolean;
  title:string;
  generaterdTimePersianStr:string;
}
export interface ICustomMessage {
  message: string;
  name: string;
  stack: string;
  shouldLog: boolean;
  generaterdTime:Date;
  msgType:MsgType
  title:string;
  generaterdTimePersianStr:string;
}

enum MsgType
{
    Info='Info',
    Error='Error',
    Success='Success',
    Warning='Warning',
    Simple='Simple'
}