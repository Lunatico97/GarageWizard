import { Injectable } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { LoggerService } from "./logger.service";

@Injectable()
export class RouterService 
{
    constructor(private _router: Router, private _logger: LoggerService){}
    
    public routeToPath(route: string)
    {
        this._router.navigateByUrl(route);
        this._logger.log("RouterService", `Routed to ${route}`);
    }

    public getReturnURL(route: ActivatedRoute) : string | null{
        let returnUrl = null;
        route.queryParamMap.subscribe(
            params => {
                returnUrl = params.get('returnUrl');
            }
        );
        return returnUrl;
    }

    public routeToReturnURL(returnURL: string | null): void{
      if(returnURL === null){
        this.routeToPath('');
      }
      else this.routeToPath(returnURL);
    }
}