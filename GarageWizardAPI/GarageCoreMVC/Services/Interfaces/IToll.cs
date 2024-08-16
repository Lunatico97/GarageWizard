using GarageCoreMVC.Common.Enums;
using GarageCoreMVC.Models;

namespace GarageCoreMVC.Services.Interfaces
{
    public interface IToll : IEnumerable<Spot>
    {
        // Toll Repository Interface
        Task<List<Spot>> GetSpots();
        Task<List<Spot>> GetVacantSpots();
        Task<List<Spot>> GetVacantUsableSpots(int capacity);
        Task<Spot?> GetSpotByVID(string reqID);
        Task<Spot?> GetSpotByID(string sID);
        Task AddSpot(string sID, int capacity);
        Task RemoveSpot(string sID);
        Task UpdateSpotOccupancy(string sID, bool toValue);
        Task<bool> CheckIfSpotExists(string sID);
        Task<bool> IsThisSpotVacant(string sID);
        Task<bool> AreThereVacantSpots();
        bool CheckValidityDirect(Spot spot);
    }
}
