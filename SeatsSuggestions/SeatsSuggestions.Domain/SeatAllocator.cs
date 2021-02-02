using System.Collections.Generic;
using System.Threading.Tasks;
using SeatsSuggestions.Domain.Ports;

namespace SeatsSuggestions.Domain
{
    /// <summary>
    ///     The Hexagon.
    /// </summary>
    public class SeatAllocator : IRequestSuggestions
    {
        private const int NumberOfSuggestionsPerPricingCategory = 3;
        private readonly IProvideUpToDateAuditoriumSeating _auditoriumSeatingProvider;

        public async Task<SuggestionsMade> MakeSuggestions(ShowId showId, PartyRequested partyRequested)
        {
            var auditoriumSeating = await _auditoriumSeatingProvider.GetAuditoriumSeating(showId);

            var suggestionsMade = new SuggestionsMade(showId, partyRequested);

            suggestionsMade.Add(GiveMeSuggestionsFor(auditoriumSeating, partyRequested, PricingCategory.First));
            suggestionsMade.Add(GiveMeSuggestionsFor(auditoriumSeating, partyRequested, PricingCategory.Second));
            suggestionsMade.Add(GiveMeSuggestionsFor(auditoriumSeating, partyRequested, PricingCategory.Third));
            suggestionsMade.Add(GiveMeSuggestionsFor(auditoriumSeating, partyRequested, PricingCategory.Mixed));

            if (suggestionsMade.MatchExpectations())
            {
                return suggestionsMade;
            }

            return new SuggestionNotAvailable(showId, partyRequested);
        }

        public SeatAllocator(IProvideUpToDateAuditoriumSeating auditoriumSeatingProvider)
        {
            _auditoriumSeatingProvider = auditoriumSeatingProvider;
        }

        private static IEnumerable<SuggestionMade> GiveMeSuggestionsFor(AuditoriumSeating auditoriumSeating, PartyRequested partyRequested, PricingCategory pricingCategory)
        {
            var foundedSuggestions = new List<SuggestionMade>();

            for (var i = 0; i < NumberOfSuggestionsPerPricingCategory; i++)
            {
                var seatOptionsSuggested = auditoriumSeating.SuggestSeatingOptionFor(new SuggestionRequest(partyRequested, pricingCategory));

                if (seatOptionsSuggested.MatchExpectation())
                {
                    // We get the new version of the Auditorium after the allocation
                    auditoriumSeating = auditoriumSeating.Allocate(seatOptionsSuggested);

                    foundedSuggestions.Add(new SuggestionMade(partyRequested, pricingCategory, seatOptionsSuggested.Seats));
                }
            }

            return foundedSuggestions;
        }
    }
}