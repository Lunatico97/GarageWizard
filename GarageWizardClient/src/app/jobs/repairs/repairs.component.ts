import { ɵWebAnimationsStyleNormalizer } from '@angular/animations/browser';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Defaults } from 'src/app/config/defaults';
import { Job } from 'src/app/models/job';
import { Vehicle } from 'src/app/models/vehicle';
import { LoggerService } from 'src/app/services/logger.service';
import { ParkService } from 'src/app/services/park.service';
import { RepairService } from 'src/app/services/repair.service';
import { RouterService } from 'src/app/services/router.service';

@Component({
  selector: 'app-repairs',
  templateUrl: './repairs.component.html',
  styleUrls: ['./repairs.component.scss']
})
export class RepairsComponent implements OnInit {

  jobsData: Job[] = [];
  vehicles: Vehicle[] = [];
  selectedJobs: Job[] = [];
  loading: boolean = false;
  showConfirmationBox: boolean = false;
  imageFiles: any[]= [];
  tempTotal: number = 0;
  tempHours: number = 0;
  repairFormGroup!: FormGroup;

  constructor(private _logger: LoggerService, private _router: RouterService, private _formBuilder: FormBuilder, private _parkService: ParkService, private _repairService: RepairService) {}

  ngOnInit(): void {
    this.loading = true;
    this.repairFormGroup = this._formBuilder.group({
      vehicleID: ['', [Validators.required, Validators.pattern(Defaults.VehicleIDRegex)]],
    });
    this.getVehiclesNotOnRepair();
    this.getAllJobs();
  }

  toggleDisabled(job: Job): boolean{
    if(this.selectedJobs.includes(job)) return true;
    else return false;
  }

  onAddCallback(job: Job): void{
    this.selectedJobs.push(job);
    this.tempHours += job.hours;
    this.tempTotal += job.charge;
  }

  onRemoveCallback(job: Job): void{
    this.selectedJobs.splice(this.selectedJobs.indexOf(job), 1);
    this.tempHours -= job.hours;
    this.tempTotal -= job.charge;
  }

  onConfirmCallback(): void{
    this.showConfirmationBox = true;
  }

  onConfirmationCallback(){
    this.createRepairs();
    this.showConfirmationBox = false;
  }

  onDenialCallback(){
    this.showConfirmationBox = false;
  }

  createRepairs(): void{
    if(this.repairFormGroup.valid){
      this._repairService.createRepairs(this.repairFormGroup, this.selectedJobs)
        .subscribe({
          next: (value) => {
            if(value.success){
              this._logger.log("Repairs", value.message);
            }
          },
          error: (error) => {
            
          },
          complete: () => {
            this._logger.pop('Repairs added successfully !');
            this._router.routeToPath('jobs');
          }
        });
    }
  }

  getVehiclesNotOnRepair(): void{
    this._parkService.getVehiclesNotOnRepair()
            .subscribe({
                next: (response: Vehicle[]) => {
                  this.vehicles = response;
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

  getAllJobs(): void{
    this._repairService.getJobs()
      .subscribe({
        next: (response: Job[]) => {
            this.jobsData = response;
        },
        error: (error) => {
            this._logger.log(`Jobs [Error]`, error);
            this.loading = false;
        },
        complete: () => {
            this._logger.log(`Jobs [Complete]`, "GET request successful !");
            this.getAllImages();
            this.loading = false;
        }
    });
  }

  getImage(jobID: string): void
  {
    this._repairService.getUploadedJobImage(jobID)
      .subscribe({
        next: (response: Blob) => {
          const reader = new FileReader();
          reader.readAsDataURL(response);
          reader.onload = () => {
            this.imageFiles.push(reader.result as ArrayBuffer);
          }
        },
        complete : () => {
          this._logger.log('Images', 'Loaded successfully !');
        }
      });
  }

  getAllImages(): void
  {
    this.imageFiles = [];
    for(let job of this.jobsData)
    {
      this.getImage(job.id);
    }
  }
}
