import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { ConfirmedValidator } from '../../../validators/confirm-password.validator';
import { Constants } from '../../constants';
import { IdPUsersService } from '../../idp-user.service';
import { dbUser } from '../../interfaces/interfaces_usersOnIdp';

@Component({
  selector: 'app-current-user-manager',
  templateUrl: './current-user-manager.component.html',
  styleUrls: ['./current-user-manager.component.css']
})
export class CurrentUserManagerComponent implements OnInit {

  constructor(private idpService: IdPUsersService, private fb: FormBuilder) {

    this.FormChangeUserPass = fb.group({
      oldPassword: new FormControl('', Validators.required),
      newPassword: new FormControl('', [Validators.required, Validators.minLength(2)]),
      confirmPassword: new FormControl('', [Validators.required, Validators.minLength(2)])
    }, {
      validator: ConfirmedValidator('newPassword', 'confirmPassword')
    })
  }

  stsAuthorityUrl: string = Constants.stsAuthority;
  currentDbUser$: Observable<dbUser> = this.idpService.CurrentDbUser$;

  FormChangeUserPass: FormGroup = new FormGroup({});

  ngOnInit(): void {
  }
  ChangeUserPass(userName: string) {

  }
}
