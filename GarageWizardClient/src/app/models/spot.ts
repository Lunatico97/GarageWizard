export class Spot
{
    id!: string;
    capacity!: number;
    occupied: boolean = false;

    constructor(sID: string, cap: number){
        this.id = sID;
        this.capacity = cap;
    }
}