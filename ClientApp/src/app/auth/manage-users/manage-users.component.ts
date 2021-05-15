import { AfterViewInit, ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { isPropertyName } from 'typescript';
import { IdPUsersService } from '../idp-user.service';
import { dbClaim, dbUser } from '../interfaces/interfaces_usersOnIdp';
import { FormGroup, FormControl, RequiredValidator, Validators } from '@angular/forms';
import { timeout, map } from 'rxjs/operators';
import { MessageService } from '../../messageService/message.service';
import { constants } from 'os';
import { Constants } from '../constants';
@Component({
  selector: 'app-manage-users',
  templateUrl: './manage-users.component.html',
  styleUrls: ['./manage-users.component.css']
})
export class ManageUsersComponent implements OnInit, AfterViewInit {


  dataSource: MatTableDataSource<dbUser>;

  @ViewChild(MatPaginator) paginator: MatPaginator;

  ngAfterViewInit() {
    this.log("ngAfterViewInit");
    this._idPUsersService.UserDataChanged();
    setTimeout(() => this.loadData(), 100);
  }

  loadData() {
    this.log("data changed");
    this._idPUsersService.dbUsers$.pipe(
      map(dbusers => dbusers.map(dbuser => ({
        ...dbuser,
        lockDescription: (dbuser.isLockedOut) ? "حذف قفل" : "قفل کردن"
      } as dbUser)))).subscribe({
        next: (users) => {
          this.log("this._idPUsersService.getAllUsers().subscribe");
          //  users = users.map(us => Object.defineProperty(us, 'lockDescription', { value: (us.isLockedOut) ? "حذف قفل" : "قفل کردن", writable: false, enumerable: true }));
          this.dataSource = new MatTableDataSource<dbUser>(users);
          this.paginator.length = users.length;
          this.dataSource.paginator = this.paginator;
          this.dataSource.filterPredicate = this.filterPredicate;
          this.getPagedData();
        }
      });

  }
  getPagedData() {
    this.dataSource.filter = this._filterValue;
    this.users = this.dataSource._pageData(this.dataSource.filteredData);
    console.log(this.users);
  }

  constructor(private _idPUsersService: IdPUsersService, private messageService: MessageService) {
    //this.users = [];

  }
  users: dbUser[];

  ngOnInit(): void {
    this.log("ngOnInit");
  }

  private _filterValue: string = "";
  public get filterValue(): string {
    return this._filterValue;
  }
  public set filterValue(value: string) {
    this._filterValue = value;
    this.dataSource.filter = value;
    this.getPagedData();
  }


  filterPredicate = (data: any, filter: string) => {
    const accumulator = (currentTerm: any, key: any) => { return this.nestedFilterCheck(currentTerm, data, key); };
    const dataStr = Object.keys(data).reduce(accumulator, '').toLowerCase();
    // Transform the filter by converting it to lowercase and removing whitespace.
    const transformedFilter = filter.trim().toLowerCase();

    this.log("dataStr: => " + dataStr);

    this.log("transformedFilter: => " + transformedFilter);

    return dataStr.indexOf(transformedFilter) !== -1;
  };
  nestedFilterCheck(search: any, data: any, key: any /**key: any !! **/) {
    if (typeof data[key] === 'object') {
      for (const k in data[key]) {
        if (data[key][k] !== null) {
          search = this.nestedFilterCheck(search, data[key], k);
        }
      }
    } else {
      if (!key.toLowerCase().endsWith("id"))
        search += data[key];

      this.log("key:  " + key + "   typeof data[key] => " + typeof (data[key]));
    }
    return search;
  }

  log = (p: any) => {
    console.log(p)
  }


}
