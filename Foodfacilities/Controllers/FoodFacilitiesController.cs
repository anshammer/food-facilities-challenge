using Foodfacilities.Interfaces;
using Foodfacilities.Models;
using Foodfacilities.SeedData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Foodfacilities.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodFacilitiesController : ControllerBase
    {
        private readonly IFoodFacilityService _foodFacilityService;

        public FoodFacilitiesController(IFoodFacilityService foodFacilityService)
        {
            _foodFacilityService = foodFacilityService;
        }

        [HttpGet("searchbyapplicantname")]
        public async Task<IActionResult> GetFoodFacilitiesByName([Required][FromQuery] string applicantName, [EnumDataType(typeof(FoodFacilitiesStatus))] string? status)
        {
            try
            {
                var result = await _foodFacilityService.SearchByApplicantNameAsync(applicantName, status);
                return result.Any() ? Ok(result) : NotFound($"No food facilities found for the applicant name: {applicantName}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("searchbystreetname")]
        public async Task<IActionResult> GetFoodFacilitiesByStreet([Required][FromQuery] string streetName)
        {
            try
            {
                var result = await _foodFacilityService.SearchByStreetNameAsync(streetName);
                return result.Any() ? Ok(result) : NotFound($"No food facilities found with: {streetName}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("searchbylocation")]
        public async Task<IActionResult> GetFoodFacilitiesByGeoLocation([Required][FromQuery] double latitude, [Required][FromQuery] double longitude, string? status)
        {
            try
            {
                var result = await _foodFacilityService.SearchByGeoLocationAsync(latitude, longitude, status);
                return result.Any() ? Ok(result) : NotFound("No food facilities found near the specified location");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
