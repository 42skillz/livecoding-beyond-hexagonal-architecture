using System.Threading.Tasks;
using ExternalDependencies.AuditoriumLayoutRepository;
using ExternalDependencies.ReservationsProvider;
using NFluent;
using NUnit.Framework;

namespace SeatsSuggestions.Tests.IntegrationTests
{
    /// <summary>
    ///     Tests suite for the External dependencies/services.
    /// </summary>
    [TestFixture]
    public class ExternalDependenciesShould
    {
        [Test]
        public async Task Allow_us_to_retrieve_AuditoriumLayout_for_a_given_ShowId()
        {
            var auditoriumLayoutRepository = new AuditoriumLayoutRepository();
            var auditoriumDto = await auditoriumLayoutRepository.GetAuditoriumSeatingFor("2");

            Check.That(auditoriumDto.Rows).HasSize(6);
            Check.That(auditoriumDto.Corridors).HasSize(2);
            var firstSeatOfFirstRow = auditoriumDto.Rows["A"][0];
            Check.That(firstSeatOfFirstRow.Category).IsEqualTo(2);
        }

        [Test]
        public async Task Allow_us_to_retrieve_reserved_seats_for_a_given_ShowId()
        {
            var seatsRepository = new ReservationsProvider();
            var reservedSeatsDto = await seatsRepository.GetReservedSeats("1");

            Check.That(reservedSeatsDto.ReservedSeats).HasSize(19);
        }
    }
}