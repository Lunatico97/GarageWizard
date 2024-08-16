import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot,RouterStateSnapshot, Router } from '@angular/router';
import { AccountService } from './account.service';
import { LoggerService } from './logger.service';
 
@Injectable()
export class AuthGuardService implements CanActivate {
 
    constructor(private _logger: LoggerService, private _router: Router, private _accountService: AccountService ) {}
 
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean 
    {
        if (!this._accountService.checkIfUserIsLoggedIn()) {
            this._logger.pop('Please login before you can access the services !');
            this._router.navigate(["account"], {queryParams: { returnUrl: route.url} });
            return false;
        } 
        return true;
    }
 
}