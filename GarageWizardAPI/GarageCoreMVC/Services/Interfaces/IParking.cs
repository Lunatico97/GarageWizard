using GarageCoreMVC.Common.Enums;
using GarageCoreMVC.Models;

namespace GarageCoreMVC.Services.Interfaces
{
    public interface IParking
    {
        Task<ParkingTransaction?> GetParkingByID(string spotID, string vehicleID);
        Task<List<ParkingTransaction>> GetParkingTransactions();
        Task AddParking(string spotID, string vehicleID, ServiceT service);
        Task RemoveParking(string spotID, string vehicleID);
        Task CheckoutParking(string spotID, string vehicleID);
        TimeSpan CalculateParkedTime(ParkingTransaction parking);
        Task<double> CalculateRevenue();
    }
}
