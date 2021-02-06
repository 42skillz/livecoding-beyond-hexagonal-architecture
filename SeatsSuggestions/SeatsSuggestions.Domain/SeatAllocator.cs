using System.Collections.Generic;
using System.Threading.Tasks;
using SeatsSuggestions.Domain.Helper;

namespace SeatsSuggestions.Domain
{
    /// <summary>
    ///     The functional and imperative shell/orchestration core.
    /// </summary>
    public static class SeatAllocator
    {
        private const int NumberOfSuggestionsPerPricingCategory = 3;

        public delegate Task<SuggestionsMade> SuggestionsDelegate(ShowId id, PartyRequested party);

        public async static Task<SuggestionsMade> MakeSuggestionsImperativeShell(Ports.IProvideUpToDateAuditoriumSeating auditoriumSeatingProvider, ShowId id, PartyRequested partyRequested)
        {
            // non-pure function
            var auditoriumSeating = await auditoriumSeatingProvider.GetAuditoriumSeating(id);

            // call pure function
            return SeatAllocator
                .TryMakeSuggestions(id, partyRequested, auditoriumSeating)
                    .GetValueOrFallback(new SuggestionNotAvailable(id, partyRequested));

            // Balance restored:
            // - inner hexagon knows about adapter capabilities but not implementation
            // - orchestration is back in the 'core' where we can locally reason about it
            
            // Notes: 
            // in this case the imperative shells can be easily distinguished by the 'async' keyword which kind of plays the role of the IO<> marker type. 
        }

        public static Maybe<SuggestionsMade> TryMakeSuggestions(ShowId showId, PartyRequested partyRequested, AuditoriumSeating auditoriumSeating)
        {
            var suggestionsMade = new SuggestionsMade(showId, partyRequested);

            suggestionsMade.Add(GiveMeSuggestionsFor(auditoriumSeating, partyRequested, PricingCategory.First));
            suggestionsMade.Add(GiveMeSuggestionsFor(auditoriumSeating, partyRequested, PricingCategory.Second));
            suggestionsMade.Add(GiveMeSuggestionsFor(auditoriumSeating, partyRequested, PricingCategory.Third));
            suggestionsMade.Add(GiveMeSuggestionsFor(auditoriumSeating, partyRequested, PricingCategory.Mixed));

            if (suggestionsMade.MatchExpectations())
            {
                return new Maybe<SuggestionsMade>(suggestionsMade);
            }

            return new Maybe<SuggestionsMade>();
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