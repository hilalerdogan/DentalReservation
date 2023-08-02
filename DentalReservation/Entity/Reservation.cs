namespace DentalReservation.Api.Entity
{
    //belki başka dtolar eklenebilir
    public class Reservation : BaseEntity
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }

    }
}
