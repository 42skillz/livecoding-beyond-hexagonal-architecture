using System.Collections.Generic;
using Value;

namespace SeatsSuggestions.Domain
{
    public class SuggestionRequest : ValueType<SuggestionRequest>
    {
        public PartyRequested PartyRequested { get; }
        public PricingCategory PricingCategory { get; }

        public SuggestionRequest(PartyRequested partyRequested, PricingCategory pricingCategory)
        {
            PartyRequested = partyRequested;
            PricingCategory = pricingCategory;
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] {PartyRequested, PricingCategory};
        }

        public override string ToString()
        {
            return $"{PartyRequested}-{PricingCategory.ToString()}";
        }
    }
}