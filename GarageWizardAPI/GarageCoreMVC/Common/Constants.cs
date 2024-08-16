namespace GarageCoreMVC.Common;

public static class Constants
{
    public const int TollRate = 25;
    // Cache Keys
    public const string VehicleCacheKey = "Vehicles";
    public const string SpotCacheKey = "Spots";
    public const string JobCacheKey = "Jobs";
    // MVC
    public const string IndexView = "Index";
    public const string HomeController = "Home";
    // Regex
    public const string VehicleStringRegex = @"^[A-Z][a-zA-Z0-9- ]*$";
    public const string VehicleIDRegex = @"\b[A-Z]{2}\d{3}\b";
    public const string SpotIDRegex = @"\b[A-Z]{1}[A-Z0-9]{3}\b";
}
