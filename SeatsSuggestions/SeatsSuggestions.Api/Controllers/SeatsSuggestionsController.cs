using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeatsSuggestions.Domain;
using SeatsSuggestions.Domain.Helper;
using SeatsSuggestions.Domain.Ports;

#pragma warning disable 1591

namespace SeatsSuggestions.Api.Controllers
{
    /// <summary>
    ///     Web controller acting as the Imperative Shell for our Suggestions use case.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SeatsSuggestionsController : ControllerBase
    {
        private readonly Func<ShowId, PartyRequested, Task<SuggestionsMade>> _suggestionsDelegate;

        public SeatsSuggestionsController(Func<ShowId, PartyRequested, Task<SuggestionsMade>> suggestionDelegate)
        {
            _suggestionsDelegate = suggestionDelegate;
        }

        // GET api/SeatsSuggestions?showId=5&party=3
        [HttpGet]
        public async Task<IActionResult> GetSuggestionsFor([FromQuery(Name = "showId")] string showId, [FromQuery(Name = "party")] int party)
        {
            // Imperative shell for this use case -------------------------------

            // Infra => Domain
            var id = new ShowId(showId);
            var partyRequested = new PartyRequested(party);

            // Call curried imperative shell
            // Balance restored : Adapter no longer needs to know other adapters, only uses the domain core
            var suggestions = await _suggestionsDelegate(id, partyRequested);

            // Domain => Infra
            return new OkObjectResult(suggestions /*JsonConvert.SerializeObject(suggestions, Formatting.Indented)*/);
        }
    }
}