
import {Injectable} from "@angular/core";
import {HttpEvent, HttpHandler, HttpInterceptor,HttpRequest, HttpStatusCode} from "@angular/common/http";
import { Observable, catchError, of } from "rxjs";
import { Defaults } from "../config/defaults";
import { Router } from "@angular/router";
import { AccountService } from "./account.service";
 
@Injectable()
export class InterceptorService implements HttpInterceptor {
    constructor(private _router: Router, private _account: AccountService) {
    }
 
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>>{
        if(this._account.checkIfUserIsLoggedIn()){
            request = request.clone({ headers: request.headers.set('Authorization', 'Bearer ' + sessionStorage.getItem('JWT'))}); 
            return next.handle(request).pipe(
                catchError(
                (error, caught) => {
                        if(error.status === HttpStatusCode.Unauthorized || error.status === HttpStatusCode.Forbidden){
                            this._router.navigateByUrl('access-denied');
                        }
                        console.log(error);
                        return of(error);
                    }
                )
            );
        }
        else return next.handle(request);
    }
}
 