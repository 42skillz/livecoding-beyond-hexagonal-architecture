namespace SeatsSuggestions.Domain
{
    internal class SeatingOptionNotAvailable : SeatingOptionSuggested
    {
        public SeatingOptionNotAvailable(SuggestionRequest suggestionRequest) : base(suggestionRequest)
        {
        }
    }
}