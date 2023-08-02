using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalReservation.Api.Entity;
using DentalReservation.Api.DTO.ReservationDto;
using DentalReservation.Api.DatabaseContext;
using Microsoft.EntityFrameworkCore.Query;
using DentalReservation.Api.DTO.ReservationDto;

namespace DentalReservation.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationContext _reservationContext;

        public ReservationController(ReservationContext reservationContext)
        {
            _reservationContext = reservationContext;
        }

        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> GetAll()
        {
            var result = _reservationContext.Reservations.
                Include(reservation => reservation.Name).
                Include(reservation => reservation.Date).
                Include(reservation => reservation.Duration);

            if (result == null)
            {
                return NotFound();
            }

            var reservationDetailList = new List<ReservationDetailDto>();
            result.ForEachAsync(reservation => reservationDetailList.Add(new ReservationDetailDto(reservation)));
            return Ok(reservationDetailList);


        }


        [HttpPost]
        [Route(("checkavailability"))]
        public IActionResult CheckAvailability(DateTime reservationDateTime, int durationInHours)
        {

            if (!IsWorkingDay(reservationDateTime))
            {
                return Ok(new AvailabilityStatus { IsAvailable = false, Message = "The selected date is not a working day." });
            }

         
            if (!IsWithinShiftHours(reservationDateTime, durationInHours))
            {
                return Ok(new AvailabilityStatus { IsAvailable = false, Message = "The selected time is not within shift hours." });
            }


            if (HasOverlap(reservationDateTime, durationInHours))
            {
                return Ok(new AvailabilityStatus { IsAvailable = false, Message = "The selected time slot is already reserved." });
            }


            return Ok(new AvailabilityStatus { IsAvailable = true, Message = "The selected time slot is available." });
        }


        private bool IsWorkingDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }

        private bool IsWithinShiftHours(DateTime reservationDateTime, int durationInHours)
        {

            TimeSpan startTime = new TimeSpan(9, 0, 0);
            TimeSpan endTime = new TimeSpan(17, 0, 0);

            TimeSpan requestedStartTime = reservationDateTime.TimeOfDay;
            TimeSpan requestedEndTime = reservationDateTime.AddHours(durationInHours).TimeOfDay;

            return requestedStartTime >= startTime && requestedEndTime <= endTime;
        }


        private bool HasOverlap(DateTime reservationDateTime, int durationInHours)
        {
            var result = _reservationContext.Reservations.
                Include(reservation => reservation.Name).
                Include(reservation => reservation.Date).
                Include(reservation => reservation.Duration);

            
            var reservations = new List<ReservationDetailDto>();
            result.ForEachAsync(reservation => reservations.Add(new ReservationDetailDto(reservation)));

            DateTime requestedEndTime = reservationDateTime.AddHours(durationInHours);


            bool overlap = reservations.Any(r =>
                (reservationDateTime >= r.Date && reservationDateTime < r.Date.AddHours(r.Duration)) ||
                (requestedEndTime > r.Date && requestedEndTime <= r.Date.AddHours(r.Duration)));

            return overlap;
        }


        [HttpPost]
        [Route(("addreservation"))]
        public async Task<IActionResult> AddReservation([FromBody] newReservationDto request)
        {

            if (!IsWorkingDay(request.Date))
            {
                return BadRequest("The selected date is not a working day.");
            }

            if (!IsWithinShiftHours(request.Date, request.Duration))
            {
                return BadRequest("The selected time is not within shift hours.");
            }

            if (HasOverlap(request.Date, request.Duration))
            {
                return BadRequest("The selected time slot is already reserved.");
            }

            var item = _reservationContext.Reservations.Add(request.ToReservation());
            await _reservationContext.SaveChangesAsync();
            var newEntity = _reservationContext.Reservations.
                Include(reservation => reservation.Name).
                Include(reservation => reservation.Date).
                Include(reservation => reservation.Duration).
                Single(x => x.Id == item.Entity.Id);
            return Ok(new ReservationDetailDto(newEntity));
        }
    }

}
