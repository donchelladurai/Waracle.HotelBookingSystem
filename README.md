# Hotel Booking API - By Don Chelladurai
  Don Chelladurai's solution for the "Backend Developer Challenge"

## Built with
- Visual Studio 2022
- .NET 8.0
- EF Core 9.0
- Azure SQL DB
- Azure Application Insights
- Hosted as an Azure APP Service

The design uses an onion architecture with the repository layer separated from Infrastructure, to allow for multiple data sources. I assumed a hotel booking system would refer to multiple third party data sources or vendors to fetch information, besides its own data database or databases. I wanted the infrastructure layer to be able to handle these using separate projects which can be developed and scaled independently.

## Hosted On
https://waracle-hotelbookingsystem-webapi-fjgpd5d5hwdrbxbt.canadacentral-01.azurewebsites.net/swagger

### Please note: 
Azure seems to be throttling connections to the free version of Azure SQL that I've used, likely because it is hosted in a shared resource. If an HTTP Request times out in the testable link and returns an HTTP 500, please try again 2 or 3 times. I've set the connection time out to 300 seconds and retries to 10 in the connection string that is published. 

## Endpoints

### Data (Used for seeding and resetting data)

| HTTP Method | Endpoint                       | Description                                 |
|--------|-------------------------------------|---------------------------------------------|
| DELETE | /api/data/clear                     | Removes all transactional data in the database 
| POST   | /api/data/seed                      | Seeds the database with test data

### Bookings

| HTTP Method | Endpoint                      | Description                                 |
|--------|------------------------------------|---------------------------------------------|
| GET    | /api/bookings                      | Get all bookings in the database       
| POST   | /api/bookings                      | Book a room at a hotel              
| GET    | /api/bookings/{bookingReference}}  | Get a booking using a booking reference

### Hotels

| HTTP Method | Endpoint                      | Description                                 |
|--------|------------------------------------|---------------------------------------------|
| GET    | /api/hotels/name                   | Gets all hotels that match the given name

### Rooms

| HTTP Method | Endpoint                      | Description                                                                             |
|--------|------------------------------------|-----------------------------------------------------------------------------------------|
| GET    | /api/rooms                         | Gets all rooms in all hotels between two given dates for the given number of occupants

## Database
  Azure SQL

  ### Servername
  waracle-hotel-booking-system.database.windows.net

  ### Username
  waracledbadmin

  ### Password
  Monday!2E

## Third Party Dependencies
- Mediatr
- FluentValidation
- NUnit
- Moq
- RestSharp
- Newtonsoft.Json
- Swagger

