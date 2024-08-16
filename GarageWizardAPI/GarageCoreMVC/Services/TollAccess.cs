using System.Collections;
using GarageCoreMVC.Models;
using GarageCoreMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using GarageCoreMVC.Services.Interfaces;
using GarageCoreMVC.Validators;
using GarageCoreMVC.Common.Enums;
using GarageCoreMVC.Common;
using System.Diagnostics.CodeAnalysis;

namespace GarageCoreMVC.Services
{
    public class TollAccess : IToll
    {
        private readonly GarageDBContext _context;
        private readonly IMemoryCache _mCache;

        public TollAccess(GarageDBContext context, IMemoryCache cache)
        {
            _context = context;
            _mCache = cache;
        }

        public async Task<List<Spot>> GetSpots()
        {
            if (!_mCache.TryGetValue(Constants.SpotCacheKey, out List<Spot>? spots))
            {
                spots = await _context.Spots.OrderBy(spot => spot.ID).ToListAsync();
                _mCache.Set(Constants.SpotCacheKey, spots, TimeSpan.FromMinutes(20));
            }
            return spots ?? new List<Spot>();
        }

        public async Task<List<Spot>> GetVacantSpots()
        {
            var spots = await _context.Spots.ToListAsync();
            if(spots == null) return new List<Spot>();
            else
            {
                var vacancies = from spot in spots
                                where !spot.Occupied
                                orderby spot.ID ascending
                                select spot;
                return vacancies.ToList<Spot>();
            }
        }

        public async Task<List<Spot>> GetVacantUsableSpots(int capacity)
        {
            var spots = await _context.Spots.ToListAsync();
            if (spots == null) return new List<Spot>();
            else
            {
                var vacancies = from spot in spots
                                where !spot.Occupied && spot.Capacity >= capacity
                                orderby spot.ID ascending
                                select spot;
                return vacancies.ToList<Spot>();
            }
                
        }

        public static float CalculateCharges(TimeSpan span, ServiceT service)
        {
            float charge = (int)service * ((span.Days * 24) + span.Hours + (float)span.Minutes / 60) * Constants.TollRate;
            return charge;
        }

        public async Task<bool> AreThereVacantSpots()
        {
            var spots = await _context.Spots.ToListAsync();
            var query = from spot in spots
                        where !spot.Occupied
                        select spot;
            if (query.Count() > 0) return true;
            else return false;
        }

        public async Task<bool> IsThisSpotVacant(string sID)
        {
            var spot = await GetSpotByID(sID);
            if (spot != null) return !spot.Occupied;
            else return false;
        }

        public async Task<Spot?> GetSpotByID(string sID)
        {
            var spot = await _context.Spots.FirstOrDefaultAsync(s => s.ID == sID);
            return spot;
        }

        public async Task<Spot?> GetSpotByVID(string reqID)
        {
            var parking = await _context.Parkings.FirstOrDefaultAsync(p => p.VehicleID == reqID);
            if(parking == null) return null;
            var spot = await _context.Spots.FirstOrDefaultAsync(s => s.ID == parking.SpotID);
            return spot;
        }

        public async Task UpdateSpotOccupancy(string sID, bool toValue)
        {
            var spot = await GetSpotByID(sID);
            if(spot != null)
            {
                spot.Occupied = toValue;
                _context.Spots.Update(spot);
                _mCache.Remove(Constants.SpotCacheKey);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddSpot(string sID, int capacity)
        {
            Spot spot = new Spot(sID, capacity);
            _context.Spots.Add(spot);
            await _context.SaveChangesAsync();
            _mCache.Remove(Constants.SpotCacheKey);
            Console.WriteLine($"Parking spot with ID: <{sID}> is successfully added to the toll !");
        }

        public async Task RemoveSpot(string sID)
        {
            Spot? spot = await GetSpotByID(sID);
            if(spot != null)
            {
                _context.Spots.Remove(spot);
                await _context.SaveChangesAsync();
                _mCache.Remove(Constants.SpotCacheKey);
                Console.WriteLine($"Parking spot with ID: <{sID}> is successfully removed from the toll !");
            }
        }

        public async Task<bool> CheckIfSpotExists(string sID)
        {
            var spot = await GetSpotByID(sID);
            return (spot != null) ;
        }

        public bool CheckValidityDirect(Spot spot)
        {
            try
            {
                if (spot != null && SpotValidator.Validate(spot)) { }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Validation check failed => {0}", ex.Message);
                return false;
            }
            return true;
        }

        // Default enumerator is essential to compile the code !
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [ExcludeFromCodeCoverage]
        public IEnumerator<string> GetEnumerator()
        {
            foreach (Spot s in _context.Spots.ToList())
            {
                yield return s.ToString();
            }
        }

        [ExcludeFromCodeCoverage]
        IEnumerator<Spot> IEnumerable<Spot>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

