using DentalReservation.Api.Entity;

namespace DentalReservation.Api.DTO.ReservationDto
{
    public class newReservationDto
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }

        public newReservationDto()
        {

        }

        public Reservation ToReservation()
        {
            return new Reservation()
            {
                Id = 0,
                Name = this.Name,
                Date = this.Date,
                Duration = this.Duration
            };
        }
    }
}
