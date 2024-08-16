import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { RouterService } from './services/router.service';
import { AccountService } from './services/account.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoggerService } from './services/logger.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'GarageWizard';
  navlinks = ['/', 'vehicles', 'jobs', 'spots', 'roles'];
  navTitles = ['Home', 'Vehicles', 'Jobs', 'Spots', 'Roles'];
  navImages = ['assets/home.png', 'assets/vehicles.png', 'assets/maintenance.png', 'assets/parking.png', 'assets/roles.png'];

  constructor(private _router: RouterService, private _logger: LoggerService, private _account: AccountService){}

  public checkIfLoggedIn(){
    return this._account.checkIfUserIsLoggedIn();
  }

  public routeTo(path: string){
    this._router.routeToPath(path);
  }

  public onLogoutCallback(){
    this._account.logoutUser()
    .subscribe({
      next: (value) => {
        if(!value.loggedIn){
          this._logger.log("Account Service [Logout]", "Successful user departure !");
          this._account.setUserLoginFlag(false);
          sessionStorage.clear();
          this._account.setSudoFlag(false);
          this._router.routeToPath('account');
        }
      },
      error: (errors) => {
        this._logger.log("AccountService [Logout Error]", errors.message);
      },
      complete: () => {
        this._logger.log("AccountService [Logout]", "Successful POST request");
      }
    });
    this._logger.pop("Logged out of account !", 3000);
  }
}
