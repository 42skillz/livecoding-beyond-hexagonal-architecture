using System.Collections.Generic;
using System.Threading.Tasks;
using ExternalDependencies;
using SeatsSuggestions.Domain;
using SeatsSuggestions.Domain.Ports;

namespace SeatsSuggestions.Infra.Adapter
{
    /// <summary>
    ///     Adapt DTO instances coming from the external dependencies and Bounded Contexts (ReservationsProvider, AuditoriumLayoutRepository)
    ///     to instances of AuditoriumSeating (i.e. ).
    /// </summary>
    public class AuditoriumSeatingAdapter : IProvideUpToDateAuditoriumSeating
    {
        private readonly IProvideCurrentReservations _seatsReservationsProviderWebClient;
        private readonly IProvideAuditoriumLayouts _auditoriumLayoutProviderWebClient;

        public async Task<AuditoriumSeating> GetAuditoriumSeating(ShowId showId)
        {
            // Call the AuditoriumLayout Bounded Context in order to get an empty AuditoriumSeating
            var auditoriumDtoTask = _auditoriumLayoutProviderWebClient.GetAuditoriumSeatingFor(showId.Id);

            // Call the SeatReservation Bounded Context to get the list of already reserved seats
            var reservedSeatsDtoTask = _seatsReservationsProviderWebClient.GetReservedSeats(showId.Id);

            await Task.WhenAll(auditoriumDtoTask, reservedSeatsDtoTask);

            var auditoriumDto = auditoriumDtoTask.Result;
            var reservedSeatsDto = reservedSeatsDtoTask.Result;

            // Adapt all these information into a type belonging to our SeatSuggestion Bounded Context (External Domains/BCs INFRA => Our Domain)
            var auditoriumSeating = AdaptAuditoriumSeatingDto(auditoriumDto, reservedSeatsDto);

            return auditoriumSeating;
        }

        public AuditoriumSeatingAdapter(IProvideAuditoriumLayouts auditoriumLayoutProviderWebClient, IProvideCurrentReservations seatsReservationsProviderWebClient)
        {
            _auditoriumLayoutProviderWebClient = auditoriumLayoutProviderWebClient;
            _seatsReservationsProviderWebClient = seatsReservationsProviderWebClient;
        }


        /// <summary>
        /// Adapt 2 external BCs into 1 (our BC).
        /// </summary>
        /// <param name="auditoriumDto">The topology of an Auditorium coming from the AuditoriumLayout.API</param>
        /// <param name="reservedSeatsDto">The list of already reserved seats coming from the SeatReservations.API</param>
        /// <returns>An instance of <see cref="AuditoriumSeating"/> (i.e. The topology of the Auditorium with seats availability mapped)</returns>
        private static AuditoriumSeating AdaptAuditoriumSeatingDto(AuditoriumDto auditoriumDto, ReservedSeatsDto reservedSeatsDto)
        {
            var rows = new Dictionary<string, Row>();

            foreach (var (rowName, seatDtos) in auditoriumDto.Rows)
            {
                var seats = new List<Seat>();
                foreach (var seatDto in seatDtos)
                {
                    var number = ExtractSeatNumber(seatDto.Name);
                    var priceCategory = ConvertCategory(seatDto.Category);

                    var isReservationsSeat = reservedSeatsDto.ReservedSeats.Contains(seatDto.Name);
                    var seatAvailability = isReservationsSeat ? SeatAvailability.Reserved : SeatAvailability.Available;

                    seats.Add(new Seat(rowName, number, priceCategory, seatAvailability));
                }

                rows[rowName] = new Row(rowName, seats);
            }

            return new AuditoriumSeating(rows);
        }

        private static PricingCategory ConvertCategory(int dtoPricingCategory)
        {
            return (PricingCategory) dtoPricingCategory;
        }

        private static uint ExtractSeatNumber(string seatName)
        {
            return uint.Parse(seatName.Substring(1));
        }
    }
}