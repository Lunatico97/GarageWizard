using System.Drawing;

namespace GarageCoreMVC.Common;

public static class DefaultValues
{
    //Vehicles
    public const string VehicleStringEmptyMessage = "The name or registration id of a vehicle can't be empty !";
    public const string VehicleStringRegexMessage = "The name of vehicle or vendor should contain any special character except '-' and start with an alphabet !";
    public const string VehicleIDEmptyMessage = "The registration ID of a vehicle can't be empty!";
    public const string VehicleWheelErrorMessage = "A vehicle should at least have one wheel!";
    public const string VehicleDuplicateErrorMessage = $"Duplicate vehicles with same registration ID's can't exist !";
    public const string VehicleIDValidatorMessage = "Registration ID should be of type: [XX:YYY] where XX is national code and YYY is unique number";
    //Spots
    public const string SpotIDEmptyMessage = "Spot identifier can't be empty!";
    public const string SpotCapacityErrorMessage = "Spot identifier can't be empty!";
    public const string SpotIDNotProvidedMessage = "The spot or vehicle for reservation is not provided !!";
    public const string SpotIDValidatorMessage = "Spot identifier should be of type: [X-Y-ZZ] where X: Floor, Y: Number, Z: Unique spot locator";
    public const string SpotOccupancyErrorMessage = "This spot is either occupied or, your vehicle can't fit in the spot ! Try again !";
    public const string ParkingSpaceFullMessage = "Parking space is completely full !!";
    //Roles
    public const string RoleExistsMessage = "This role already exists !";
    public const string RoleCreationFailedMessage = "Failed to create a new role !";
    public const string RoleCreationSuccessMessage = "Successfully created a new role !";
    //Parking
    public const string ParkingTransactionInvalidMessage = "The following parking transaction is invalid as it doesn't exist !";
    //Jobs
    public const string JobStringEmptyMessage = "The name of a repair job can't be empty !";
    public const string JobStringRegexMessage = "The name of a repair job should contain any special character except '-' and start with an alphabet !";
    public const string JobChargeableMessage = "A job should be chargeable at an amount !";
    public const string JobsAssignedSuccessMessage = "Assigned and saved job transactions !";
    public const string NoPendingJobsMessage = "No pending jobs for this vehicle !!";
    public const string JobsCompletedSuccessMessage = "The repairs are serviced successfully !";
    public const string JobsDeletedSuccessMessage = "The job is deleted successfully !";
    public const string JobAssociatedToIDNotFoundMessage = "Job data can't be found on the server !";
    public const string JobsRestrictDeletionMessage = "Repairs associated to this job are pending ! Job can't be deleted !";
    public const string JobListUnavailableToAssignMessage = "Job list is empty or unavailable from the client to assign !";
    //Images
    public const string UploadJobImagePath = "uploads/jobs";
    public const string ImageUploadedSuccessMessage = "Image uploaded successfully !";
    public const string ImageNotFoundMessage = "Image file not found on the server !";
    public const string FileUnavailableMessage = "File is not received by the server !";
    // Users and Authentication
    public const string LoggedOutMessage = "Logged out of application !";
    public const string LoginSuccessMessage = "Logged in to the application successfully !";
    public const string LoginFailedMessage = "Login failed - Invalid username or password !!";
    public const string DuplicateUserFailMessage = "Failed to create user - duplication of credentials !!";
    public const string AuthMWTokenExpiryMessage = "Your session is expired - please log in again !";
    public const string AuthMWBadTokenMessage = "Your session token got compromised - please try again !";
    public const string NoUserAuthenticatedYetMessage = "No user is authenticated yet !";
    public const string PasswordChangeRestrictedMessage = "No user is authenticated yet so, no change request can be accepted";
    public const string UserPasswordWrongMessage = "User's old password is wrong ! Please try again !";
    public const string AccessDenialMessage = "You are just a visitor - know your limits !";
    // Unavailable data messages
    public const string ValidationFailedMessage = "Backend validation failed !!!";
    public const string VehicleDataUnavailableMessage = "Vehicle data is not received by the server !";
    public const string PasswordChangeDtoUnavailableMessage = "User password change object is not available to the server !";
    public const string UserRegisterDtoUnavailableMessage = "User register data is not available to the server !";
    public const string UserLoginDtoUnavailableMessage = "User login data is not available to the server !";
    public const string RoleAccessListUnavailableMessage = "Access list cannot be found by the server ! ";
    public const string UserInAccessUnavailableMessage = "Users in access list are not available on the server !";
    public const string RoleDetailsUnavailableMessage = "The role data is not received by the server !";
    public const string JobDataUnavailableMessage = "Job data is not received by the server !!";
    public const string VehicleIDInRepairDtoUnavailableMessage = "The server couldn't get vehicle's identification !";
    public const string WelcomeMessage = "Welcome To GarageCRUD API !!!";
}