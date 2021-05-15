import { AfterContentInit, AfterViewChecked, AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { IdPUsersService } from '../../idp-user.service';
import { dbClaim, dbUser } from '../../interfaces/interfaces_usersOnIdp';
import { FormGroup, FormControl } from '@angular/forms';
import {MatSnackBar} from '@angular/material/snack-bar';
import { exception } from 'console';
import { config } from 'rxjs';
import { MessageService } from '../../../messageService/message.service';

@Component({
  selector: 'app-user-claims',
  templateUrl: './user-claims.component.html',
  styleUrls: ['./user-claims.component.css']
})
export class UserClaimsComponent implements  OnInit{

  constructor(private _idPUsersService: IdPUsersService, private _snackBar: MatSnackBar, private messageService: MessageService) {

  }
  

  user: dbUser ;
  @Input() userName:string

  bindData() {
    this._idPUsersService.dbUsersForClaims$.subscribe({
      next: (users) => {
        console.log(users);
        this.user = users.find(user => user.userName == this.userName) as dbUser;
      },
      error: (err) => {
        console.log(err);
      }
    })
  }

  ngOnInit(): void {
    console.log("UserClaimsComponent  ngOnInit +=>> " + this.userName)
    this.bindData();
  }

  ClaimForm = new FormGroup({
    claimType: new FormControl(''),
    claimValue: new FormControl(''),
  });


 
  DeleteClaim(claim: dbClaim) {
    this._idPUsersService.DeleteClaim(claim, this.user.userName).subscribe({
      next: (rs) => {
         this._idPUsersService.ClaimDataChanged();
        this.messageService.Notify("حذف شد");
         
      },
      error: (err) => this.messageService.NotifyErr(err)
    })
  }
  CreateNewClaim() {
    this._idPUsersService.CreateNewClaimForUser(this.ClaimForm.value, this.user.userName)
      .subscribe({
        next: (rs) => {
          this.ClaimForm.reset(); this._idPUsersService.ClaimDataChanged();
          this.messageService.Notify("اضافه شد");
        },
        error: (err) =>   this.messageService.NotifyErr(err)
        
      });
  }
}
