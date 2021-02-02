using System.Threading.Tasks;
using ExternalDependencies;
using Microsoft.AspNetCore.Mvc;

namespace SeatReservations.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/data_for_reservation_seats")]
    public class ReservationSeatsController : ControllerBase
    {
        private readonly IProvideCurrentReservations _provideCurrentReservations;

        public ReservationSeatsController(IProvideCurrentReservations provideCurrentReservations)
        {
            _provideCurrentReservations = provideCurrentReservations;
        }

        // GET api/data_for_reservation_seats/5
        [HttpGet("{showId}")]
        public async Task<ReservedSeatsDto> Get(string showId)
        {
            return await _provideCurrentReservations.GetReservedSeats(showId);
        }
    }
}