namespace GarageCoreMVC.Common
{
    public static class Urls
    {
        // Accounts
        public const string RegisterAccount = "/account/register";
        public const string LoginToAccount = "/account/login";
        public const string LogoutOfAccount = "/account/logout";
        public const string DenyAccess = "/account/access-denied";
        public const string GetAllUsers = "/users/info";
        public const string GetCurrentUser = "/users/current";
        public const string ChangeUserPassword = "/users/current/change";
        // Admin Privileges
        public const string CreateRole = "/role/create";
        public const string ListRoles = "/role/info";
        public const string UpdateRole = "role/edit/{roleID}";
        public const string GetRoleDetails = "role/details";
        public const string EditRoleAccess = "role/access/edit";
        // Home
        public const string Home = "/";
        public const string Test = "/test";
        // Jobs
        public const string ListJobs = "/job/info";
        public const string CreateJob = "/job/create";
        public const string RemoveJob = "/job/remove"; 
        public const string AssignJob = "/job/assign";
        public const string GetAssignedJobs = "/job/specifics";
        public const string CompleteAssignedJobs = "/job/specifics/complete";
        // Spots
        public const string ListSpots = "/spot/info";
        public const string CreateSpot = "/spot/create";
        public const string RemoveSpot = "/spot/remove";
        public const string ReleaseSpot = "/spot/release";
        public const string ReserveSpotCheck = "/spot/reserve/check/{vecID}-{wheels}"; // where vecID is vehicle ID
        public const string ReserveSpotFailed = "/spot/reserve/fail";
        public const string ReserveSpotInstant = "/spot/reserve/";
        // Vehicles
        public const string ParkVehicle = "/vehicle/park";
        public const string ListVehicles = "/vehicle/info";
        public const string RemoveVehicle = "/vehicle/remove";
        public const string UpdateVehicle = "/vehicle/edit";
        public const string ListVehiclesNotOnRepair = "/vehicle/norepair/info";
        public const string GetSpecificVehicle = "/vehicle/specifics";
        // Statistics
        public const string GetStatistics = "/stats";
        // Image
        public const string GetUploadedJobImage = "job/uploads/get";
        public const string UploadJobImage = "job/uploads/place";
        // Client validation
        public const string ValidateClientApp = "/validate/client-app/{token}";

    }
}
