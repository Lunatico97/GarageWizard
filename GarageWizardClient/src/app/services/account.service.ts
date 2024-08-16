import { Injectable } from "@angular/core";
import { HTTPService } from "./http.service";
import { LoggerService } from "./logger.service";
import { Configuration } from "../config/globalConfig";
import { UserLogin } from "../models/userLogin";
import { UserRegister } from "../models/userRegister";
import { FormGroup } from "@angular/forms";
import { RouterService } from "./router.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Observable } from "rxjs";
import { User } from "../models/user";
import { UserChangeDto } from "../models/userChangeDto";
import { AuthResponseMessage } from "../models/auth-response-message";
import { HttpErrorResponse } from "@angular/common/http";
import { ServerReponseMessage } from "../models/server-response-message";

@Injectable()
export class AccountService
{   
    private isUserLoggedIn: boolean = false;
    private isUserTechnician: boolean = false;
    constructor(private _http: HTTPService, private _logger: LoggerService, private _router: RouterService, private _snackbar: MatSnackBar){}

    public getCurrentUser(): Observable<User>
    {
      return this._http.sendSingleGetRequest<User>({}, Configuration.garageApiGetCurrentUserEndpoint);
    }

    public getAllUsers(): Observable<User[]>
    {
      return this._http.sendGetRequest<User>(Configuration.garageApiGetUsersEndpoint);
    }

    public registerUser(userRegisterForm: FormGroup): Observable<AuthResponseMessage>
    {
        var user = new UserRegister(
          userRegisterForm.get('Name')?.value,
          userRegisterForm.get('Email')?.value,
          userRegisterForm.get('Password')?.value, 
        );
        return this._http.sendPostRequest<AuthResponseMessage>(user, Configuration.garageApiRegisterEndpoint);
    }

    public loginUser(userLoginForm: FormGroup): Observable<AuthResponseMessage>
    {
        var user = new UserLogin(
          userLoginForm.get('Email')?.value,
          userLoginForm.get('Password')?.value, 
          userLoginForm.get('RememberMe')?.value
        );
        return this._http.sendPostRequest<AuthResponseMessage>(user, Configuration.garageApiLoginEndpoint);
    }

    public changeUserPassword(userPasswordForm: FormGroup): Observable<AuthResponseMessage>
    {
      var changeDto = new UserChangeDto(
        userPasswordForm.get('OldPassword')?.value,
        userPasswordForm.get('Password')?.value
      );
      return this._http.sendPostRequest<AuthResponseMessage>(changeDto, Configuration.garageApiChangeUserPasswordEndpoint);
    }

    public logoutUser(): Observable<AuthResponseMessage>{
      return this._http.sendPostRequest<AuthResponseMessage>([], Configuration.garageApiLogoutEndpoint);
    }

    public setUserLoginFlag(setTo: boolean): void{
      this.isUserLoggedIn = setTo;
    }

    public checkIfUserIsLoggedIn(): boolean{
        return this.isUserLoggedIn;
    }

    public setSudoFlag(setTo: boolean): void{
      this.isUserTechnician = setTo;
    }

    public checkIfUserisTechnician(): boolean{
        return this.isUserTechnician;
    }
};
