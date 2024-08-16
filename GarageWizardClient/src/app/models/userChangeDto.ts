export class UserChangeDto{
    OldPassword!: string;
    NewPassword!: string;

    public constructor(oldPwd: string, newPwd: string){
        this.OldPassword = oldPwd;
        this.NewPassword = newPwd;
    }
}