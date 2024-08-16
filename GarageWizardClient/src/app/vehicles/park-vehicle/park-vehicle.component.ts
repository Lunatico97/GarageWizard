import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Defaults } from 'src/app/config/defaults';
import { Service } from 'src/app/config/enums';
import { ServerReponseMessage } from 'src/app/models/server-response-message';
import { Spot } from 'src/app/models/spot';
import { HTTPService } from 'src/app/services/http.service';
import { LoggerService } from 'src/app/services/logger.service';
import { ParkService } from 'src/app/services/park.service';
import { RouterService } from 'src/app/services/router.service';
import { SpotService } from 'src/app/services/spot.service';

@Component({
  selector: 'app-park-vehicle',
  templateUrl: './park-vehicle.component.html',
  styleUrls: ['./park-vehicle.component.scss']
})
export class ParkVehicleComponent implements OnInit {

  isLinear: boolean = true;
  nextDisabled: boolean = true;
  loading: boolean = false;
  reserveFail: boolean = false;
  parkHoldToReserve: boolean = false;
  vacantSpots: Spot[] = [];
  parkFormGroup!: FormGroup;
  reserveFormGroup!: FormGroup;
  serviceClass: string[] = Object.keys(Service).splice(2, 2);
  
  constructor(private _formBuilder: FormBuilder, private _http: HTTPService, public _router: RouterService,
    private _logger: LoggerService, private _parkService: ParkService, private _spotService: SpotService, private _snackbar: MatSnackBar) {}

  ngOnInit() {
    this.parkFormGroup = this._formBuilder.group({
      name: ['', [Validators.required, Validators.pattern(Defaults.VehicleStringRegex)]],
      vendor: ['', [Validators.required, Validators.pattern(Defaults.VehicleStringRegex)]],
      regID: ['', [Validators.required, Validators.pattern(Defaults.VehicleIDRegex)]],
      wheels: ['4', [Validators.required]],
    });
    this.reserveFormGroup = this._formBuilder.group({
      spotID: ['', Validators.required],
      service: ['0', Validators.required]
    });
  }

  public getVacantSpots(): void{
    this.loading = true;
    this._spotService.getVacantSpot(this.parkFormGroup)
            .subscribe({
                next: (response: Spot[]) => {
                    this.vacantSpots = response;
                },
                error: (error) => {
                    this._logger.log(`Vacant Spots [Error]`, error);
                    this.loading = false;
                },
                complete: () => {
                    this._logger.log(`Vacant Spots [Complete]`, "GET request successful !");
                    this.loading = false;
                }
            });
    }


  onParkCallback(): void{
    if(!this.nextDisabled && this.parkFormGroup.valid)
    {
      this._parkService.parkVehicle(this.parkFormGroup)
          .subscribe({
            next: (value: ServerReponseMessage) => {
              if(value.success)
              {
                this.parkHoldToReserve = true;
              }
              this._logger.log("ParkService [Vehicle]", value.message);
            },
            error: (errors) => {
              this._logger.log("ParkService [Error]", errors.message);
            },
            
            complete: () => {
              this._logger.log("ParkService", "Succesful POST request");
            }
        });
        this.getVacantSpots();
    }
    console.log(this.parkFormGroup)
  }

  onReserveCallback(): void{
    if(this.parkHoldToReserve && this.reserveFormGroup.valid){
      this._spotService.reserveSpot(this.parkFormGroup, this.reserveFormGroup)
      .subscribe({
        next: (value: ServerReponseMessage) => {
          if(!value.success)
          {
            this._logger.log("SpotService [Reservation]", value.message);
            this.reserveFail = true;
          }
          //parkFormGroup.reset();
          //reserveFormGroup.reset();
        },
        error: (errors) => {
          this._logger.log("SpotService [Error]", errors.message);
        },
        
        complete: () => {
          this._logger.log("SpotService", "Succesful POST request");
        }
    });
    }
    console.log(this.reserveFormGroup);
  }

  onConfirmCallback(): void{
    if(this.reserveFail){
      this._parkService.deleteVehicle(this.parkFormGroup.get('regID')?.value)
        .subscribe({
          next: (value: ServerReponseMessage) => {
            this._logger.log("ParkService [From server]", value.message);
            this._logger.pop(value.message as string);
          },
          error: (errors) => {
            this._logger.log("ParkService [Error]", errors.message);
          },
          
          complete: () => {
            this._logger.log("ParkService", "Successful DELETE request");
          }
        });

    }
    else this._logger.pop(`Parked a new vehicle ${this.parkFormGroup.get('regID')?.value} !`);
    this._router.routeToPath('vehicles');
  }

  onBackCallback(): void{
    if(this.parkHoldToReserve){
      this.reserveFail = true;
      this.onConfirmCallback();
    }
    this._router.routeToPath('vehicles');
  }

  ngDoCheck(): void{
    if(!this.parkFormGroup.pristine && this.parkFormGroup.valid)
    {
      this.nextDisabled = false;
    }
    else this.nextDisabled = true;
  }

}
