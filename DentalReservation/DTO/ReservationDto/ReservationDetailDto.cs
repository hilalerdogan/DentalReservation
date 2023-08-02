using DentalReservation.Api.Entity;
using bdto = DentalReservation.Api.DTO.BaseDto;

namespace DentalReservation.Api.DTO.ReservationDto
{
    public class ReservationDetailDto : bdto.BaseDTO
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }

        public ReservationDetailDto(Reservation reservation)
        {
            Name = reservation.Name;
            Date = reservation.Date;
            Duration = reservation.Duration;
        }
    }
}
