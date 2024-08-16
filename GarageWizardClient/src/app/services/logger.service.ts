import { Injectable } from "@angular/core";
import { MatSnackBar } from "@angular/material/snack-bar";

@Injectable()
export class LoggerService 
{
    constructor(private _snackbar: MatSnackBar){}
    public log(from : string, message : any) : void
    {
        console.log(`LOGGER: ${from} => ` + message);
    }

    public pop(message: string, duration_ms: number = 2500): void{
        this._snackbar.open(message, "OK", {duration: duration_ms});
    }
}