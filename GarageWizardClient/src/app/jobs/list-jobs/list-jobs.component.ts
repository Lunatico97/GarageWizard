import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Defaults } from 'src/app/config/defaults';
import { Job } from 'src/app/models/job';
import { LoggerService } from 'src/app/services/logger.service';
import { RepairService } from 'src/app/services/repair.service';
import { RouterService } from 'src/app/services/router.service';

@Component({
  selector: 'app-list-jobs',
  templateUrl: './list-jobs.component.html',
  styleUrls: ['./list-jobs.component.scss']
})
export class ListJobsComponent implements OnInit {

  jobFormGroup!: FormGroup;
  jobIDControl!: FormControl;
  jobsData!: Job[];
  imageFile!: File;
  constructor(private _logger: LoggerService, private _router: RouterService, private _formBuilder: FormBuilder, private _repairService: RepairService) { }

  ngOnInit(): void {
    this.jobFormGroup = this._formBuilder.group({
      name: ['', [Validators.required, Validators.pattern(Defaults.VehicleStringRegex)]],
      description: ['', [Validators.required, Validators.maxLength(100)]],
      charge: ['', [Validators.required]],
      hours: ['', [Validators.required]],
      jobImagePath: ['']
    });
    this.jobIDControl = new FormControl('', [Validators.required]);
    this.getAllJobs();
  }

  onAddJobCallback(): void{
    if(this.jobFormGroup.valid){
      this._repairService.addJob(this.jobFormGroup)
      .subscribe({
        next: (value) => {
          this._logger.log("Jobs [Create]", value.message);
          if(value.success)
          {
            this._logger.pop(value.message);
          }
        },
        error: (errors) => {
          this._logger.log("Jobs [Error]", errors.message);
        },
        
        complete: () => {
          this._logger.log("Jobs", "Succesful POST request");
          this.onUploadCallback();
          this._router.routeToPath('jobs/repairs');
        }
      });
    }
    console.log(this.jobFormGroup)
  }

  onUploadCallback(){
    const formData = new FormData();
    formData.append('file', this.imageFile);
    this._repairService.uploadJobImage(formData)
      .subscribe({
        next: (value) => {
          if(value.success)
          {
            this._logger.log("Uploader", value.message);
            this._logger.pop('New job added successfully !');
          }
        }
      });
  }

  onDeleteCallback(){
    this._repairService.deleteJob(this.jobIDControl.value)
    .subscribe({
      next: (value) => {
        if(value.success)
        {
          this._logger.log("Jobs", value.message);
          this._logger.pop("Job deleted successfully !");
          this._router.routeToPath('jobs/repairs');
        }
      }
    });
  }

  onFileChange(event: any) {
    if(event.target instanceof HTMLInputElement && event.target.files.length > 0)
    {
      this.imageFile = event.target.files[0] as File;
      this.jobFormGroup.patchValue({
        jobImagePath: this.imageFile.name
      });
    }
  }

  getAllJobs(): void{
    this._repairService.getJobs()
      .subscribe({
        next: (response: Job[]) => {
            this.jobsData = response;
        },
        error: (error) => {
            this._logger.log(`Jobs [Error]`, error);
        },
        complete: () => {
            this._logger.log(`Jobs [Complete]`, "GET request successful !");
        }
    });
  }
}
