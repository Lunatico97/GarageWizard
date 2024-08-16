using GarageCoreMVC.Models;

namespace GarageCoreMVC.Services.Interfaces
{
    // Vehicle Repository Interface
    public interface IGarage : IEnumerable<Vehicle>
    {
        Task<List<Vehicle>> GetVehicles();
        Task<List<Vehicle>> GetVehiclesNotOnRepair();
        Task<Vehicle?> GetVehicleByID(string reqID);
        Task ParkVehicle(Vehicle vehicle);
        Task UpdateVehicle(Vehicle vehicle);
        Task RemoveVehicle(string reqID);
        Task<bool> DoesVehicleExist(string reqID);
        Task SetVehicleOnRepairFlag(string reqID, bool value);
        bool CheckValidityDirect(Vehicle v);
    }
}
