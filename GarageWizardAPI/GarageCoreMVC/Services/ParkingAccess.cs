using GarageCoreMVC.Common;
using GarageCoreMVC.Common.Enums;
using GarageCoreMVC.Data;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services;
using GarageCoreMVC.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GarageCoreMVC.Services
{
    public class ParkingAccess : IParking
    {
        private readonly GarageDBContext _context;

        public ParkingAccess(GarageDBContext context)
        {
            _context = context;
        }

        public async Task<List<ParkingTransaction>> GetParkingTransactions()
        {
            return await _context.Parkings.ToListAsync();
        }

        public async Task AddParking(string spotID, string vehicleID, ServiceT service)
        {
            ParkingTransaction parking = new ParkingTransaction
            {
                ID = Guid.NewGuid().ToString(),
                SpotID = spotID,
                VehicleID = vehicleID,
                ParkStart = DateTime.Now,
                ParkEnd = DateTime.MaxValue,
                Service = service
            };
            _context.Parkings.Add(parking);
            await _context.SaveChangesAsync();
        }

        public async Task CheckoutParking(string spotID, string vehicleID)
        {
            ParkingTransaction? tempParking = await GetParkingByID(spotID, vehicleID);
            if (tempParking != null)
            {
                tempParking.ParkEnd = DateTime.Now;
                _context.Parkings.Update(tempParking);
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine(DefaultValues.ParkingTransactionInvalidMessage);
            }
        }

        public async Task<ParkingTransaction?> GetParkingByID(string spotID, string vehicleID)
        {
            return await _context.Parkings.FirstOrDefaultAsync(p => p.SpotID == spotID && p.VehicleID == vehicleID);
        }

        public async Task RemoveParking(string spotID, string vehicleID)
        {
            ParkingTransaction? tempParking = await GetParkingByID(spotID, vehicleID);
            if (tempParking != null)
            {
                _context.Parkings.Remove(tempParking);
                await _context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine(DefaultValues.ParkingTransactionInvalidMessage);
            }
        }
        public TimeSpan CalculateParkedTime(ParkingTransaction parking)
        {
            if (parking.ParkStart != DateTime.MinValue)
            {
                TimeSpan difference;
                if (parking.ParkEnd == DateTime.MaxValue)
                    difference = DateTime.Now.Subtract(parking.ParkStart);
                else
                    difference = parking.ParkEnd.Subtract(parking.ParkStart);
                return difference;
            }
            return TimeSpan.Zero;
        }


        public async Task<double> CalculateRevenue()
        {
            double revenue = 0;
            foreach (var transaction in await _context.Parkings.ToListAsync())
            {
                revenue += TollAccess.CalculateCharges(CalculateParkedTime(transaction), transaction.Service);
            }
            return revenue;
        }
    }
}
