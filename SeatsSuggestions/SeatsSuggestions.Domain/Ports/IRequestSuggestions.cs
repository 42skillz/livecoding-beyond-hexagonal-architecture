using System.Threading.Tasks;

namespace SeatsSuggestions.Domain.Ports
{
    /// <summary>
    ///     Left-side port in order to ask for a bunch of Seats Suggestions.
    /// </summary>
    public interface IRequestSuggestions
    {
        SuggestionsMade MakeSuggestions(ShowId showId, PartyRequested partyRequested, AuditoriumSeating auditoriumSeating);
    }
}