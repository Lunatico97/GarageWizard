using System.Text.RegularExpressions;
using GarageCoreMVC.Common;
using GarageCoreMVC.Models;

namespace GarageCoreMVC.Validators
{
    public static class SpotValidator
    {
        public static bool Validate(Spot spot)
        {
            Regex idRegex = new Regex(Constants.SpotIDRegex);

            if (string.IsNullOrEmpty(spot.ID))
            {
                throw new ArgumentException(DefaultValues.SpotIDEmptyMessage);
            }
            else if (spot.Capacity == 0 || int.IsNegative(spot.Capacity))
            {
                throw new ArgumentException(DefaultValues.SpotCapacityErrorMessage);
            }
            else if (!idRegex.IsMatch(spot.ID))
            {
                throw new ArgumentException(DefaultValues.SpotIDValidatorMessage);
            }
            else
            {
                return true;
            }
        }
    }
}
