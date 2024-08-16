export class Job
{
    id!: string;
    name!: string;
    description!: string;
    charge!: number;
    hours!: number;
    jobImagePath!: string;

    public constructor(jname: string, des: string, jcharge: number, jhours: number, jpath: string){
        this.name = jname;
        this.description = des;
        this.charge = jcharge;
        this.hours = jhours;
        this.jobImagePath = jpath;
    }
}