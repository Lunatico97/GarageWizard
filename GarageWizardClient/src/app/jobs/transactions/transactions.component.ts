import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { LoggerService } from '../../services/logger.service';
import { Vehicle } from '../../models/vehicle';
import { RepairTransaction } from '../../models/repair-transaction';
import { ParkService } from 'src/app/services/park.service';
import { RepairService } from 'src/app/services/repair.service';
import { AccountService } from 'src/app/services/account.service';
import { RouterService } from 'src/app/services/router.service';

@Component({
  selector: 'app-transactions',
  templateUrl: './transactions.component.html',
  styleUrls: ['./transactions.component.scss']
})
export class TransactionsComponent implements OnInit {

  repairFormGroup!: FormGroup;
  vehicles: Vehicle[] = [];
  transactions: RepairTransaction[] = [];
  showList: boolean = false;
  constructor(private _logger: LoggerService, private _router: RouterService, private _formBuilder: FormBuilder, private _accountService: AccountService, private _parkService: ParkService, private _repairService: RepairService) {}
  
  ngOnInit(): void {
    this.repairFormGroup = this._formBuilder.group({
      vehicleID: ['', [Validators.required]],
    });
    this.getAllVehicles();
  }

  getAllVehicles(): void{
    this._parkService.getAllVehicles()
            .subscribe({
                next: (response: Vehicle[]) => {
                  this.vehicles = response;
                },
                error: (error) => {
                    this._logger.log(`Vehicles [Error]`, error);
                },
                complete: () => {
                    this._logger.log(`Vehicles [Complete]`, "GET request successful !");
                }
            });
  }

  getVehicleRepairs(vehicleID: string): void{
    this._repairService.getRepairTransactions(vehicleID)
            .subscribe({
                next: (response: RepairTransaction[]) => {
                  this.transactions = response;
                  this.showList = true;
                },
                error: (error) => {
                    this._logger.log(`Repair Transactions [Error]`, error);
                },
                complete: () => {
                    this._logger.log(`Repair Transactions [Complete]`, "GET request successful !");
                }
            });
  }

  onSubmitCallback(): void{
    this.getVehicleRepairs(this.repairFormGroup.get('vehicleID')?.value);
  }

  onCompleteCallback(): void{
    this._repairService.completeRepairTransactions(this.repairFormGroup.get('vehicleID')?.value)
      .subscribe({
        next: (value) => {
          if(value.success){
            this._logger.pop(value.message);
          }
        },
        error: (err) => {
          this._logger.log("Repairs", err);
        },
        complete: () => {
          this._router.routeToPath('home');
        }
      });
  }

  checkIfUserisTechnician(): boolean{
    return this._accountService.checkIfUserisTechnician();
  }
}
