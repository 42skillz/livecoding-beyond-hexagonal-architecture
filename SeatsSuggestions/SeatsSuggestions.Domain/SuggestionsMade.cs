using System;
using System.Collections.Generic;
using System.Linq;

namespace SeatsSuggestions.Domain
{
    /// <summary>
    ///     Occurs when a bunch of Suggestion are made.
    /// </summary>
    public class SuggestionsMade
    {
        public ShowId ShowId { get; }
        public PartyRequested PartyRequested { get; }

        private Dictionary<PricingCategory, List<SuggestionMade>> ForCategory { get; } = new Dictionary<PricingCategory, List<SuggestionMade>>();

        public IEnumerable<string> SeatsInFirstPricingCategory => SeatNames(PricingCategory.First);
        public IEnumerable<string> SeatsInSecondPricingCategory => SeatNames(PricingCategory.Second);
        public IEnumerable<string> SeatsInThirdPricingCategory => SeatNames(PricingCategory.Third);
        public IEnumerable<string> SeatsInMixedPricingCategory => SeatNames(PricingCategory.Mixed);

        public SuggestionsMade(ShowId showId, PartyRequested partyRequested)
        {
            ShowId = showId;
            PartyRequested = partyRequested;

            InstantiateAnEmptyListForEveryPricingCategory();
        }

        public IEnumerable<string> SeatNames(PricingCategory pricingCategory)
        {
            var suggestionsMade = ForCategory[pricingCategory];
            return suggestionsMade.Select(s => string.Join("-", s.SeatNames()));
        }

        private void InstantiateAnEmptyListForEveryPricingCategory()
        {
            foreach (PricingCategory pricingCategory in Enum.GetValues(typeof(PricingCategory)))
            {
                ForCategory[pricingCategory] = new List<SuggestionMade>();
            }
        }

        public void Add(IEnumerable<SuggestionMade> suggestions)
        {
            foreach (var suggestionMade in suggestions)
            {
                ForCategory[suggestionMade.PricingCategory].Add(suggestionMade);
            }
        }

        public bool MatchExpectations()
        {
            return ForCategory.SelectMany(s => s.Value).Any(x => x.MatchExpectation());
        }
    }
}