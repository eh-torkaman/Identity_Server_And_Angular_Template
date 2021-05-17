import {  Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { IdPUsersService } from '../../idp-user.service';
import { dbClaim, dbUser } from '../../interfaces/interfaces_usersOnIdp';
import { FormGroup, FormControl, RequiredValidator, Validators } from '@angular/forms';
import { MessageService } from '../../../messageService/message.service';
import { Constants } from '../../constants';
@Component({
  selector: 'app-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.css']
})
export class UserCardComponent implements OnInit {

  constructor(private _idPUsersService: IdPUsersService, private messageService: MessageService) {

  }
  @Input() user: dbUser;
  ngOnInit(): void {
    this.imageVersrionStr = this.user.profileImageNumber;
  }
  imageVersrionStr: number = 0;
  stsAuthorityUrl: string = Constants.stsAuthority;
  FormResetUserPass = new FormGroup({
    password: new FormControl('', Validators.required)
  })

  ResetUserPass(userName: string) {
    this._idPUsersService.ResetUserPass(userName, this.FormResetUserPass.controls["password"].value).subscribe({
      next: _ => { this.messageService.Notify("پسورد تغییر کرد") },
      error: err => this.messageService.NotifyErr(err)
    });
  }

  DeleteUser( ) {
    this._idPUsersService.DeleteUser(this.user.userName).subscribe({
      next: () => {
        this.messageService.Notify("کابر " + this.user.userName + " حذف شد")
        this._idPUsersService.UserDataChanged();
      },
      error: (err) => this.messageService.NotifyErr(err)

    });;
  }
  UploadImageUseFinished(event: any) {
    this.log(event);
    this.imageVersrionStr = event.value;
  }

  LockUnlock() {
    this._idPUsersService.LockUnlockUser(this.user.userName).subscribe({
      next: () => {
        this.messageService.Notify("انجام شد")
        this._idPUsersService.UserDataChanged();
      },
      error: (err) => this.messageService.NotifyErr(err)

    });;
  }

  log = (p: any) => {
    console.log(p)
  }

}
