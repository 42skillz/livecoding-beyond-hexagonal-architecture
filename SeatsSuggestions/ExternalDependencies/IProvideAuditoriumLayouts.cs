using System.Threading.Tasks;

namespace ExternalDependencies
{
    public interface IProvideAuditoriumLayouts
    {
        Task<AuditoriumDto> GetAuditoriumSeatingFor(string showId);
    }
}