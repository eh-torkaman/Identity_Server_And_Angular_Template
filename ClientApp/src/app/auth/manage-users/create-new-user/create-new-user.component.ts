import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MessageService } from '../../../messageService/message.service';
import { IdPUsersService } from '../../idp-user.service';

@Component({
  selector: 'app-create-new-user',
  templateUrl: './create-new-user.component.html',
  styleUrls: ['./create-new-user.component.css']
})
export class CreateNewUserComponent implements OnInit {

  constructor(private _idPUsersService: IdPUsersService, private messageService: MessageService) {

  }
  ngOnInit(): void {
  }
  FormCreateNewUser = new FormGroup({
    userName: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required)
  })
  CreateNewUser() {
    this._idPUsersService.CreateNewUser(this.FormCreateNewUser.value).subscribe({
      next: _ => { this.messageService.Notify("ساخته شد"); this._idPUsersService.UserDataChanged(); },
      error: err => this.messageService.NotifyErr(err)
    });
  }
}
