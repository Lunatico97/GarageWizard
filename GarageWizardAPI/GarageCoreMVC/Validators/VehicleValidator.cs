using System.Text.RegularExpressions;
using GarageCoreMVC.Common;
using GarageCoreMVC.Models;

namespace GarageCoreMVC.Validators
{
    public static class VehicleValidator
    {
        public static bool Validate(Vehicle vehicle)
        {
            Regex idRegex = new Regex(Constants.VehicleIDRegex);
            Regex textRegex = new Regex(Constants.VehicleStringRegex);

            if (string.IsNullOrEmpty(vehicle.Name) || string.IsNullOrEmpty(vehicle.Vendor) || string.IsNullOrEmpty(vehicle.RegID))
            {
                throw new ArgumentException(DefaultValues.VehicleStringEmptyMessage);
            }
            else if (!textRegex.IsMatch(vehicle.Name) || !textRegex.IsMatch(vehicle.Vendor))
            {
                throw new ArgumentException(DefaultValues.VehicleStringRegexMessage);
            }
            else if (vehicle.Wheels == 0 || int.IsNegative(vehicle.Wheels))
            {
                throw new ArgumentException(DefaultValues.VehicleWheelErrorMessage);
            }
            else if (!idRegex.IsMatch(vehicle.RegID))
            {
                throw new ArgumentException(DefaultValues.VehicleIDValidatorMessage);
            }
            else
            {
                return true;
            }
        }
    }
}
