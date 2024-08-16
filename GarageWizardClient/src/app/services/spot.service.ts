import { Injectable } from "@angular/core";
import { HTTPService } from "./http.service";
import { LoggerService } from "./logger.service";
import { Configuration } from "../config/globalConfig";
import { FormGroup } from "@angular/forms";
import { Reservation } from "../models/reservation";
import { Spot } from "../models/spot";
import { HttpErrorResponse, HttpParams } from "@angular/common/http";
import { RouterService } from "./router.service";
import { Observable } from "rxjs";
import { ServerReponseMessage } from "../models/server-response-message";

@Injectable()
export class SpotService
{   
    public backendErrors: any;
    constructor(private _http: HTTPService, private _logger: LoggerService, private _router: RouterService){}

    reserveSpot(vehicleFormGroup: FormGroup, reserveFormGroup: FormGroup): Observable<any>
    {
      var reservation = new Reservation(
        reserveFormGroup.get('spotID')?.value,
        vehicleFormGroup.get('regID')?.value,
        reserveFormGroup.get('service')?.value
      );
      return this._http.sendPostRequest<any>(reservation, Configuration.garageApiReserveSpotEndpoint);
    }

    public createSpot(spotFormGroup: FormGroup): Observable<ServerReponseMessage>{
        var newSpot = new Spot(
            spotFormGroup.get('spotID')?.value,
            spotFormGroup.get('capacity')?.value,
        );
        return this._http.sendPostRequest<ServerReponseMessage>(newSpot, Configuration.garageApiCreateSpotEndpoint);
    }

    public getSpots(): Observable<Spot[]>{
      return this._http.sendGetRequest<Spot>(Configuration.garageApiListSpotsEndpoint);
    }

    public getVacantSpot(parkFormGroup: FormGroup): Observable<Spot[]>
    {
      let vecID = parkFormGroup.get('regID')?.value;
      let wheels = parkFormGroup.get('wheels')?.value;
      return this._http.sendGetRequest<Spot>(Configuration.garageApiVacantSpotCheckEndpoint + `/${vecID}-${wheels}`);
    }

    public deleteSpot(spotID: string): Observable<ServerReponseMessage>
    {
      const params = new HttpParams().set('spotID', spotID);
      return this._http.sendDeleteRequest<ServerReponseMessage>({'params': params}, Configuration.garageApiRemoveSpotEndpoint);
    }

};
