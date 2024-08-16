import { Component, OnInit } from '@angular/core';
import { HTTPService } from '../services/http.service';
import { Configuration } from '../config/globalConfig';
import { StatsDto } from '../models/statsDto';
import { AccountService } from '../services/account.service';
import { User } from '../models/user';
import { catchError } from 'rxjs';
import { Chart } from 'chart.js/auto';
import { RouterService } from '../services/router.service';
import { MatDialog } from '@angular/material/dialog';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { SeriesDto } from '../models/seriesDto';

@Component({
  selector: 'dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {

  constructor(private _http: HTTPService, private _router: RouterService, private _account: AccountService, public _dialog: MatDialog) { }
  currentUser!: User;
  stats!: StatsDto;
  series!: SeriesDto[];
  message!: string;
  chart!: any;
  loading: boolean = false;
  chartLoading: boolean = false;

  ngOnInit(): void {
    this.getDashboardStats();
    this.getLoggedUser();
    this.getSeriesData();
  }

  getDashboardStats(): void{
    this.loading = true;
    this._http.sendSingleGetRequest<StatsDto>({}, Configuration.garageApiStatsEndpoint)
      .subscribe({
        next: (response: StatsDto) => {
          this.stats = response;
          this.createPieChart();
        },
        complete: () => {
          this.loading = false;
        }
      });
  }

  getSeriesData(): void{
    this.chartLoading = true;
    this._http.sendGetRequest<SeriesDto>("revenue/series")
        .subscribe({
          next: (response: SeriesDto[]) => {
            this.series = response;
            this.createChart();
          },
          complete: () => {
            this.chartLoading = false;
          }
        });
  }

  getLoggedUser(): void{
    this._account.getCurrentUser()
            .pipe(
              catchError((err, caught) => {
                console.log(err.error.message);
                this.message = err.error.message;
                return [];
              })
            )
            .subscribe({
                next: (response: User) => {
                  this.currentUser = response;
                  if(this.currentUser.role === 'Sudo')
                      this._account.setSudoFlag(true);
                },
                complete: () => {
                  
                }
              });
  }

  onChangePasswordCallback(): void{
    const dialogRef = this._dialog.open(ChangePasswordComponent, {width: '300px'});
    dialogRef.afterClosed()
      .subscribe(
        (result) => {
          console.log(`Dialog result: ${result}`);
        }
    );
  }

  checkIfLoggedIn(): boolean{
    return this._account.checkIfUserIsLoggedIn();
  }

  onClick(): void{
    this._router.routeToPath('account');
  }

  createChart(){
    let labels = this.series.map(data => data.day);
    let tollData = this.series.map(data => data.amountToll.toString());
    let repairData = this.series.map(data => data.amountRepairs.toString());
    this.chart = new Chart("MyChart", {
      type: 'bar',
      data: {
          labels: labels, 
	       datasets: [
          {
            label: "Revenue From Toll",
            data: tollData,
            backgroundColor: 'purple'
          },
          {
            label: "Revenue From Repairs",
            data: repairData,
            backgroundColor: 'goldenrod'
          }
        ]
      },
      options: {
        aspectRatio:2.5,
        plugins: {
          title: {
              color: 'green',
              display: true,
              text: 'Revenue Analytics'
          }
      }
      }  
    });
  }

  createPieChart(){
    this.chart = new Chart("MyPieChart", {
      type: 'doughnut',
      data: {
          labels: ['Two Wheelers', 'Four Wheelers', 'Special Wheelers'], 
	        datasets: [  
          // {
          //   data: [this.stats.revenueFromToll, this.stats.revenueFromRepairs]
          // },
          {
            data: [this.stats.twoWheelers, this.stats.fourWheelers, this.stats.specialWheelers]
          },
        ]
      },
      options: {
        aspectRatio:2.5,
        plugins: {
          title: {
              color: 'green',
              display: true,
              text: 'Vehicle Distribution'
          }
        }
      }  
    });
  }

}

