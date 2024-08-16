using GarageCoreMVC.Common;
using GarageCoreMVC.Data;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services.Interfaces;
using GarageCoreMVC.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GarageCoreMVC.Services
{
    public class RepairAccess : IRepair
    {
        private readonly GarageDBContext _context;
        private readonly IMemoryCache _mCache;
        public RepairAccess(GarageDBContext context, IMemoryCache cache) 
        {
            _context = context;
            _mCache = cache;
        }

        public bool CheckValidityDirect(Job job)
        {
            try
            {
                if (job != null && JobValidator.Validate(job)) { }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Validation check failed => {0}", ex.Message);
                return false;
            }
            return true;
        }

        public async Task CreateJob(string name, string description, double hours, int charge, string path)
        {
            Job newJob = new Job
            {
                Name = name,
                Description = description,
                Charge = charge,
                Hours = hours,
                JobImagePath = path
            };
            _context.Jobs.Add(newJob);
            _mCache.Remove(Constants.JobCacheKey);
            await _context.SaveChangesAsync();
        }

        public async Task CreateRepairTransactions(string vehicleID, List<Job> jobs)
        {
            RepairTransaction? newRepair = null;
            if(jobs.Count > 0)
            {
                foreach(var job in jobs)
                {
                    newRepair = new RepairTransaction
                    {
                        ID = Guid.NewGuid().ToString(),
                        VehicleID = vehicleID,
                        TransactionStamp = DateTime.Now,
                        Charge = job.Charge,
                        JobID = job.ID
                    };
                    _context.Repairs.Add(newRepair);
                    await _context.SaveChangesAsync();
                }                
            }
        }

        public async Task<List<RepairTransaction>> GetAllRepairTransactions()
        {
            return await _context.Repairs.ToListAsync();
        }

        public async Task<List<RepairTransaction>> GetRepairTransactionsByVID(string vehicleID)
        {
            return await _context.Repairs.Where(repair => repair.VehicleID == vehicleID).ToListAsync();
        }

        public async Task<List<RepairTransaction>> GetPendingTransactionsByJID(string jobID)
        {
            return await _context.Repairs.Where(repair => !repair.IsCompleted && repair.JobID == jobID).ToListAsync();
        }

        public async Task ArchiveRepairTransactions(string vehicleID)
        {
            var transactions = await this.GetRepairTransactionsByVID(vehicleID);
            foreach(var transaction in transactions)
            {
                transaction.VehicleID = null;
                _context.Update(transaction);
                await _context.SaveChangesAsync();
            }
        }

        public async Task CompleteRepairTransactions(string vehicleID)
        {
            var transactions = await this.GetRepairTransactionsByVID(vehicleID);
            foreach (var transaction in transactions)
            {
                transaction.IsCompleted = true;
                _context.Update(transaction);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteJob(string jobID)
        {
            var job = await GetJobByID(jobID);
            if(job != null)
            {
                _context.Jobs.Remove(job);
                _mCache.Remove(Constants.JobCacheKey);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DoesJobExist(string jobName)
        {
            var job = await GetJobByName(jobName);
            return (job != null);
        }

        public async Task<Job?> GetJobByID(string jobID)
        {
            return await _context.Jobs.FirstOrDefaultAsync(j => j.ID.Equals(jobID));
        }

        public async Task<Job?> GetJobByName(string jobName)
        {
            return await _context.Jobs.FirstOrDefaultAsync(j => j.Name.Equals(jobName));
        }

        public async Task<List<Job>> ListJobs()
        {
            if (!_mCache.TryGetValue(Constants.JobCacheKey, out List<Job>? jobs))
            {
                jobs = await _context.Jobs.OrderBy(j => j.Name).ToListAsync();
                _mCache.Set(Constants.JobCacheKey, jobs, TimeSpan.FromMinutes(20));
            }
            return jobs ?? new List<Job>();
        }

        public async Task UpdateJob(string jobID, string name, string description, int charge, string path)
        {
            Job? tempJob = await GetJobByID(jobID);
            if (tempJob != null)
            {
                tempJob.Name = name;
                tempJob.Description = description;  
                tempJob.Charge = charge;
                tempJob.JobImagePath = path;
                _context.Jobs.Update(tempJob);
                _mCache.Remove(Constants.JobCacheKey);
                await _context.SaveChangesAsync();
            } 
        }

        public async Task<double> CalculateRevenue()
        {
            double revenue = 0;
            foreach(var transaction in await _context.Repairs.ToListAsync())
            {
                var job = await GetJobByID(transaction.JobID);
                if (job == null) continue;
                revenue += job.Charge;
            }
            return revenue;
        }
    }
}
