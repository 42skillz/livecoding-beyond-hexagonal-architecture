using System.Collections.Generic;
using NFluent;
using NUnit.Framework;
using SeatsSuggestions.Domain;

namespace SeatsSuggestions.Tests.UnitTests
{
    [TestFixture]
    public class RowShould
    {
        [Test]
        public void Be_a_Value_Type()
        {
            var A1 = new Seat("A", 1, PricingCategory.Second, SeatAvailability.Available);
            var A2 = new Seat("A", 2, PricingCategory.Second, SeatAvailability.Available);

            // Two different instances with same values should be equals
            var rowFirstInstance = new Row("A", new List<Seat> {A1, A2});
            var rowSecondInstance = new Row("A", new List<Seat> {A1, A2});
            Check.That(rowSecondInstance).IsEqualTo(rowFirstInstance);

            // Should not mutate existing instance 
            var A3 = new Seat("A", 2, PricingCategory.Second, SeatAvailability.Available);
            rowSecondInstance.AddSeat(A3);
            Check.That(rowSecondInstance).IsEqualTo(rowFirstInstance);
        }
    }
}