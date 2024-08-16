// System namespaces
using GarageCoreMVC.Common;
using GarageCoreMVC.Data;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services.Interfaces;
using GarageCoreMVC.Validators;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace GarageCoreMVC.Services;

public class GarageAccess : IGarage
{
    private readonly GarageDBContext _context;
    private readonly IMemoryCache _mCache;

    // Dependency injection at constructor
    public GarageAccess(GarageDBContext context, IMemoryCache cache)
    {
        _context = context;
        _mCache = cache;
    }

    public async Task<List<Vehicle>> GetVehicles()
    {
        if (!_mCache.TryGetValue(Constants.VehicleCacheKey, out List<Vehicle>? vehicles))
        {
            vehicles = await _context.Vehicles.ToListAsync();
            _mCache.Set(Constants.VehicleCacheKey, vehicles, TimeSpan.FromMinutes(20));
        }
        return vehicles ?? new List<Vehicle>();
    }

    public async Task<List<Vehicle>> GetVehiclesNotOnRepair()
    {
        return await _context.Vehicles.Where(vehicle => !vehicle.IsOnRepair).ToListAsync();
    }

    public async Task SetVehicleOnRepairFlag(string reqID, bool value)
    {
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(vehicle => vehicle.RegID == reqID);
        if (vehicle != null)
        {
            vehicle.IsOnRepair = value;
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
            _mCache.Remove(Constants.VehicleCacheKey);
        }
    }

    public async Task<Vehicle?> GetVehicleByID(string reqID)
    {
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.RegID == reqID);
        return vehicle;
    }

    public async Task ParkVehicle(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
        _mCache.Remove(Constants.VehicleCacheKey);
        Console.WriteLine($"Vehicle with registration ID: <{vehicle.RegID}> is successfully parked !");
    }

    public async Task RemoveVehicle(string reqID)
    {
        Vehicle? vehicle = await GetVehicleByID(reqID);
        if (vehicle != null)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            _mCache.Remove(Constants.VehicleCacheKey);
            Console.WriteLine($"Vehicle with registration ID: <{reqID}> is successfully removed !");
        }
        else
        {
            Console.WriteLine($"Vehicle with registration ID: <{reqID}> doesn't exist !");
        }
    }

    public async Task<bool> DoesVehicleExist(string reqID)
    {
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.RegID == reqID);
        if (vehicle != null) return true;
        else return false;
    }

    public async Task UpdateVehicle(Vehicle vehicle)
    {
        Vehicle? foundVehicle = await GetVehicleByID(vehicle.RegID);
        if (foundVehicle != null)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
            _mCache.Remove(Constants.VehicleCacheKey);
            Console.WriteLine($"Vehicle with registration ID: <{vehicle.RegID}> is updated with new information !");
        }
        else
        {
            Console.WriteLine($"Vehicle with registration ID: <{vehicle.RegID}> doesn't exist !");
        }
    }

    public bool CheckValidityDirect(Vehicle vehicle)
    {
        try
        {
            if (vehicle != null && VehicleValidator.Validate(vehicle)) { }
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
    public IEnumerator<Vehicle> GetEnumerator()
    {
        List<Vehicle> vehicles = _context.Vehicles.ToList();
        foreach (Vehicle v in vehicles)
        {
            yield return v;
        }
    }
}

