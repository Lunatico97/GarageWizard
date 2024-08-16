import { Component, OnInit } from '@angular/core';
import { AccountService } from 'src/app/services/account.service';
import { RouterService } from 'src/app/services/router.service';

@Component({
  selector: 'app-selector',
  templateUrl: './selector.component.html',
  styleUrls: ['./selector.component.scss']
})
export class SelectorComponent implements OnInit {

  navlinks = ['/jobs/transactions', '/jobs/repairs'];
  navTitles = ['Transactions', 'Repairs'];
  navImages = ['assets/transactions.png', 'assets/repairs.png'];
  constructor(private _router: RouterService, private _accountService: AccountService) { }

  ngOnInit(): void {
    if(this._accountService.checkIfUserisTechnician()){
      this.navlinks.push('/jobs/info');
      this.navTitles.push('Create');
      this.navImages.push('assets/maintenance.png');
    }
  }

  onClickCallback(route: string): void{
    this._router.routeToPath(route);
  }

}
