import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Configuration } from 'src/app/config/globalConfig';
import { Vehicle } from 'src/app/models/vehicle';
import { VehicleDto } from 'src/app/models/vehicleDto';
import { AccountService } from 'src/app/services/account.service';
import { HTTPService } from 'src/app/services/http.service';
import { LoggerService } from 'src/app/services/logger.service';
import { ParkService } from 'src/app/services/park.service';
import { RouterService } from 'src/app/services/router.service';
import { SpotService } from 'src/app/services/spot.service';

@Component({
  selector: 'list-vehicle',
  templateUrl: './list-vehicle.component.html',
  styleUrls: ['./list-vehicle.component.scss']
})
export class ListVehicleComponent implements OnInit {

  vehiclesData: Vehicle[] = [];
  specificVehicle!: VehicleDto;
  viewSpecific: boolean = false;
  loading: boolean = false;
  key: string = "regID";

  constructor(private _logger: LoggerService, public _router: RouterService, private _parkService: ParkService, private _spotService: SpotService,
    private _accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.getAllVehicles();
  }

  onDelete = (reqID: string) => {
    this._parkService.deleteVehicle(reqID)
      .subscribe({
        next: (value) => {
           this._logger.log("ParkService [From server]", value.message);
           this._logger.pop(value.message);
        },
        error: (errors) => {
          this._logger.log("ParkService [Error]", errors.message);
        },
        
        complete: () => {
          this._logger.log("ParkService", "Successful DELETE request");
          this.getAllVehicles();
        }
      });  
  }

  onView = (reqID: string) => {
    this._parkService.getSpecificVehicle(reqID)
          .subscribe({
            next: (response: VehicleDto) => {
              this.specificVehicle = response;
              this.viewSpecific = true;
            },
            error: (error) => {
                this._logger.log(`Vehicles [Error]`, error);
            },
            complete: () => {
                this._logger.log(`Vehicles [Complete]`, "GET request successful !");
            }
        });
  }

  getAllVehicles(): void {
    this.loading = true;
    this._parkService.getAllVehicles()
            .subscribe({
                next: (response: Vehicle[]) => {
                  this.vehiclesData = response;
                },
                error: (error) => {
                    this._logger.log(`Vehicles [Error]`, error);
                    this.loading = false;
                },
                complete: () => {
                    this._logger.log(`Vehicles [Complete]`, "GET request successful !");
                    this.loading = false;
                }
            });
  }

  checkSudo(): boolean{
    return this._accountService.checkIfUserisTechnician();
  }
}
