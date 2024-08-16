using GarageCoreMVC.Common;
using Humanizer.DateTimeHumanizeStrategy;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GarageCoreMVC.Models
{
    [ExcludeFromCodeCoverage]
    public class JobView
    {
        [Required(ErrorMessage = DefaultValues.JobStringEmptyMessage)]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9- ]*$", ErrorMessage = DefaultValues.JobStringRegexMessage)]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Charge { get; set; }
        public IFormFile JobImage { get; set; } = null;

        public Job ConvertToJob()
        {
            Job job = new Job
            {
                Name = this.Name,
                Description = this.Description,
                Charge = this.Charge,
                JobImagePath = this.JobImage.FileName 
            };
            return job;
        }
    }
}
