import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { AccountService } from 'src/app/services/account.service';
import { LoggerService } from 'src/app/services/logger.service';
import { PasswordMatchingValidator } from 'src/app/utils/passwordMatching.validator';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {
  changePasswordForm!: FormGroup;
  constructor(private _logger: LoggerService, private _accountService: AccountService, private _dialogRef: MatDialogRef<ChangePasswordComponent>) { }

  ngOnInit(): void {
    this.changePasswordForm = new FormGroup(
        {
          OldPassword : new FormControl('', [Validators.required]),
          Password: new FormControl('', [Validators.required, Validators.minLength(8)]),
          ConfirmPassword: new FormControl('', [Validators.required]),
        }, {validators: PasswordMatchingValidator}
      );
  }

  onChangeCallback(): void{
    if(this.changePasswordForm.valid){
      this._accountService.changeUserPassword(this.changePasswordForm)
        .subscribe({
            next: (value) => {
              this._logger.log("Password Change", value.message);
              this._logger.pop(value.message);
            },
            complete: () => {
              this._logger.log("Password Change", "POST request successful !!");
            }
        });

      this._dialogRef.close();
    }
    console.log(this.changePasswordForm);
  }

}
