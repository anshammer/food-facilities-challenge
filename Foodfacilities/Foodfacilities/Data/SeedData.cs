using CsvHelper;
using CsvHelper.Configuration;
using Foodfacilities.Models;
using Foodfacilities.SeedData;
using System.Globalization;

namespace Foodfacilities.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            if (!db.FoodFacilities.Any())
            {
                var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
                var csvPath = Path.Combine(env.ContentRootPath, "Data", "Mobile_Food_Facility_Permit.csv");

                if (File.Exists(csvPath))
                {
                    using var reader = new StreamReader(csvPath);
                    using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HeaderValidated = null,
                        MissingFieldFound = null
                    });

                    var records = csv.GetRecords<FoodFacility>().ToList();

                    db.FoodFacilities.AddRange(records);
                    db.SaveChanges();
                }
                else
                {
                    Console.WriteLine($"CSV file not found at path: {csvPath}");
                }
            }

        }
    }
}
