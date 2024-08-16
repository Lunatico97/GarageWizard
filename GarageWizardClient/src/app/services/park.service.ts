import { Injectable } from "@angular/core";
import { HTTPService } from "./http.service";
import { LoggerService } from "./logger.service";
import { Configuration } from "../config/globalConfig";
import { Form, FormGroup } from "@angular/forms";
import { Vehicle } from "../models/vehicle";
import { HttpErrorResponse, HttpParams } from "@angular/common/http";
import { SpotService } from "./spot.service";
import { Observable } from "rxjs";
import { VehicleDto } from "../models/vehicleDto";
import { ServerReponseMessage } from "../models/server-response-message";

@Injectable()
export class ParkService
{   
    constructor(private _http: HTTPService, private _logger: LoggerService, private _spotService: SpotService){}

    public parkVehicle(parkFormGroup: FormGroup): Observable<ServerReponseMessage>
    {
        var vehicle = new Vehicle(
            parkFormGroup.get('name')?.value, parkFormGroup.get('vendor')?.value,
            parkFormGroup.get('wheels')?.value, parkFormGroup.get('regID')?.value
        );
        return this._http.sendPostRequest<ServerReponseMessage>(vehicle, Configuration.garageApiParkVehicleEndpoint)
    }

    public deleteVehicle(reqID: string): Observable<ServerReponseMessage>
    {
      const params = new HttpParams().set('reqID', reqID);
      return this._http.sendDeleteRequest<ServerReponseMessage>({'params': params}, Configuration.garageApiRemoveVehicleEndpoint);
    }

    public getAllVehicles(): Observable<Vehicle[]>
    {
      return this._http.sendGetRequest<Vehicle>(Configuration.garageApiListVehiclesEndpoint);
    }

    public getVehiclesNotOnRepair(): Observable<Vehicle[]>
    {
      return this._http.sendGetRequest<Vehicle>(Configuration.garageApiListVehiclesNotOnRepairEndpoint);
    }

    public getSpecificVehicle(reqID: string): Observable<VehicleDto>
    {
      const params = new HttpParams().set('reqID', reqID);
      return this._http.sendSingleGetRequest<VehicleDto>({'params': params}, Configuration.garageApiGetSpecificVehicleEndpoint);
    }
};
