using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SeatsSuggestions.Domain;
using SeatsSuggestions.Domain.Ports;
#pragma warning disable 1591

namespace SeatsSuggestions.Api.Controllers
{
    /// <summary>
    /// Web controller acting as a left-side Adapter of a Hexagonal Architecture.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SeatsSuggestionsController : ControllerBase
    {
        private readonly IRequestSuggestions _hexagon;

        public SeatsSuggestionsController(IRequestSuggestions hexagon)
        {
            _hexagon = hexagon;
        }

        // GET api/SeatsSuggestions?showId=5&party=3
        [HttpGet]
        public async Task<IActionResult> GetSuggestionsFor([FromQuery(Name = "showId")] string showId, [FromQuery(Name = "party")] int party)
        {           
            // Infra => Domain
            var id = new ShowId(showId);
            var partyRequested = new PartyRequested(party);

            // Call the Domain
            var suggestions = await _hexagon.MakeSuggestions(id, partyRequested);

            // Domain => Infra
            return new OkObjectResult(suggestions/*JsonConvert.SerializeObject(suggestions, Formatting.Indented)*/);
        }
    }
}