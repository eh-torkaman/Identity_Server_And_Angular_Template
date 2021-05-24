import { Component, OnInit } from '@angular/core';
import { IdPUsersService } from '../idp-user.service';

@Component({
  selector: 'app-manage-roles',
  templateUrl: './manage-roles.component.html',
  styleUrls: ['./manage-roles.component.css']
})
export class ManageRolesComponent implements OnInit {
  DbRoles$=this.idPUsersService.DbRoles$;
     
  constructor(private idPUsersService:IdPUsersService) { }
  ngOnInit(): void {
  }

}
