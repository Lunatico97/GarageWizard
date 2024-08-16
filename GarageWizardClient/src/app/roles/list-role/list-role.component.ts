import { HttpParams } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Configuration } from 'src/app/config/globalConfig';
import { Role } from 'src/app/models/role';
import { RoleDetails } from 'src/app/models/role-details';
import { ServerReponseMessage } from 'src/app/models/server-response-message';
import { User } from 'src/app/models/user';
import { AccountService } from 'src/app/services/account.service';
import { HTTPService } from 'src/app/services/http.service';
import { LoggerService } from 'src/app/services/logger.service';
import { RoleService } from 'src/app/services/role.service';
import { RouterService } from 'src/app/services/router.service';

@Component({
  selector: 'app-list-role',
  templateUrl: './list-role.component.html',
  styleUrls: ['./list-role.component.scss']
})
export class ListRoleComponent implements OnInit {
  rolesData: Role[] = [];
  roleDetails!: RoleDetails;
  usersWithRoleAccess: string[] = [];
  preChecked: string[] = [];
  selectedUsers: string[] = [];
  allUsers: User[] = [];
  roleToEditAccess!: string;
  loading: boolean = false;
  viewRoleUsers : boolean = false;

  constructor(private _logger: LoggerService, public _router: RouterService, private _accountService: AccountService, private _roleService: RoleService) {}

  ngOnInit(): void {
    this.loading = true;
    this._roleService.getAllRoles()
            .subscribe({
                next: (response: Role[]) => {
                    this.rolesData = response;
                },
                error: (error) => {
                    this._logger.log(`Roles [Error]`, error);
                    this.loading = false;
                },
                complete: () => {
                    this._logger.log(`Roles [Complete]`, "GET request successful !");
                    this.loading = false;
                }
            });
  }

  getAssociatedIDs(roleID: string): void{
    this._roleService.getRoleDetails(roleID)
            .subscribe({
                next: (response: RoleDetails) => {
                    this.roleDetails = response;
                    this.usersWithRoleAccess = this.roleDetails.accessUserIDs;
                    this.roleToEditAccess = this.roleDetails.name;
                    this.viewRoleUsers = true;
                },
                error: (error) => {
                    this._logger.log(`Roles [Error]`, error);
                    this.loading = false;
                },
                complete: () => {
                    this._logger.log(`Roles [Complete]`, "GET request successful !");
                    this.loading = false;
                }
            });
  }

  getAllUsers(): void{
    this._accountService.getAllUsers()
            .subscribe({
                next: (response: User[]) => {
                    this.allUsers = response;
                },
                error: (error) => {
                    this._logger.log(`Users [Error]`, error);
                    this.loading = false;
                },
                complete: () => {
                    this._logger.log(`Users [Complete]`, "GET request successful !");
                    this.loading = false;
                }
            });
  }

  editSpecificRoleAccess(): void{
    this._roleService.editRoleAccess(this.roleToEditAccess, this.allUsers, this.selectedUsers)
        .subscribe({
          next: (value: ServerReponseMessage) => {
              this._logger.log("RoleService [Edit Access]", value.message);
              if(value.success){
                this._logger.pop(value.message);
              }
          },
          error: (error) => {
              this._logger.log(`RoleService [Edit Access]`, error);
          },
          complete: () => {
              this._logger.log(`RoleService [Edit Access]`, "POST request successful !");
              this.viewRoleUsers = false;
          }
      });
  }

  toggleUserAccess(user: string): void{
    if(this.checkIfUserIsSelected(user)){
      this.selectedUsers.splice(this.selectedUsers.indexOf(user), 1);
    }
    else this.selectedUsers.push(user);
  }

  onViewCallback(roleID: string){
    this.selectedUsers = [];
    this.usersWithRoleAccess = [];
    this.getAssociatedIDs(roleID);
    this.getAllUsers();
  }

  onEditAccessCallback(): void{
    this.editSpecificRoleAccess();
  }

  checkIfUserHasAccess(user: string): boolean{
    return this.usersWithRoleAccess.includes(user);
  }

  checkIfUserIsSelected(user: string): boolean{
    return this.selectedUsers.includes(user);
  }
}
