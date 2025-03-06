using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartyGame.Entities;
using PartyGame.Models.PlaceModels;
using PartyGame.Services;

namespace PartyGame.Controllers
{
    [Route("api/place")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpGet]
        public ActionResult GetAllPlaces(IPlaceService placeService)
        {
            List<Place> places = _placeService.GetAllPlaces().Result;
            return Ok(places);
        }

        [HttpGet("{placeID}")]
        public ActionResult GetPlace([FromRoute] string placeId)
        {
            Place place = _placeService.GetPlaceById(placeId).Result;
            return Ok(place);
        }

        
        [HttpPost("to_check")]
        public ActionResult AddNewPlaceToQueue([FromBody] NewPlaceDto newPlace)
        {
            _placeService.AddNewPlaceToQueue(newPlace);
            return Ok(
                new
                {
                    Message = "Place successfully added to db"
                }
            );
        }

        [HttpGet("to_check")]
        public ActionResult GetAllPlacesInQueue()
        {
            List<PlaceToCheckDto> placesToCheck = _placeService.GetAllPlacesInQueue().Result;

            return Ok(placesToCheck);
        }

        [HttpPost]
        public ActionResult AddNewPlace([FromBody] NewPlaceDto newPlace)
        {
            _placeService.AddNewPlace(newPlace);
            return Ok(
                new
                {
                    Message = "Place successfully added to db"
                }
            );
        }


        [HttpDelete("to_check/reject")]
        public ActionResult RejectPlaceToCheck([FromQuery] string placeId)
        {
            _placeService.RejectPlace(placeId);
            return Ok(
                new
                {
                    Message = "Place successfully rejected"
                }
                );
        }


        [HttpPost("to_check/approve")]
        public ActionResult AcceptPlaceToCheck([FromQuery] string placeId)
        {
            _placeService.AcceptPlace(placeId);
            return Ok(
                new
                {
                    Message = "Place successfully added to places"
                }
            );
        }

    }
}
