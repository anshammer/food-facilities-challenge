using Foodfacilities.Interfaces;
using Foodfacilities.Models;
using Foodfacilities.SeedData;
using Microsoft.EntityFrameworkCore;

namespace Foodfacilities.Services
{
    public class FoodFacilityService : IFoodFacilityService
    {
        private readonly AppDbContext _dbContext;
        private const int MaxResults = 50;

        public FoodFacilityService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<FoodFacility>> SearchByApplicantNameAsync(string applicantName, string? status)
        {
            var query = _dbContext.FoodFacilities.AsQueryable();

            query = query.Where(f => EF.Functions.Like(f.Applicant, $"%{applicantName}%"));

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<FoodFacilitiesStatus>(status, true, out var facilityStatus))
                {
                    query = query.Where(f => f.Status.Equals(facilityStatus.ToString()));
                }
                else
                {
                    throw new ArgumentException("Invalid status value. Allowed values are: APPROVED, REQUESTED, EXPIRED.");
                }
            }

            var result = await query.ToListAsync();

            if (result.Count > MaxResults)
            {
                throw new InvalidOperationException("Too many results found. Please refine your search.");
            }

            return result;
        }

        public async Task<List<FoodFacility>> SearchByStreetNameAsync(string streetName)
        {
            if (string.IsNullOrWhiteSpace(streetName))
                throw new ArgumentException("Street name cannot be null or empty.");

            var query = _dbContext.FoodFacilities
                .Where(f => EF.Functions.Like(f.Address, $"%{streetName}%"));

            var result = await query.ToListAsync();

            if (result.Count > MaxResults)
            {
                throw new InvalidOperationException("Too many results found. Please refine your search.");
            }

            return result;
        }

        public async Task<List<FoodFacility>> SearchByGeoLocationAsync(double latitude, double longitude, string? status)
        {
            var query = _dbContext.FoodFacilities.Where(f =>
                f.Latitude != null && f.Longitude != null &&
                !(f.Latitude == 0 && f.Longitude == 0));

            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<FoodFacilitiesStatus>(status, true, out var facilityStatus))
                {
                    query = query.Where(f => EF.Functions.Like(f.Status, facilityStatus.ToString()));
                }
                else
                {
                    throw new ArgumentException("Invalid status value. Allowed values are: APPROVED, REQUESTED, EXPIRED.");
                }
            }
            else
            {
                query = query.Where(f => EF.Functions.Like(f.Status, $"%{FoodFacilitiesStatus.APPROVED}"));
            }

            var result = await query
                .OrderBy(f => (f.Latitude.Value - latitude) * (f.Latitude.Value - latitude) +
                              (f.Longitude.Value - longitude) * (f.Longitude.Value - longitude))
                .Take(5)
                .ToListAsync();

            return result;
        }
    }
}
