using Foodfacilities.Models;

namespace Foodfacilities.Interfaces
{
    public interface IFoodFacilityService
    {
        Task<List<FoodFacility>> SearchByApplicantNameAsync(string applicantName, string? status);
        Task<List<FoodFacility>> SearchByStreetNameAsync(string streetName);
        Task<List<FoodFacility>> SearchByGeoLocationAsync(double latitude, double longitude, string? status);
    }
}
