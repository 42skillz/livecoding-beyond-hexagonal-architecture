using System.Collections.Generic;
using System.Linq;
using NFluent;
using NUnit.Framework;
using SeatsSuggestions.Domain;

namespace SeatsSuggestions.Tests.UnitTests
{
    [TestFixture]
    public class SeatCollectionExtensionsShould
    {
        private static List<Seat> ComputeDistances(IEnumerable<Seat> seats, int rowSize)
        {
            return seats.Select(s => new Seat(s.RowName, s.Number, s.PricingCategory, s.SeatAvailability, s.ComputeDistanceFromRowCentroid(rowSize))).ToList();
        }

        [Test]
        public void Compute_distance_for_each_seat_when_row_size_is_even()
        {
            var seats = new List<Seat>
            {
                new Seat("A", 1, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 2, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 3, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 4, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 5, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 6, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 7, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 8, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 9, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 10, PricingCategory.First, SeatAvailability.Available)
            };

            seats = ComputeDistances(seats, seats.Count);

            Check.That(seats[0].DistanceFromRowCentroid).IsEqualTo(4); // A1
            Check.That(seats[1].DistanceFromRowCentroid).IsEqualTo(3); // A2
            Check.That(seats[2].DistanceFromRowCentroid).IsEqualTo(2); // A3
            Check.That(seats[3].DistanceFromRowCentroid).IsEqualTo(1); // A4
            Check.That(seats[4].DistanceFromRowCentroid).IsEqualTo(0); // A5
            Check.That(seats[5].DistanceFromRowCentroid).IsEqualTo(0); // A6
            Check.That(seats[6].DistanceFromRowCentroid).IsEqualTo(1); // A7
            Check.That(seats[7].DistanceFromRowCentroid).IsEqualTo(2); // A8
            Check.That(seats[8].DistanceFromRowCentroid).IsEqualTo(3); // A9
            Check.That(seats[9].DistanceFromRowCentroid).IsEqualTo(4); // A10
        }

        [Test]
        public void Compute_distance_for_each_seat_when_row_size_is_odd()
        {
            var seats = new List<Seat>
            {
                new Seat("A", 1, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 2, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 3, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 4, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 5, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 6, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 7, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 8, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 9, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 10, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 11, PricingCategory.First, SeatAvailability.Available)
            };

            seats = ComputeDistances(seats, seats.Count);

            Check.That(seats[0].DistanceFromRowCentroid).IsEqualTo(5); // A1
            Check.That(seats[1].DistanceFromRowCentroid).IsEqualTo(4); // A2
            Check.That(seats[2].DistanceFromRowCentroid).IsEqualTo(3); // A3
            Check.That(seats[3].DistanceFromRowCentroid).IsEqualTo(2); // A4
            Check.That(seats[4].DistanceFromRowCentroid).IsEqualTo(1); // A5
            Check.That(seats[5].DistanceFromRowCentroid).IsEqualTo(0); // A6
            Check.That(seats[6].DistanceFromRowCentroid).IsEqualTo(1); // A7
            Check.That(seats[7].DistanceFromRowCentroid).IsEqualTo(2); // A8
            Check.That(seats[8].DistanceFromRowCentroid).IsEqualTo(3); // A9
            Check.That(seats[9].DistanceFromRowCentroid).IsEqualTo(4); // A10
            Check.That(seats[10].DistanceFromRowCentroid).IsEqualTo(5); // A11
        }

        [Test]
        public void Order_by_middle_of_the_Row()
        {
            var seats = new List<Seat>
            {
                new Seat("A", 1, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 2, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 3, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 4, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 5, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 6, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 7, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 8, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 9, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 10, PricingCategory.First, SeatAvailability.Available)
            };

            seats = ComputeDistances(seats, seats.Count);

            var adjacentSeats = seats.SelectAdjacentSeats(new PartyRequested(4)).ToList();
            var orderByMiddleOfTheRow = adjacentSeats.OrderByMiddleOfTheRow(10);
            Check.That(orderByMiddleOfTheRow.Select(a => a.ToString())).ContainsExactly("A4-A5-A6-A7");
        }

        [Test]
        public void Order_by_middle_of_the_Row_when_every_seat_is_available()
        {
            var seats = new List<Seat>
            {
                new Seat("A", 1, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 2, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 3, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 4, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 5, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 6, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 7, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 8, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 9, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 10, PricingCategory.Second, SeatAvailability.Available)
            };

            seats = ComputeDistances(seats, seats.Count);

            var adjacentSeats = seats.SelectAdjacentSeats(new PartyRequested(1));
            var orderByMiddleOfTheRow = adjacentSeats.OrderByMiddleOfTheRow(10);
            Check.That(orderByMiddleOfTheRow.Select(a => a.ToString())).ContainsExactly("A5", "A6", "A4", "A7", "A3", "A8", "A2", "A9", "A1", "A10");
        }

        [Test]
        public void Return_a_few_adjacent_seats_when_SelectAdjacentSeats_is_called()
        {
            var seats = new List<Seat>
            {
                new Seat("A", 1, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 3, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 4, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 5, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 6, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 8, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 9, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 10, PricingCategory.First, SeatAvailability.Available)
            };
            seats = ComputeDistances(seats, 10);
            var adjacentSeats = seats.SelectAdjacentSeats(new PartyRequested(2));
            Check.That(adjacentSeats.Select(s => s.ToString())).ContainsExactly("A5-A6", "A3-A4", "A8-A9");
        }

        [Test]
        public void Return_empty_when_SelectAdjacentSeats_is_called_and_no_adjacentSeats_are_available()
        {
            var seats = new List<Seat>
            {
                new Seat("A", 1, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 3, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 5, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 7, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 9, PricingCategory.First, SeatAvailability.Available)
            };

            var adjacentSeats = seats.SelectAdjacentSeats(new PartyRequested(2));
            Check.That(adjacentSeats).IsEmpty();
        }

        [Test]
        public void SelectAvailableSeatsCompliantWith_when_all_are_First_and_requesting_First()
        {
            var seats = new List<Seat>
            {
                new Seat("A", 1, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 2, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 3, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 4, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 5, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 6, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 7, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 8, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 9, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 10, PricingCategory.First, SeatAvailability.Available)
            };

            var seatsCompliant = seats.SelectAvailableSeatsCompliant(PricingCategory.First);
            Check.That(seatsCompliant.Select(s => s.ToString())).ContainsExactly("A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "A10");
        }


        [Test]
        public void SelectAvailableSeatsCompliantWith_when_some_are_second_and_requesting_second()
        {
            var seats = new List<Seat>
            {
                new Seat("A", 1, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 2, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 3, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 4, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 5, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 6, PricingCategory.Second, SeatAvailability.Available),
                new Seat("A", 7, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 8, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 9, PricingCategory.First, SeatAvailability.Available),
                new Seat("A", 10, PricingCategory.First, SeatAvailability.Available)
            };

            var seatsCompliant = seats.SelectAvailableSeatsCompliant(PricingCategory.Second);
            Check.That(seatsCompliant.Select(s => s.ToString())).ContainsExactly("A3", "A6");
        }
    }
}