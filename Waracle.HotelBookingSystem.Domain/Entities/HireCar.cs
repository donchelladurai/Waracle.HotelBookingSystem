namespace Waracle.HotelBookingSystem.Domain.Entities
{
    public class HireCar
    {
        public HireCar(string hireCompany, string make, string model, int seats)
        {
            this.HireCompany = hireCompany;
            this.Make = make;
            this.Model = model;
            this.Seats = seats;
        }

        public string HireCompany { get; set; }

        public string Make { get; }

        public string Model { get; }

        public int Seats { get; }
    }
}
