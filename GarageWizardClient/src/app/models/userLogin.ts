
export class UserLogin
{
    Email! : string;
    Password! : string;
    RememberMe : boolean = false;

    constructor(email: string, password: string, rememberMe: boolean)
    {
        this.Email = email;
        this.Password = password;
        this.RememberMe = rememberMe;
    }
}