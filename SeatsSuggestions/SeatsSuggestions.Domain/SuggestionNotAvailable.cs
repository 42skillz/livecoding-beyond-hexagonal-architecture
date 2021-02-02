namespace SeatsSuggestions.Domain
{
    /// <summary>
    ///     Occurs when a Suggestion that does not meet expectation is made.
    /// </summary>
    public class SuggestionNotAvailable : SuggestionsMade
    {
        public SuggestionNotAvailable(ShowId showId, PartyRequested partyRequested) : base(showId, partyRequested)
        {
        }
    }
}