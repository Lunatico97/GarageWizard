import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthResponseMessage } from 'src/app/models/auth-response-message';
import { ServerReponseMessage } from 'src/app/models/server-response-message';
import { AccountService } from 'src/app/services/account.service';
import { LoggerService } from 'src/app/services/logger.service';
import { RouterService } from 'src/app/services/router.service';

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  userLoginForm!: FormGroup;

  constructor(private _accountService: AccountService, private _router: RouterService, private _logger: LoggerService,
     private activatedRoute: ActivatedRoute) { }

  ngOnInit(): void {
    this.userLoginForm = new FormGroup(
      {
        Email : new FormControl('', [Validators.required]),
        Password: new FormControl('', [Validators.required, Validators.minLength(8)]),
        RememberMe: new FormControl(false)
      }, 
    );
    
  }

  onLoginCallback(): void {
    if(this.userLoginForm.valid)
    {
      this._accountService.loginUser(this.userLoginForm)
          .subscribe({
            next: (value: AuthResponseMessage) => {
              if(value.loggedIn)
              {
                this._accountService.setUserLoginFlag(true);
                sessionStorage.setItem('username', value.username);
                sessionStorage.setItem('JWT', value.token);
                this._logger.log("AccountService [JWT]", value.token);
                this._logger.pop("Logged in successfully !");
                let url = this._router.getReturnURL(this.activatedRoute);
                this._router.routeToReturnURL(url);
              }
              else{
                this._logger.log("AccountService [Login]", value.message);
                this._logger.pop(value.message);
                this.userLoginForm.setErrors({message: value.message});
              } 
            },
            error: (errors) => {
              this._logger.log("AccountService [Login Error]", errors.message);
            },
            complete: () => {
              this._logger.log("AccountService [Login]", "Successful POST request");
            }
          }); 
    }
    console.log(this.userLoginForm);
  }
}
