using System.Threading.Tasks;
using ExternalDependencies;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SeatsSuggestions.Infra.Helpers;

namespace SeatsSuggestions.Infra
{
    /// <summary>
    ///     Get via a web api and adapt auditorium and return <see cref="AuditoriumDto" />.
    /// </summary>
    public class AuditoriumWebClient : IProvideAuditoriumLayouts
    {
        private readonly string _uriAuditoriumSeatingRepository;
        private readonly IWebClient _webClient;

        public AuditoriumWebClient(string uriAuditoriumSeatingRepository, IWebClient webClient)
        {
            _uriAuditoriumSeatingRepository = uriAuditoriumSeatingRepository;
            _webClient = webClient;
        }

        public async Task<AuditoriumDto> GetAuditoriumSeatingFor(string showId)
        {
            var response = await _webClient.GetAsync($"{_uriAuditoriumSeatingRepository}api/v1/data_for_auditoriumSeating/{showId}");

            response.EnsureSuccessStatusCode();

            var jsonAuditoriumSeating = await response.Content.ReadAsStringAsync();

            var auditoriumSeatingDto = JsonConvert.DeserializeObject<AuditoriumDto>(jsonAuditoriumSeating, new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});

            return auditoriumSeatingDto;
        }
    }
}