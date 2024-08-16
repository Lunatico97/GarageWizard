import { HttpResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ObservableInput, catchError } from 'rxjs';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'account-ui',
  templateUrl: './combined.component.html',
  styleUrls: ['./combined.component.scss']
})
export class CombinedComponent implements OnInit {

  navlinks = ['/account/login', '/account/register'];
  navTitles = ['Login', 'Register'];
  showUser: boolean = false;
  loggedIn: boolean = false;
  currentUser!: User;

  constructor(private _account: AccountService){}

  ngOnInit(): void {
    this.loggedIn = this._account.checkIfUserIsLoggedIn();
  }
}
