using Microsoft.AspNetCore.Mvc;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using GarageCoreMVC.Common;
using GarageCoreMVC.Services;
using GarageCoreAPI.Models;

namespace GarageCoreMVC.Controllers
{
    public class VehicleController : Controller
    {
        private IGarage garage;
        private IToll toll;
        private IParking parking;
        private IRepair repair;

        public VehicleController(IGarage garage, IToll toll, IParking parking, IRepair repair)
        {
            this.garage = garage;
            this.toll = toll;
            this.parking = parking;
            this.repair = repair;
        }

        [HttpPost]
        [Route(Urls.ParkVehicle)]
        [Authorize]
        public async Task<IActionResult> Park([FromBody] Vehicle vehicle)
        {
            if (vehicle == null) return BadRequest(DefaultValues.VehicleDataUnavailableMessage);
            else
            {
                if (garage.CheckValidityDirect(vehicle))
                {
                    if (await garage.DoesVehicleExist(vehicle.RegID))
                    {
                        return Json(new ResponseMessage { Success = false, Message = DefaultValues.VehicleDuplicateErrorMessage });
                    }
                    else
                    {
                        await garage.ParkVehicle(vehicle);
                        return Json(new ResponseMessage { Success = true, Message = $"Vehicle with ID: {vehicle.RegID} is parked successfully !" });
                    }
                }
                return Json(new ResponseMessage { Success = false, Message = DefaultValues.ValidationFailedMessage });
            }

        }

        [HttpGet]
        [Route(Urls.ListVehicles)]
        [Authorize]
        public async Task<ActionResult> Info()
        {
            return Json(await garage.GetVehicles());
        }

        [HttpGet]
        [Route(Urls.ListVehiclesNotOnRepair)]
        [Authorize]
        public async Task<ActionResult> InfoNotOnRepair()
        {
            return Json(await garage.GetVehiclesNotOnRepair());
        }

        [HttpDelete]
        [Route(Urls.RemoveVehicle)]
        [Authorize(Roles = "Sudo")]
        public async Task<ActionResult> Remove([FromQuery] string reqID)
        {
            var spot = await toll.GetSpotByVID(reqID);
            if (spot != null)
            {
                var vehicle = await garage.GetVehicleByID(reqID);
                if (vehicle != null)
                {
                    if (vehicle.IsOnRepair) return Json(new ResponseMessage { Success = false, Message = $"Vehicle with ID: {reqID} can't be removed due to pending repairs! " });
                    await repair.ArchiveRepairTransactions(reqID);
                    parking.CheckoutParking(spot.ID, reqID);
                    await toll.UpdateSpotOccupancy(spot.ID, false);
                    await garage.RemoveVehicle(reqID);
                    return Json(new ResponseMessage { Success = true, Message = $"Vehicle with ID: {reqID} is successfully removed! " });
                }
                return Json(new ResponseMessage { Success = false, Message = $"Vehicle with ID: {reqID} doesn't exist ! " });
            }
            else
            {
                await garage.RemoveVehicle(reqID);
                return Json(new ResponseMessage { Success = true, Message = $"Vehicle with ID: {reqID} is successfully removed! " });
            }

        }

        [HttpPost]
        [Authorize(Roles = "Sudo")]
        [Route(Urls.UpdateVehicle)]
        public async Task<ActionResult> Edit([FromBody] Vehicle vehicle)
        {
            if (vehicle != null)
            {
                if (garage.CheckValidityDirect(vehicle))
                {
                    await garage.UpdateVehicle(vehicle);
                    return Json(new ResponseMessage { Success = true, Message = $"Vehicle with ID <{vehicle.RegID} is updated successfully !" });
                }
                else return Json(new ResponseMessage { Success = false, Message = DefaultValues.ValidationFailedMessage });
            }
            else return BadRequest(DefaultValues.VehicleDataUnavailableMessage);
        }

        [HttpGet]
        [Authorize]
        [Route(Urls.GetSpecificVehicle)]
        public async Task<ActionResult> GetSpecifics([FromQuery] string reqID)
        {
            Vehicle? vehicle = await garage.GetVehicleByID(reqID);
            Spot? spot = await toll.GetSpotByVID(reqID);
            ParkingTransaction? pt = await parking.GetParkingByID(spot.ID, reqID);
            if (vehicle == null || pt == null)
            {
                return BadRequest($"Vehicle with ID: {reqID} doesn't exist !");
            }
            else
            {
                var vehicleDto = new VehicleDto
                {
                    Name = vehicle.Name,
                    Vendor = vehicle.Vendor,
                    Wheels = vehicle.Wheels,
                    RegID = vehicle.RegID,
                    ParkedSpotID = spot.ID,
                    ParkedSince = pt.ParkStart.ToString(),
                    ParkedFor = parking.CalculateParkedTime(pt).ToString(),
                };
                return Json(vehicleDto);
            }
        }


        [HttpGet]
        [Route(Urls.GetStatistics)]
        public async Task<ActionResult> GetStatistics()
        {
            List<Vehicle> vehicles = await garage.GetVehicles();
            var twoWheelersCount = vehicles.Where(v => v.Wheels == 2).Count();
            var fourWheelersCount = vehicles.Where(v => v.Wheels == 4).Count();

            return Json(new StatisticsDto
            {
                TwoWheelers = twoWheelersCount,
                FourWheelers = fourWheelersCount,
                SpecialWheelers = vehicles.Count() - twoWheelersCount - fourWheelersCount,
                VehiclesOnRepair = (await garage.GetVehicles()).Count() - (await garage.GetVehiclesNotOnRepair()).Count(),
                VacantSpots = (await toll.GetVacantSpots()).Count(),
                RevenueFromToll = await parking.CalculateRevenue(),
                RevenueFromRepairs = await repair.CalculateRevenue(),
            });
        }

        [HttpGet]
        [Route("/revenue/series")]
        public async Task<ActionResult> GetParkingTimeSeriesData()
        {
            var allDaysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();
            var parkingRevenueByDay = (await parking.GetParkingTransactions()).GroupBy(p => p.ParkStart.DayOfWeek)
                .Select(t => new
                {
                    day = t.Key,
                    amount = t.Sum(t => TollAccess.CalculateCharges(parking.CalculateParkedTime(t), t.Service))
                }
                );
            var repairRevenueByDay = (await repair.GetAllRepairTransactions()).GroupBy(r => r.TransactionStamp.DayOfWeek)
                .Select(t => new
                {
                    day = t.Key,
                    amount = t.Sum(t => t.Charge)
                }
                );
            //var tableJoined = parkingRevenueByDay.GroupJoin(repairRevenueByDay, p => p.day, r => r.day
            //           (p, r) => new {day = p.day, amountToll = p.amount, amountRepairs = r.amount}
            //      ).OrderBy(tj => tj.day).ToList();
            var weeklyRevenueData = allDaysOfWeek.Select(
                    a => new ParkingByDayDto
                    {
                        Day = a.ToString(),
                        AmountToll = parkingRevenueByDay.FirstOrDefault(p => p.day == a)?.amount ?? 0,
                        AmountRepairs = repairRevenueByDay.FirstOrDefault(r => r.day == a)?.amount ?? 0
                    }
            );    
            return Json(weeklyRevenueData.ToList());
    }
}
}
