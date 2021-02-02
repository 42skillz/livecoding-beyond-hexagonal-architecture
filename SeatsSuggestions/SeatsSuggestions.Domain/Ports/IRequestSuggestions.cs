using System.Threading.Tasks;

namespace SeatsSuggestions.Domain.Ports
{
    /// <summary>
    ///     Left-side port in order to ask for a bunch of Seats Suggestions.
    /// </summary>
    public interface IRequestSuggestions
    {
        Task<SuggestionsMade> MakeSuggestions(ShowId showId, PartyRequested partyRequested);
    }
}