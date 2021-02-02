using System.Collections.Generic;

namespace SeatsSuggestions.Domain
{
    public class SeatingOptionSuggested
    {
        public PricingCategory PricingCategory { get; }
        public List<Seat> Seats { get; } = new List<Seat>();
        public PartyRequested PartyRequested { get; }

        public SeatingOptionSuggested(SuggestionRequest suggestionRequest)
        {
            PartyRequested = suggestionRequest.PartyRequested;
            PricingCategory = suggestionRequest.PricingCategory;
        }

        public bool MatchExpectation()
        {
            return Seats.Count == PartyRequested.PartySize;
        }

        public void AddSeats(AdjacentSeats adjacentSeats)
        {
            Seats.AddRange(adjacentSeats);
        }
    }
}