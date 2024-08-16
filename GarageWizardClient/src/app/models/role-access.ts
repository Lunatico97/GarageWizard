export class RoleAccess
{
    UserID!: string;
    Role!: string;
    IsAuthorized!: boolean;

    public constructor(uid: string, role: string, isAuth: boolean = false){
        this.UserID = uid;
        this.Role = role;
        this.IsAuthorized = isAuth;
    }
}