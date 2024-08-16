import { Injectable } from "@angular/core";
import { HTTPService } from "./http.service";
import { LoggerService } from "./logger.service";
import { RouterService } from "./router.service";
import { Configuration } from "../config/globalConfig";
import { Observable } from "rxjs";
import { Role } from "../models/role";
import { RoleDetails } from "../models/role-details";
import { HttpParams } from "@angular/common/http";
import { RoleAccess } from "../models/role-access";
import { User } from "../models/user";

@Injectable()
export class RoleService
{   
    constructor(private _http: HTTPService, private _logger: LoggerService, private _router: RouterService){}

    public getAllRoles(): Observable<Role[]>
    {
        return this._http.sendGetRequest<Role>(Configuration.garageApiListRolesEndpoint);
    }

    public getRoleDetails(roleID: string): Observable<RoleDetails>
    {
        const params = new HttpParams().set('roleID', roleID);
        return this._http.sendSingleGetRequest<RoleDetails>({'params': params}, Configuration.garageApiGetRoleDetailsEndpoint);
    }

    public editRoleAccess(roleToEdit: string, userList: User[], selectedList: string[]): Observable<any>
    {
        var accessBag: RoleAccess[] = [];
        for(let user of userList){
            if(selectedList.includes(user.email)){
                accessBag.push(new RoleAccess(user.email, roleToEdit, true));
            } 
            else accessBag.push(new RoleAccess(user.email, roleToEdit, false));
        }
        return this._http.sendPostRequest<any>(accessBag, Configuration.garageApiEditRoleAccessEndpoint);
    }
};
