using Foodfacilities.Controllers;
using Foodfacilities.Models;
using Foodfacilities.SeedData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodfacilities.Tests
{
    public class FoodFacilitiesControllerTests
    {

        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var context = new AppDbContext(options);

            // Seed test data
            context.FoodFacilities.AddRange(new List<FoodFacility>
            {
                new FoodFacility { Applicant = "Joe's Tacos", Status = "APPROVED", Address = "123 California St",  Latitude = 37.7749, Longitude = -122.4194},
                new FoodFacility { Applicant = "Patty's Burgers", Status = "APPROVED", Address = "357 Sansome St", Latitude = 37.7755,Longitude = -122.4187},
                new FoodFacility { Applicant = "Top's Donuts", Status = "REQUESTED", Address = "753 Post Ave",  Latitude = 40.7128,Longitude = -74.0060 },
                new FoodFacility { Applicant = "Jim's Sliders", Status = "EXPIRED", Address = "123 Thorne St", Latitude = 37.7758,Longitude = -122.4179},
                new FoodFacility { Applicant = "Jake's Pizza", Status = "APPROVED", Address = "888 Marion St", Latitude = 37.7742,Longitude = -122.4185},
                new FoodFacility { Applicant = "Bing's Icecreams", Status = "REQUESTED", Address = "456 Post Ave", Latitude = 37.7751,Longitude = -122.4199 },
                new FoodFacility { Applicant = "Pacha's Coffee", Status = "REQUESTED", Address = "111 Queen Ave",  Latitude = 54.7128,Longitude = -64.0060 },
            });

            context.SaveChanges();
            return context;
        }

        [Fact]
        public void GetFoodFacilitiesByName_ShouldReturnMatchingFacilities()
        {
            var controller = new FoodFacilitiesController(GetInMemoryDbContext());

            var result = controller.GetFoodFacilitiesByName("Taco", "APPROVED");

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var facilities = okResult.Value as List<FoodFacility>;
            Assert.Equal("APPROVED", facilities[0].Status);
            Assert.All(facilities, f => Assert.Contains("taco", f.Applicant, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void GetFoodFacilitiesByName_ShouldReturnNoMatchingFacilities()
        {
            var dbContext = GetInMemoryDbContext();
            var controller = new FoodFacilitiesController(dbContext);

            var result = controller.GetFoodFacilitiesByName("NONEXISTENT", null);
            var notfoUdResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No food facilities found for the applicant name: NONEXISTENT", notfoUdResult.Value);
        }

        [Fact]
        public void GetFoodFacilitiesByName_ShouldReturnNoMatchingFacilitiesForStatus()
        {
            var dbContext = GetInMemoryDbContext();
            var controller = new FoodFacilitiesController(dbContext);

            var result = controller.GetFoodFacilitiesByName("Jim's", "APPROVED");
            var notfoUdResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No food facilities found for the applicant name: Jim's", notfoUdResult.Value);
        }

        [Fact]
        public void GetFoodFacilitiesByName_WithInvalidStatus_ReturnsBadRequest()
        {
            var dbContext = GetInMemoryDbContext();
            var controller = new FoodFacilitiesController(dbContext);

            var result = controller.GetFoodFacilitiesByName("Joe", "REJECTED");

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid status value. Allowed values are: APPROVED, REQUESTED, EXPIRED.", badRequestResult.Value);
        }

        [Fact]
        public void GetFoodFacilitiesByLocation_ShouldReturnMatchingFacilities()
        {
            var controller = new FoodFacilitiesController(GetInMemoryDbContext());
            var result = controller.GetFoodFacilitiesByGeoLocation(37.7750, -122.4194, null);

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var facilities = okResult.Value as List<FoodFacility>;
            Assert.NotEmpty(facilities);
        }


        [Fact]
        public void GetFoodFacilitiesByLocation_ShouldReturnMatchingFacilitiesWithStatus()
        {
            var controller = new FoodFacilitiesController(GetInMemoryDbContext());
            var result = controller.GetFoodFacilitiesByGeoLocation(37.7750, -122.4194, "REQUESTED");

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var facilities = okResult.Value as List<FoodFacility>;
            Assert.NotEmpty(facilities);
            Assert.All(facilities, f => Assert.Equal("REQUESTED", f.Status, StringComparer.OrdinalIgnoreCase));
        }

        [Fact]
        public void GetFoodFacilitiesByStreetName_ShouldReturnMatchingFacilities()
        {
            var controller = new FoodFacilitiesController(GetInMemoryDbContext());
            var result = controller.GetFoodFacilitiesByStreet("California");

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var okResult = result as OkObjectResult;
            var facilities = okResult.Value as List<FoodFacility>;
            Assert.NotEmpty(facilities);
            Assert.All(facilities, f => Assert.Contains("California", f.Address, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void GetFoodFacilitiesByStreetName_ShouldReturnNoMatchingFacilities()
        {
            var controller = new FoodFacilitiesController(GetInMemoryDbContext());
            var streetName = "NonExistentStreet";
            var result = controller.GetFoodFacilitiesByStreet(streetName);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"No food facilities found on the street name starting with: {streetName}", notFoundResult.Value);
        }

    }
}
