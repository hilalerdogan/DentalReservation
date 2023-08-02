using DentalReservation.Api.Entity;
using Microsoft.EntityFrameworkCore;

namespace DentalReservation.Api.DatabaseContext
{
    public class ReservationContext : DbContext
    {
        public DbSet<Reservation> Reservations { get; set; }    

        //constructor didn'T understane take a look
    }
}
