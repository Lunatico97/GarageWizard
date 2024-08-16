import { Component, OnInit, ɵɵsanitizeUrlOrResourceUrl } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PasswordMatchingValidator } from '../../utils/passwordMatching.validator';
import { AccountService } from 'src/app/services/account.service';
import { RouterService } from 'src/app/services/router.service';
import { LoggerService } from 'src/app/services/logger.service';
import { Defaults } from 'src/app/config/defaults';
import { AuthResponseMessage } from 'src/app/models/auth-response-message';

@Component({
  selector: 'register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

  userRegisterForm!: FormGroup;
  error!: string;

  constructor(private _accountService: AccountService, private _logger: LoggerService, private _router: RouterService) { }

  ngOnInit(): void {
    this.userRegisterForm = new FormGroup(
      {
        Name: new FormControl('', [Validators.required, Validators.maxLength(20)]),
        Email : new FormControl('', [Validators.required, Validators.pattern(Defaults.EmailRegex)]),
        Password: new FormControl('', [Validators.required, Validators.minLength(8)]),
        ConfirmPassword: new FormControl('', [Validators.required, Validators.minLength(8)])
      }, {validators: PasswordMatchingValidator}
    );
  }

  onRegisterCallback(): void {
    if(this.userRegisterForm.valid)
    {
      this._accountService.registerUser(this.userRegisterForm)
        .subscribe({
          next: (value: AuthResponseMessage) => {
            if(value.registered)
            {
              this.userRegisterForm.reset();
              this._router.routeToPath('account');
            }
            this._logger.log("AccountService [From server]", value.message);
            this._logger.pop(value.message);
          },
          error: (errors) => {
              this._logger.log("AccountService [Error]", errors.message);
          },
          complete: () => {
            this._logger.log("AccountService [Register]", "Succesful POST request");
          }
      });
    }
    console.log(this.userRegisterForm);
  }

}
