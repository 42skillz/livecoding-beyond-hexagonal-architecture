using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeatsSuggestions.Domain;
using SeatsSuggestions.Domain.Ports;

#pragma warning disable 1591

namespace SeatsSuggestions.Api.Controllers
{
    /// <summary>
    ///     Web controller acting as a left-side Adapter of a Hexagonal Architecture.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SeatsSuggestionsController : ControllerBase
    {
        private readonly IProvideUpToDateAuditoriumSeating _auditoriumSeatingProvider;

        public SeatsSuggestionsController(IProvideUpToDateAuditoriumSeating auditoriumSeatingProvider)
        {
            _auditoriumSeatingProvider = auditoriumSeatingProvider;
        }

        // GET api/SeatsSuggestions?showId=5&party=3
        [HttpGet]
        public async Task<IActionResult> GetSuggestionsFor([FromQuery(Name = "showId")] string showId, [FromQuery(Name = "party")] int party)
        {
            // Imperative shell for this use case -------------------------------

            // Infra => Domain
            var id = new ShowId(showId);
            var partyRequested = new PartyRequested(party);

            // non-pure function
            var auditoriumSeating = await _auditoriumSeatingProvider.GetAuditoriumSeating(id);

            // pure function (the core)
            var suggestions = SeatAllocator
                .TryMakeSuggestions(id, partyRequested, auditoriumSeating)
                    .GetValueOrFallback(new SuggestionNotAvailable(id, partyRequested));

            // Domain => Infra
            return new OkObjectResult(suggestions /*JsonConvert.SerializeObject(suggestions, Formatting.Indented)*/);
        }
    }
}