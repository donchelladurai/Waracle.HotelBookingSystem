# Hotel Booking API - By Don Chelladurai
  Don Chelladurai's solution for the "Backend Developer Challenge"

## Hosted On
https://waracle-hotelbookingsystem-webapi-fjgpd5d5hwdrbxbt.canadacentral-01.azurewebsites.net/swagger

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
| GET    | /api/bookings/reference            | Get a booking using a booking reference

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
Mediatr
FluentValidation
NUnit
Moq
RestSharp
Newtonsoft.Json

