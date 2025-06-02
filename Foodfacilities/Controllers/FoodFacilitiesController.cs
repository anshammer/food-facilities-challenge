using Foodfacilities.Models;
using Foodfacilities.SeedData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Foodfacilities.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodFacilitiesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private const int MaxResults = 50; // Maximum results to return for street search

        public FoodFacilitiesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves food facilities by partial or full applicant name and optional status.
        /// </summary>
        /// <param name="applicantName">The name (or partial name) of the applicant to search for. Required.</param>
        /// <param name="status">Optional status filter (e.g., APPROVED, REQUESTED, EXPIRED).</param>
        /// <returns>A list of matching food facilities, or 400/404 if none or invalid input.</returns>
        /// <response code="200">Returns the list of matching food facilities.</response>
        /// <response code="400">If the status is invalid.</response>
        /// <response code="404">If no results are found for the given name and status.</response>
        [HttpGet("searchbyapplicantname")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult GetFoodFacilitiesByName([Required][FromQuery] string applicantName, [EnumDataType(typeof(FoodFacilitiesStatus))] string? status)
        {

            IQueryable<FoodFacility> query = _dbContext.FoodFacilities;

            // Filter on partial match of applicant name (case-insensitive LIKE)
            query = query.Where(f => EF.Functions.Like(f.Applicant, $"%{applicantName}%"));

            // If a valid status is provided, filter by status
            if (status != null)
            {
                if (Enum.TryParse<FoodFacilitiesStatus>(status, true, out var facilityStatus))
                {
                    query = query.Where(f => EF.Functions.Like(f.Status, $"%{status}"));
                }
                else
                {
                    return BadRequest("Invalid status value. Allowed values are: APPROVED, REQUESTED, EXPIRED.");
                }
            }

            var result = query.ToListAsync().Result;

            if (result.Count == 0)
            {
                return NotFound($"No food facilities found for the applicant name: {applicantName}");
            }
            return Ok(result);
        }

        /// <summary>
        /// Searches food facilities by partial or full street name.
        /// </summary>
        /// <param name="streetName">The street name or partial name to search for. Required.</param>
        /// <returns>A list of food facilities on the specified street, limited by result size.</returns>
        /// <response code="200">Returns the list of matching food facilities.</response>
        /// <response code="400">If street name is empty or too many results found.</response>
        /// <response code="404">If no matching records are found.</response>
        [HttpGet("searchbystreetname")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult GetFoodFacilitiesByStreet([Required][FromQuery] string streetName)
        {
            if (string.IsNullOrWhiteSpace(streetName))
            {
                return BadRequest("Street name cannot be null or empty.");
            }
            IQueryable<FoodFacility> query = _dbContext.FoodFacilities;

            // Perform partial string match on address
            query = query.Where(f => EF.Functions.Like(f.Address, $"%{streetName}%"));

            var result = query.ToListAsync().Result;

            if (result.Count == 0)
            {
                return NotFound($"No food facilities found on the street name starting with: {streetName}");
            }
            else if (result.Count > MaxResults)
            {
                return BadRequest("Too many results found. Please refine your search.");
            }
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the 5 closest food facilities to a given geographic coordinates, filtered optionally by status.
        /// </summary>
        /// <param name="status">Optional status filter (e.g., APPROVED, REQUESTED, EXPIRED).</param>
        /// <returns>A list of the 5 closest food facilities to the given coordinates.</returns>
        /// <response code="200">Returns the nearest food facilities.</response>
        /// <response code="404">If no facilities are found near the location.</response>
        [HttpGet("searchbylocation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult GetFoodFacilitiesByGeoLocation([Required][FromQuery] double latitude, [Required][FromQuery] double longitude, string? status)
        {
            // Validate status value (if provided)
            if (status != null && !Enum.TryParse<FoodFacilitiesStatus>(status, true, out _))
            {
                return BadRequest("Invalid status value. Allowed values are: APPROVED, REQUESTED, EXPIRED.");
            }

            IQueryable<FoodFacility> query = _dbContext.FoodFacilities;
            query = query.Where(f => f.Latitude != null && f.Longitude != null);

            // Filter by status if provided, otherwise default to APPROVED
            if (status != null)
            {
                query = query.Where(f => EF.Functions.Like(f.Status, $"%{status}"));
            }
            else
            {
                query = query.Where(f => EF.Functions.Like(f.Status, $"%{FoodFacilitiesStatus.APPROVED}"));
            }

            query = query.OrderBy(f => Math.Pow(f.Latitude.Value - latitude, 2)
                                     + Math.Pow(f.Longitude.Value - longitude, 2))
                         .Take(5);

            var result = query.ToListAsync().Result;

            if (result.Count == 0)
            {
                return NotFound($"No food facilities found near the specified location");
            }

            return Ok(result);
        }
    }
}
