using System.Threading.Tasks;
using ExternalDependencies;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SeatsSuggestions.Infra.Helpers;

namespace SeatsSuggestions.Infra
{
    /// <summary>
    ///     Get via a web api and adapt reservations and return <see cref="ReservedSeatsDto" />.
    /// </summary>
    public class SeatReservationsWebClient : IProvideCurrentReservations
    {
        private readonly string _uriSeatReservationService;
        private readonly IWebClient _webClient;

        public SeatReservationsWebClient(string uriSeatReservationService, IWebClient webClient)
        {
            _uriSeatReservationService = uriSeatReservationService;
            _webClient = webClient;
        }

        public async Task<ReservedSeatsDto> GetReservedSeats(string showId)
        {
            var jsonSeatReservations = await GetDataForReservations(showId);

            var reservationsSeatsDto = JsonConvert.DeserializeObject<ReservedSeatsDto>(jsonSeatReservations,
                    new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});

            return reservationsSeatsDto;
        }

        private async Task<string> GetDataForReservations(string showId)
        {
            var response = await _webClient.GetAsync($"{_uriSeatReservationService}api/v1/data_for_reservation_seats/{showId}");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}