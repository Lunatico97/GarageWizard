import { Injectable } from "@angular/core";
import { HTTPService } from "./http.service";
import { LoggerService } from "./logger.service";
import { FormGroup } from "@angular/forms";
import { Observable } from "rxjs";
import { Repair } from "../models/repair";
import { Job } from "../models/job";
import { Configuration } from "../config/globalConfig";
import { HttpErrorResponse, HttpParams } from "@angular/common/http";
import { RepairTransaction } from "../models/repair-transaction";
import { ServerReponseMessage } from "../models/server-response-message";

@Injectable()
export class RepairService
{
    constructor(private _http: HTTPService, private _logger: LoggerService){}

    public createRepairs(repairFormGroup: FormGroup, selectedJobs: Job[]): Observable<ServerReponseMessage>
    {
        var repair = new Repair(repairFormGroup.get('vehicleID')?.value, selectedJobs);
       return this._http.sendPostRequest<ServerReponseMessage>(repair, Configuration.garageApiCreateRepairsEndpoint)
    }

    public addJob(jobFormGroup: FormGroup): Observable<ServerReponseMessage>
    {
        var job = new Job(
            jobFormGroup.get('name')?.value, jobFormGroup.get('description')?.value, jobFormGroup.get('charge')?.value,
            jobFormGroup.get('hours')?.value, jobFormGroup.get('jobImagePath')?.value
        );
        return this._http.sendPostRequest<ServerReponseMessage>(job, Configuration.garageApiCreateJobEndpoint);
    }

    public getJobs(): Observable<Job[]>
    {
        return this._http.sendGetRequest<Job>(Configuration.garageApiListJobsEndpoint);
    }

    public getRepairTransactions(vehicleID: string): Observable<RepairTransaction[]>
    {
        const params = new HttpParams().set('vehicleID', vehicleID);
        return this._http.sendMultiGetRequest<RepairTransaction>({'params': params}, Configuration.garageApiListRepairsEndpoint);
    }

    public completeRepairTransactions(vehicleID: string): Observable<ServerReponseMessage>
    {
        const params = new HttpParams().set('vehicleID', vehicleID);
        return this._http.sendPostRequestWithQuery<ServerReponseMessage>({"params": params}, [], Configuration.garageApiCompleteRepairsEndpoint);
    }

    public getUploadedJobImage(jobID: string): Observable<Blob>
    {
        const params = new HttpParams().set('jobID', jobID);
        return this._http.sendGetRequestForBlob(params, Configuration.garageApiGetUploadedJobImage);
    }

    public uploadJobImage(file: FormData): Observable<ServerReponseMessage>
    {
        return this._http.sendPostRequest<ServerReponseMessage>(file, Configuration.garageApiUploadJobImage);
    }

    public deleteJob(jobID: string): Observable<ServerReponseMessage>
    {
        const params = new HttpParams().set('jobID', jobID);
        return this._http.sendDeleteRequest<ServerReponseMessage>({'params': params}, Configuration.garageApiRemoveJobEndpoint);
    }
}