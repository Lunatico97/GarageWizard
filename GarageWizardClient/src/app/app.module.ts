import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

// Materials
import {MatButtonModule} from '@angular/material/button';
import {MatTabsModule} from '@angular/material/tabs';
import {MAT_SNACK_BAR_DEFAULT_OPTIONS, MatSnackBarModule} from '@angular/material/snack-bar'
import {MatSelectModule} from '@angular/material/select';
import {MatInputModule} from '@angular/material/input';
import {MatCardModule} from '@angular/material/card';
import {MatTableModule} from '@angular/material/table';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatStepperModule} from '@angular/material/stepper';
import {MatDialogModule} from '@angular/material/dialog';
import {MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldModule} from '@angular/material/form-field';

// Custom components, pipes, modules and services
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RangePipe } from './utils/range.pipe';
import { CapitalizePipe } from './utils/capitalize.pipe';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ListStuffComponent } from './list-stuff/list-stuff.component';
import { RegisterComponent } from './account/register/register.component';
import { LoginComponent } from './account/login/login.component';
import { CombinedComponent } from './account/combined/combined.component';
import { NavbarComponent } from './navbar/navbar.component';
import { HTTPService } from './services/http.service';
import { AccountService } from './services/account.service';
import { LoggerService } from './services/logger.service';
import { RouterService } from './services/router.service';
import { ListVehicleComponent } from './vehicles/list-vehicle/list-vehicle.component';
import { RepairsComponent } from './jobs/repairs/repairs.component';
import { ListSpotComponent } from './spots/list-spot/list-spot.component';
import { ListRoleComponent } from './roles/list-role/list-role.component';
import { ParkVehicleComponent } from './vehicles/park-vehicle/park-vehicle.component';
import { ParkService } from './services/park.service';
import { MatIconModule } from '@angular/material/icon';
import { InterceptorService } from './services/interceptor.service';
import { AuthGuardService } from './services/authguard.service';
import { AccessDeniedComponent } from './access-denied/access-denied.component';
import { SpotService } from './services/spot.service';
import { TransactionsComponent } from './jobs/transactions/transactions.component';
import { RepairService } from './services/repair.service';
import { SelectorComponent } from './jobs/selector/selector.component';
import { RoleService } from './services/role.service';
import { ListJobsComponent } from './jobs/list-jobs/list-jobs.component';
import { RoundPipe } from './utils/round.pipe';
import { ChangePasswordComponent } from './dashboard/change-password/change-password.component';
import { ErrorInterceptorService } from './services/error.interceptor.service';

@NgModule({
  declarations: [
    AppComponent,
    RangePipe,
    CapitalizePipe,
    RoundPipe,
    DashboardComponent,
    ListStuffComponent,
    RegisterComponent,
    LoginComponent,
    CombinedComponent,
    NavbarComponent,
    ListVehicleComponent,
    RepairsComponent,
    TransactionsComponent,
    ListSpotComponent,
    ListRoleComponent,
    ParkVehicleComponent,
    AccessDeniedComponent,
    SelectorComponent,
    ListJobsComponent,
    ChangePasswordComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MatButtonModule,
    MatTabsModule,
    MatCardModule,
    MatSelectModule,
    MatInputModule,
    MatTableModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatStepperModule,
  ],
  // Order of providers is very critical and sometimes it goes unnoticed - Diwas
  providers: [ LoggerService, HTTPService, RouterService, AccountService, AuthGuardService, 
    {provide: HTTP_INTERCEPTORS, useClass: InterceptorService, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptorService, multi: true},
    {provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: {appearance: 'outline'}},
    //{provide: MAT_SNACK_BAR_DEFAULT_OPTIONS, useValue: {duration: 2500}},
    AccountService, ParkService, SpotService, RepairService, RoleService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
