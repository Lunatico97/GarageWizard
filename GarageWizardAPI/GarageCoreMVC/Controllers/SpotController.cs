using Microsoft.AspNetCore.Mvc;
using GarageCoreMVC.Models;
using GarageCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using GarageCoreMVC.Common.Enums;
using GarageCoreMVC.Common;
using GarageCoreAPI.Models;

namespace GarageCoreMVC.Controllers
{
    public class SpotController : Controller
    {
        private IToll toll;
        private IParking parking;
        public SpotController(IToll toll, IParking parking)
        {
            this.toll = toll;
            this.parking = parking;
        }

        [HttpGet]
        [Authorize]
        [Route(Urls.ListSpots)]
        public async Task<ActionResult> Info()
        {
            return Json(await toll.GetSpots());
        }

        [HttpPost]
        [Route(Urls.CreateSpot)]
        [Authorize(Roles = "Sudo")]
        public async Task<ActionResult> Add([FromBody] Spot spot)
        {
            if (spot != null && toll.CheckValidityDirect(spot))
            {
                if (await toll.CheckIfSpotExists(spot.ID))
                {
                    return Json(new ResponseMessage{ Success = false, Message = $"A spot with <{spot.ID}> already exist !" });
                }
                await toll.AddSpot(spot.ID, spot.Capacity);
                return Json(new ResponseMessage{ Success = true, Message = $"Spot with <{spot.ID} is added successfully !" });
            }
            return Json(new ResponseMessage{ Success = false, Message = DefaultValues.ValidationFailedMessage });
        }

        [HttpDelete("{spotID}")]
        [Route(Urls.RemoveSpot)]
        [Authorize(Roles ="Sudo")]
        public async Task<ActionResult> Remove([FromQuery] string spotID)
        {
            if (await toll.IsThisSpotVacant(spotID))
            {
                await toll.RemoveSpot(spotID);
                return Json(new ResponseMessage{ Success = true, Message = $"Spot with <{spotID}> is deleted !!" });
            }
            return Json(new ResponseMessage{ Success = false, Message = $"The spot with <{spotID}> is not vacant so, it can't be deleted !" });
        }

        [HttpGet]
        [Authorize]
        [Route(Urls.ReserveSpotCheck)]
        public async Task<ActionResult> CheckSpotsToReserve([FromRoute] string vecID, [FromRoute] int wheels)
        {
            return Json(await toll.GetVacantUsableSpots(wheels));
        }

        [HttpPost]
        [Authorize]
        [Route(Urls.ReserveSpotInstant)]
        public async Task<IActionResult> Reserve([FromBody] Reservation reservation)
        {
            if (await toll.AreThereVacantSpots())
            {
                if (reservation != null && !string.IsNullOrWhiteSpace(reservation.spotID) && !string.IsNullOrWhiteSpace(reservation.vehicleID))
                {
                    if (await toll.IsThisSpotVacant(reservation.spotID))
                    {
                        await toll.UpdateSpotOccupancy(reservation.spotID, true);
                        parking.AddParking(reservation.spotID, reservation.vehicleID, (ServiceT)reservation.service);
                        return Json(new ResponseMessage { Success = true, Message = $"Reservation successful for {reservation.vehicleID} at {reservation.spotID} !!" });
                    }
                    else return Json(new ResponseMessage { Success = false, Message = $"The spot with <{reservation.spotID}> is already occupied !" });
                }
                return Json(new ResponseMessage { Success = false, Message = DefaultValues.SpotIDNotProvidedMessage });
            }
            return Json(new ResponseMessage { Success = false, Message = DefaultValues.ParkingSpaceFullMessage }); ;
        }
    }
}
