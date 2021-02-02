using System.Linq;
using System.Threading.Tasks;
using ExternalDependencies.AuditoriumLayoutRepository;
using ExternalDependencies.ReservationsProvider;
using NFluent;
using NUnit.Framework;
using SeatsSuggestions.Domain;
using SeatsSuggestions.Infra.Adapter;

namespace SeatsSuggestions.Tests.UnitTests
{
    [TestFixture]
    public class AuditoriumSeatingShould
    {
        [Test]
        public async Task Be_a_Value_Type()
        {
            var auditoriumLayoutAdapter = new AuditoriumSeatingAdapter(new AuditoriumLayoutRepository(), new ReservationsProvider());
            var showIdWithoutReservationYet = new ShowId("18");
            var auditoriumSeatingFirstInstance = await auditoriumLayoutAdapter.GetAuditoriumSeating(showIdWithoutReservationYet);
            var auditoriumSeatingSecondInstance = await auditoriumLayoutAdapter.GetAuditoriumSeating(showIdWithoutReservationYet);

            // Two different instances with same values should be equals
            Check.That(auditoriumSeatingSecondInstance).IsEqualTo(auditoriumSeatingFirstInstance);

            // Should not mutate existing instance 
            auditoriumSeatingSecondInstance.Rows.Values.First().Seats.First().Allocate();
            Check.That(auditoriumSeatingSecondInstance).IsEqualTo(auditoriumSeatingFirstInstance);
        }
    }
}