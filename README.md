# Food Facilities Search API

## Description

This project is a backend API for searching permitted food facilities in San Francisco. It provides endpoints to query food vendors based on:

- ✅ **Applicant name** – Supports partial and full name matching  
- ✅ **Street name** – Matches vendors based on a substring of the street address  
- ✅ **Geographic proximity** – Returns the 5 nearest vendors to a given latitude and longitude  

The API is built using **ASP.NET Core Web API** and uses **Entity Framework Core** to load and query data from the official food facility CSV. It supports **Docker** for containerized deployment and includes automated **xUnit** test coverage.

### Problem & Solution

The challenge was to expose a simple and flexible interface over a dataset of permitted food facilities, enabling real-world search use cases for end users. This solution reads data from a CSV file and offers a REST API to query based on name, address, or proximity—common queries that city residents or visitors might perform.

### Key Features

- Full-text search by **applicant** or **street name**  
- Location-based search using **latitude/longitude**, returning the **5 closest vendors**  
- **Dockerized** for easy local setup and deployment  
- **xUnit Automated Test Coverage** using an **in-memory database** for fast and reliable tests  

---

## Technical and Architectural Decisions

- **ASP.NET Core Web API:** For modern, high-performance REST service with good ecosystem and tooling.
- **Entity Framework Core:** Simplifies data access with LINQ and supports test-friendly in-memory DB.
- **Docker:** Ensures consistent behavior across environments and simplifies deployment. Tests are executed during the Docker build to verify the integrity of the codebase.
- **xUnit:** Well-supported testing framework that integrates seamlessly with .NET.
- **In-memory DB for tests:** Enables fast, isolated unit and integration tests without external dependencies.
- **SQLite:** Lightweight database for storing food facility data.
- **Swagger:** API documentation tool for generating interactive API documentation.
- **CsvHelper:** Library for reading CSV files.
- **ENUM:** Used to define the possible values for the `status` field to ensure type safety and clarity in the codebase. Possibly, the UI could also handle sending the correct value in a production environment.
- **MaxResults:** Added a `maxResults` parameter to limit the number of results returned by the API. This would be a configuration option in a production environment to avoid overwhelming clients with too many results.

---

## Critique Section

### 🛠 What I Would Have Done Differently with More Time

- **Persistent Database Seeding:** Currently, the SQLite database is seeded in-memory from the CSV data each time the application starts. With more time, I would implement persistent database storage to avoid reseeding on every run, improving startup performance and enabling incremental updates to the data.
- **Add a Maximum Distance Filter:** The API returns the 5 closest food facilities without restricting by an absolute distance or radius. I would add a `maxRadiusDistance` parameter to avoid returning results that are too far away.
- **Implement Logging and Metrics:** I would integrate logging and monitoring tools to improve observability and aid troubleshooting in production environments.
- **Add Pagination:** To handle larger datasets efficiently, I would add pagination to API responses for better performance and usability.
- **Improve Input Validation:** Stricter input validation would ensure that all incoming parameters are valid, reducing potential errors and improving robustness.
- **External API for Food Facilities:** Used the external SFO GOV API for food facilities instead of the CSV file to fetch live data.
- **Enhance Location-Based Searches:** By incorporating geospatial indexing or external mapping services, location queries could become more accurate and performant.

### Trade-offs Made

- **In-Memory Seeding vs Persistent Storage:** I seed SQLite with CSV data each time the app starts to keep deployment simple. However, this approach can be less efficient for larger datasets or frequent data changes compared to a persistent, continuously updated database.
- **Synchronous Calls:** Used synchronous calls in some controller methods for simplicity rather than fully async.
- **In-Memory Database in Tests:** Opted for in-memory database in tests instead of integration tests with a real database.
- **No Caching:** No caching implemented due to the small dataset.

### Things Left Out

- **User Authentication:** No user authentication or authorization mechanisms.
- **API Versioning:** Not implemented due to the limited scope of this project. 
- **API Rate Limiting:** No API rate limiting to protect against excessive requests.
- **Frontend UI or Client Application:** No frontend UI or client application provided.
- **Comprehensive Input Validation Middleware:** Input validation could be more robust.
- **External API for Geolocation:** No external API used for geolocation-based searches.

---

## Problems and Scaling Considerations

- Without indexing, linear database queries will slow down as data grows.
- Lack of caching means every query hits the database, increasing response times under load.
- Absence of pagination may cause large payloads and slow client performance.
- Loading and seeding data at startup increases memory usage as the dataset grows.
- Limited error handling and retry logic could reduce reliability during failures.
- Location searches without spatial indexes are inefficient.
- No authentication or rate limiting exposes the API to misuse and denial-of-service risks.

---

## How Would I Scale This to Many Users?

- **Use a Persistent Database:** Move from startup seeding to a continuously updated persistent database for better data management.
- **Implement Spatial Indexing:** Use spatially-enabled databases (e.g., PostGIS, SQL Server geography types) for efficient location queries.
- **Optimize Queries:** Add indexes on searchable fields such as applicant name, status, and location.
- **Add Caching:** Use caching layers like Redis to reduce database load for frequent queries.
- **Enable Pagination:** Limit response sizes to improve client and server performance.
- **Support Asynchronous Processing:** Use async methods to improve throughput under high concurrency.
- **Introduce Rate Limiting:** Protect the API from excessive requests and potential attacks.
- **Horizontal Scaling:** Deploy multiple instances behind load balancers to distribute traffic.

---

## Steps to Run the Solution and Tests

### Prerequisites

- .NET 8 SDK installed  
- Docker installed (optional for containerized runs)

### Run Locally

```bash
dotnet build
dotnet run --project Foodfacilities

## Running with Docker

Build the Docker image:

```bash
docker build -t foodfacilities-api .
```

Run the Docker container:

```bash
docker run -d -p 8080:8080 --name foodfacilities-api foodfacilities-api
```
Access the API at http://localhost:8080/swagger
