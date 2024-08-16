namespace GarageCoreMVC.Common.Enums
{
    /// <summary>
    /// Includes service type for parking toll - Vacant, Economy (1*<see cref="GarageCoreMVC.Common.Constants.TollRate"/>)
    /// and VIP (2*<see cref="GarageCoreMVC.Common.Constants.TollRate"/>)
    /// </summary>
    public enum ServiceT : int
    {
        Vacant = 0,
        Economy = 1,
        VIP = 2
    };

    /// <summary>
    /// Similar to <see cref="ServiceT"/> but this excludes 'Vacant' from choosable service
    /// </summary>
    public enum ServiceTC : int
    {
        Economy = 1,
        VIP = 2,
    }
}
