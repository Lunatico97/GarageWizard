using GarageCoreMVC.Common;
using GarageCoreMVC.Models;
using System.Text.RegularExpressions;

namespace GarageCoreMVC.Validators
{
    public static class JobValidator
    {
        public static bool Validate(Job job)
        {
            if (string.IsNullOrEmpty(job.Name))
            {
                throw new ArgumentException(DefaultValues.VehicleStringEmptyMessage);
            }
            else if (job.Charge == 0 || int.IsNegative(job.Charge))
            {
                throw new ArgumentException(DefaultValues.JobChargeableMessage);
            }
            else
            {
                return true;
            }
        }
    }
}
