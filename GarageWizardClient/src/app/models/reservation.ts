export class Reservation
{
    spotID!: string;
    vehicleID!: string;
    service: number = 0;

    constructor(sID: string, vID: string, service: number){
        this.spotID = sID;
        this.vehicleID = vID;
        this.service = service;
    }
}