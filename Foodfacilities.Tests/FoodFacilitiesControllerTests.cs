using Foodfacilities.Controllers;
using Foodfacilities.Models;
using Foodfacilities.SeedData;
using Foodfacilities.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Foodfacilities.Tests
{
    public class FoodFacilitiesControllerTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // ensures isolation
                .Options;

            var context = new AppDbContext(options);
            context.FoodFacilities.AddRange(new List<FoodFacility>
            {
                new FoodFacility { Applicant = "Joe's Tacos", Status = "APPROVED", Address = "123 California St",  Latitude = 37.7749, Longitude = -122.4194 },
                new FoodFacility { Applicant = "Patty's Burgers", Status = "APPROVED", Address = "357 Sansome St", Latitude = 37.7755, Longitude = -122.4187 },
                new FoodFacility { Applicant = "Top's Donuts", Status = "REQUESTED", Address = "753 Post Ave", Latitude = 40.7128, Longitude = -74.0060 },
                new FoodFacility { Applicant = "Jim's Sliders", Status = "EXPIRED", Address = "123 Thorne St", Latitude = 37.7758, Longitude = -122.4179 },
                new FoodFacility { Applicant = "Jake's Pizza", Status = "APPROVED", Address = "888 Marion St", Latitude = 37.7742, Longitude = -122.4185 },
                new FoodFacility { Applicant = "Bing's Icecreams", Status = "REQUESTED", Address = "456 Post Ave", Latitude = 37.7751, Longitude = -122.4199 },
                new FoodFacility { Applicant = "Pacha's Coffee", Status = "REQUESTED", Address = "111 Queen Ave", Latitude = 54.7128, Longitude = -64.0060 },
            });
            context.SaveChanges();
            return context;
        }

        private FoodFacilitiesController GetController()
        {
            var context = GetInMemoryDbContext();
            var service = new FoodFacilityService(context);
            return new FoodFacilitiesController(service);
        }

        [Fact]
        public async Task GetByApplicantName_ShouldReturnMatchingFacilities()
        {
            var controller = GetController();
            var result = await controller.GetFoodFacilitiesByName("Taco", "APPROVED");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var facilities = Assert.IsType<List<FoodFacility>>(okResult.Value);

            Assert.All(facilities, f =>
            {
                Assert.Equal("APPROVED", f.Status, ignoreCase: true);
                Assert.Contains("taco", f.Applicant, StringComparison.OrdinalIgnoreCase);
            });
        }

        [Fact]
        public async Task GetByApplicantName_ShouldReturnNotFound()
        {
            var controller = GetController();
            var result = await controller.GetFoodFacilitiesByName("NONEXISTENT", null);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No food facilities found for the applicant name: NONEXISTENT", notFound.Value);
        }

        [Fact]
        public async Task GetByApplicantName_InvalidStatus_ShouldReturnBadRequest()
        {
            var controller = GetController();
            var result = await controller.GetFoodFacilitiesByName("Joe", "REJECTED");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid status value. Allowed values are: APPROVED, REQUESTED, EXPIRED.", badRequest.Value);
        }

        [Fact]
        public async Task GetByApplicantName_NoMatchWithStatus_ShouldReturnNotFound()
        {
            var controller = GetController();
            var result = await controller.GetFoodFacilitiesByName("Jim's", "APPROVED");

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No food facilities found for the applicant name: Jim's", notFound.Value);
        }

        [Fact]
        public async Task GetByGeoLocation_ShouldReturnTop5ApprovedFacilities()
        {
            var controller = GetController();
            var result = await controller.GetFoodFacilitiesByGeoLocation(37.7750, -122.4194, null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var facilities = Assert.IsType<List<FoodFacility>>(okResult.Value);
            Assert.InRange(facilities.Count, 1, 5);
        }

        [Fact]
        public async Task GetByGeoLocation_WithStatus_ShouldReturnMatchingFacilities()
        {
            var controller = GetController();
            var result = await controller.GetFoodFacilitiesByGeoLocation(37.7750, -122.4194, "REQUESTED");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var facilities = Assert.IsType<List<FoodFacility>>(okResult.Value);
            Assert.All(facilities, f => Assert.Equal("REQUESTED", f.Status, ignoreCase: true));
        }

        [Fact]
        public async Task GetByGeoLocation_InvalidStatus_ShouldReturnBadRequest()
        {
            var controller = GetController();
            var result = await controller.GetFoodFacilitiesByGeoLocation(37.7750, -122.4194, "REJECTED");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid status value. Allowed values are: APPROVED, REQUESTED, EXPIRED.", badRequest.Value);
        }

        [Fact]
        public async Task GetByStreetName_ShouldReturnMatchingFacilities()
        {
            var controller = GetController();
            var result = await controller.GetFoodFacilitiesByStreet("California");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var facilities = Assert.IsType<List<FoodFacility>>(okResult.Value);
            Assert.All(facilities, f => Assert.Contains("California", f.Address, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetByStreetName_ShouldReturnNotFound()
        {
            var controller = GetController();
            var result = await controller.GetFoodFacilitiesByStreet("Invalid Street");

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No food facilities found with: Invalid Street", notFound.Value);
        }
    }
}
