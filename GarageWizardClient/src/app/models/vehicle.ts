export class Vehicle
{
    name!: string;
    vendor!: string;
    wheels!: number;
    regID!: string;
    isOnRepair!: boolean;

    public constructor(vname: string, vvendor: string, vwheels: number = 4, regID: string)
    {
        this.name = vname;
        this.vendor = vvendor;
        this.wheels = vwheels;
        this.regID = regID;
        this.isOnRepair = false;
    }
}