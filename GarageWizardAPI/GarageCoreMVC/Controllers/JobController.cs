using GarageCoreAPI.Models;
using GarageCoreMVC.Common;
using GarageCoreMVC.Common.Utilities;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarageCoreMVC.Controllers
{
    public class JobController : Controller
    {
        private readonly IRepair repair;
        private readonly IGarage garage;
        public JobController(IRepair repair, IGarage garage)
        {
            this.repair = repair;
            this.garage = garage;
        }

        [HttpGet]
        [Authorize]
        [Route(Urls.ListJobs)]
        public async Task<ActionResult> Info()
        {
            return Json(await repair.ListJobs());
        }

        [HttpPost]
        [Authorize(Roles = "Sudo")]
        [Route(Urls.CreateJob)]
        public async Task<ActionResult> Create([FromBody] Job job)
        {
            if (job == null) { return BadRequest(DefaultValues.JobDataUnavailableMessage); }
            if (repair.CheckValidityDirect(job))
            {
                if (await repair.DoesJobExist(job.Name))
                {
                    return Json(new ResponseMessage { Success = false, Message = $"A spot with <{job.Name}> already exist !" });
                }
                await repair.CreateJob(job.Name, job.Description, job.Hours, job.Charge, job.JobImagePath);
                return Json(new ResponseMessage{ Success = true, Message = $"New job - {job.Name} is created !" });
            }
            return Json(new ResponseMessage{ Success = false, Message = DefaultValues.ValidationFailedMessage });
        }

        [HttpPost]
        [Authorize]
        [Route(Urls.AssignJob)]
        public async Task<ActionResult> AssignJob([FromBody] RepairDto repairDto)
        {
            if(string.IsNullOrEmpty(repairDto.vehicleID))
            {
                return BadRequest(DefaultValues.VehicleIDInRepairDtoUnavailableMessage);
            }
            else if(repairDto.jobs.Count() == 0)
            {
                return BadRequest(DefaultValues.JobListUnavailableToAssignMessage);
            }
            else
            {
                await repair.CreateRepairTransactions(repairDto.vehicleID, repairDto.jobs);
                await garage.SetVehicleOnRepairFlag(repairDto.vehicleID, true);
                return Json(new ResponseMessage{ Success = true, Message = DefaultValues.JobsAssignedSuccessMessage });
            }
        }

        [HttpGet]
        [Authorize]
        [Route(Urls.GetAssignedJobs)]
        public async Task<ActionResult> GetAssignedJobs([FromQuery] string vehicleID)
        {
            if (string.IsNullOrEmpty(vehicleID))
            {
                return BadRequest(DefaultValues.VehicleIDInRepairDtoUnavailableMessage);
            }
            else
            {
                List<RepairTransactionDto> rtDtos = new List<RepairTransactionDto>();
                foreach(var transaction in await repair.GetRepairTransactionsByVID(vehicleID))
                {
                    if (transaction.JobID == null || transaction.IsCompleted) continue;
                    var job = await repair.GetJobByID(transaction.JobID);
                    rtDtos.Add(
                        new RepairTransactionDto
                        {
                            Charge = job.Charge,
                            Hours = job.Hours,
                            IsCompleted = transaction.IsCompleted,
                            TransactionDate = transaction.TransactionStamp.ToString(),
                            JobName = job.Name,
                        }
                    );
                }
                return Json(rtDtos);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Sudo")]
        [Route(Urls.CompleteAssignedJobs)]
        public async Task<ActionResult> CompleteAssignedJobs([FromQuery] string vehicleID)
        {
            if (string.IsNullOrEmpty(vehicleID))
            {
                return BadRequest(DefaultValues.VehicleIDInRepairDtoUnavailableMessage);
            }
            else
            {
                var transactions = await repair.GetPendingTransactionsByJID(vehicleID);
                if (transactions.Count() == 0) 
                    return Json(new ResponseMessage{ Success = true, Message =DefaultValues.NoPendingJobsMessage });
                else
                {
                    await repair.CompleteRepairTransactions(vehicleID);
                    await garage.SetVehicleOnRepairFlag(vehicleID, false);
                    return Json(new ResponseMessage{ Success = true, Message = DefaultValues.JobsCompletedSuccessMessage });
                }    
            }
        }

        [HttpGet]
        [Route(Urls.GetUploadedJobImage)]
        public async Task<ActionResult> GetUploadedJobImage([FromQuery] string jobID)
        {
            byte[] imageBytes = [];
            var job = await repair.GetJobByID(jobID);
            if (job == null) 
            {
                return NotFound(DefaultValues.JobAssociatedToIDNotFoundMessage);
            }
            else
            {
                var path = Path.Combine("wwwroot/" + DefaultValues.UploadJobImagePath + "/" + job.JobImagePath);
                if (System.IO.File.Exists(path))
                {
                    imageBytes = System.IO.File.ReadAllBytes(path);
                    return File(imageBytes, "image/jpeg");
                }
                else return NotFound(DefaultValues.ImageNotFoundMessage);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Sudo")]
        [Route(Urls.UploadJobImage)]
        public async Task<ActionResult> UploadJobImage([FromForm] IFormFile file)
        {
            if (file == null) return BadRequest(DefaultValues.FileUnavailableMessage) ;
            else
            {
                await Uploader.UploadAsync(file, "wwwroot/" + DefaultValues.UploadJobImagePath);
                return Json(new ResponseMessage{ Success = true, Message = DefaultValues.ImageUploadedSuccessMessage });
            }
        }

        [HttpDelete]
        [Route(Urls.RemoveJob)]
        [Authorize(Roles = "Sudo")]
        public async Task<ActionResult> RemoveJob([FromQuery] string jobID)
        {
            if (jobID == null) return BadRequest($"Job with ID: {jobID} is not found on the server !");
            var pendingJobs = await repair.GetPendingTransactionsByJID(jobID);
            if (pendingJobs.Count() != 0) 
            {
                return Json(new ResponseMessage{ Success = false, Message = DefaultValues.JobsRestrictDeletionMessage });
            } 
            else
            {
                await repair.DeleteJob(jobID);
                return Json(new ResponseMessage{ Success = true, Message = DefaultValues.JobsDeletedSuccessMessage });
            }
        }
    }
}
