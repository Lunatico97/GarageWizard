import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject, Subscription, tap } from 'rxjs';
import { Defaults } from 'src/app/config/defaults';
import { Configuration } from 'src/app/config/globalConfig';
import { Spot } from 'src/app/models/spot';
import { AccountService } from 'src/app/services/account.service';
import { HTTPService } from 'src/app/services/http.service';
import { LoggerService } from 'src/app/services/logger.service';
import { RouterService } from 'src/app/services/router.service';
import { SpotService } from 'src/app/services/spot.service';

@Component({
  selector: 'app-list-spot',
  templateUrl: './list-spot.component.html',
  styleUrls: ['./list-spot.component.scss']
})
export class ListSpotComponent implements OnInit {

  spotsData: Spot[] = [];
  loading: boolean = false;
  spotFormGroup!: FormGroup;
  addSpotFailed: boolean = false;
  key: string = "id";
  message: string = "";

  constructor(private _logger: LoggerService, public _router: RouterService, private _formBuilder: FormBuilder, private _spotService: SpotService,
    public _accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.spotFormGroup = this._formBuilder.group({
      spotID: ['', [Validators.required, Validators.pattern(Defaults.SpotIDRegex)]],
      capacity: ['', [Validators.required]]
    });
    this.getAllSpots();
  }

  onAddSpotCallback(): void {
    if(this.spotFormGroup.valid){
      this.addSpots();
      if(!this.addSpotFailed){
        this._logger.pop(`Added a spot ${this.spotFormGroup.get('spotID')?.value} !`);
        this.spotsData.push(this.spotFormGroup.value);
        this.spotFormGroup.reset();
        Object.keys(this.spotFormGroup.controls).forEach(key =>{
          this.spotFormGroup.controls[key].setErrors(null);
        });
      }
      else this._logger.pop(this.message);
    }
    console.log(this.spotFormGroup);
  }  

  onDelete = (reqID: string) => {
    this._spotService.deleteSpot(reqID)
      .subscribe({
        next: (value) => {
          this._logger.log("SpotService [Deletion]", value.message);
          this._logger.pop(value.message as string);
        },
        error: (errors) => {
          this._logger.log("SpotService [Error]", errors.message);
        },
        
        complete: () => {
          this._logger.log("SpotService", "Successful DELETE request");
          this.getAllSpots();
        }
      });
      this._logger.pop(`Deleted a spot with ID ${reqID} !`);
  }

  addSpots(): void{
    this._spotService.createSpot(this.spotFormGroup).subscribe({
            next: (value) => {
              this._logger.log("SpotService [Spot Creation]", value.message);
              this.addSpotFailed = value.success;
              this.message = value.message;
            },
            error: (errors) => {
              this._logger.log("SpotService [Error]", errors.message);
            },
            
            complete: () => {
              this._logger.log("SpotService", "Succesful POST request");
              this.getAllSpots();
            }
          });
  }

  getAllSpots(): void{
    this.loading = true;
    this._spotService.getSpots()
                .subscribe({
                next: (response: Spot[]) => {
                    this.spotsData = response;
                },
                error: (error) => {
                    this._logger.log(`Spots [Error]`, error);
                    this.loading = false;
                },
                complete: () => {
                    this._logger.log(`Spots [Complete]`, "GET request successful !");
                    this.loading = false;
                }
            });
  }
}
