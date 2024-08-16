import { Routes } from "@angular/router";
import { DashboardComponent } from "./dashboard/dashboard.component";
import { ListVehicleComponent } from "./vehicles/list-vehicle/list-vehicle.component";
import { RepairsComponent } from "./jobs/repairs/repairs.component";
import { CombinedComponent } from "./account/combined/combined.component";
import { LoginComponent } from "./account/login/login.component";
import { RegisterComponent } from "./account/register/register.component";
import { ListSpotComponent } from "./spots/list-spot/list-spot.component";
import { ListRoleComponent } from "./roles/list-role/list-role.component";
import { ParkVehicleComponent } from "./vehicles/park-vehicle/park-vehicle.component";
import { AuthGuardService } from "./services/authguard.service";
import { AccessDeniedComponent } from "./access-denied/access-denied.component";
import { TransactionsComponent } from "./jobs/transactions/transactions.component";
import { SelectorComponent } from "./jobs/selector/selector.component";
import { ListJobsComponent } from "./jobs/list-jobs/list-jobs.component";
import { ServerDownComponent } from "./server-down/server-down.component";

export const routes: Routes = [
    {path: '', redirectTo: 'home', pathMatch: 'full'},
    {path: 'access-denied', component: AccessDeniedComponent},
    {path: 'home', component: DashboardComponent},
    {path: 'account', component: CombinedComponent, 
        children: [
            { path: '', redirectTo: 'login', pathMatch: 'full' },
            { path: 'login', component:  LoginComponent },
            { path: 'register', component: RegisterComponent },
        ]
    },
    {path: 'vehicles', canActivate: [AuthGuardService],
        children: [
            { path: '', redirectTo: 'info', pathMatch: 'full' },
            { path: 'info', component:  ListVehicleComponent },
            { path: 'park', component:  ParkVehicleComponent },
        ]
    },
    {path: 'spots', canActivate: [AuthGuardService],
        children: [
            { path: '', redirectTo: 'info', pathMatch: 'full' },
            { path: 'info', component:  ListSpotComponent },
        ]
    },
    {path: 'jobs', component: SelectorComponent, canActivate: [AuthGuardService],
        children: [
            { path: '', redirectTo: 'transactions', pathMatch: 'full' },
            { path: 'transactions', component:  TransactionsComponent },
            { path: 'repairs', component: RepairsComponent },
            { path: 'info', component: ListJobsComponent },
        ]
    },
    {path: 'roles', canActivate: [AuthGuardService],
        children: [
            { path: '', redirectTo: 'info', pathMatch: 'full' },
            { path: 'info', component:  ListRoleComponent },
        ]
    },
    {path: 'server-down', component: ServerDownComponent}
];