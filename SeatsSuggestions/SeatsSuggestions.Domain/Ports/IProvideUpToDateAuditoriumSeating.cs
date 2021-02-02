using System.Threading.Tasks;

namespace SeatsSuggestions.Domain.Ports
{
    /// <summary>
    ///     Right-side port allowing us to retrieve up-to-date AuditoriumSeating.
    /// </summary>
    public interface IProvideUpToDateAuditoriumSeating
    {
        Task<AuditoriumSeating> GetAuditoriumSeating(ShowId showId);
    }
}