import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { LoggerService } from "./logger.service";
import { Observable, Subject, catchError, tap } from "rxjs";
import { Injectable } from "@angular/core";
import { Configuration } from "../config/globalConfig";

@Injectable()
export class HTTPService
{   
    private _refresh$ = new Subject<void>();
    public get refreshed(){ return this._refresh$.asObservable(); }
    constructor(private _client: HttpClient, private _logger: LoggerService){}

    sendSingleGetRequest<T>(options: {}, endpoint: string, url: string = Configuration.garageApiURL) : Observable<T> {
        return this._client.get<T>(url+endpoint, options);
    }

    sendGetRequestForBlob(params: HttpParams, endpoint: string, url: string = Configuration.garageApiURL) : Observable<Blob> {
        return this._client.get(url+endpoint, {responseType : 'blob', params: params});
    }
    
    sendMultiGetRequest<T>(options: {}, endpoint: string, url: string = Configuration.garageApiURL) : Observable<T[]> {
        return this._client.get<T[]>(url+endpoint, options);
    }

    sendGetRequest<T>(endpoint:string, url: string = Configuration.garageApiURL) : Observable<T[]> {
        return this._client.get<T[]>(url+endpoint);
    }

    sendPostRequest<T>(body: any, endpoint: string, url: string = Configuration.garageApiURL) : Observable<T> {
        return this._client.post<T>(url+endpoint, body);
    }

    sendPostRequestWithQuery<T>(options: {}, body: any, endpoint: string, url: string = Configuration.garageApiURL) : Observable<T> {
        return this._client.post<T>(url+endpoint, body, options);
    }

    sendDeleteRequest<T>(options: {}, endpoint: string, url: string = Configuration.garageApiURL) : Observable<T> {
        return this._client.delete<T>(url+endpoint, options);
    }

    sendGetRequestRefreshed<T>(endpoint:string, url: string = Configuration.garageApiURL) : Observable<T[]> {
        return this._client.get<T[]>(url+endpoint)
                           .pipe(
                                tap(() => {
                                    this._refresh$.next();
                                })
                           );
    }
};