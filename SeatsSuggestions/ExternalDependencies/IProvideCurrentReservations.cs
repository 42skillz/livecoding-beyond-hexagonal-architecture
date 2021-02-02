using System.Threading.Tasks;

namespace ExternalDependencies
{
    public interface IProvideCurrentReservations
    {
        Task<ReservedSeatsDto> GetReservedSeats(string showId);
    }
}