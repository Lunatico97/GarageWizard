import { Job } from "./job";

export class Repair{
    vehicleID!: string;
    jobs!: Job[];

    constructor(id: string, jobs: Job[]){
        this.vehicleID = id;
        this.jobs = jobs;
    }
}