using GarageCoreMVC.Models;

namespace GarageCoreMVC.Services.Interfaces
{
    public interface IRepair
    {
        Task CreateJob(string name, string description, double hours, int charge, string path);
        Task<List<Job>> ListJobs();
        Task<Job?> GetJobByID(string jobID);
        Task<Job?> GetJobByName(string jobName);
        Task DeleteJob(string jobID);
        Task UpdateJob(string jobID, string name, string description, int charge, string path);
        bool CheckValidityDirect(Job job);
        Task<bool> DoesJobExist(string jobName);
        Task CreateRepairTransactions(string vehicleID, List<Job> jobs);
        Task<List<RepairTransaction>> GetRepairTransactionsByVID(string vehicleID);
        Task<List<RepairTransaction>> GetAllRepairTransactions();
        Task<List<RepairTransaction>> GetPendingTransactionsByJID(string jobID);
        Task ArchiveRepairTransactions(string vehicleID);
        Task CompleteRepairTransactions(string vehicleID);
        Task<double> CalculateRevenue();
    }
}
