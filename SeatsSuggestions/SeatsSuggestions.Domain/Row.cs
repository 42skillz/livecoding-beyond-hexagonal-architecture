using System.Collections.Generic;
using System.Linq;
using Value;

namespace SeatsSuggestions.Domain
{
    public class Row : ValueType<Row>
    {
        public string Name { get; }
        public IEnumerable<Seat> Seats { get; }

        public Row(string name, IReadOnlyCollection<Seat> seats)
        {
            Name = name;
            Seats = seats.Select(s => new Seat(s.RowName, s.Number, s.PricingCategory, s.SeatAvailability,
                s.ComputeDistanceFromRowCentroid(seats.Count())));
        }

        public Row AddSeat(Seat seat)
        {
            var seatsUpdated = Seats.Select(s => s == seat ? seat : s).ToList();

            return new Row(Name, seatsUpdated);
        }

        public SeatingOptionSuggested SuggestSeatingOption(SuggestionRequest suggestionRequest)
        {
            var seatingOptionSuggested = new SeatingOptionSuggested(suggestionRequest);
            var availableSeatsCompliant = Seats.SelectAvailableSeatsCompliant(suggestionRequest.PricingCategory);
            var rowSize = Seats.Count();

            var adjacentSeatsOfExpectedSize = availableSeatsCompliant.SelectAdjacentSeats(suggestionRequest.PartyRequested);

            var adjacentSeatsOrdered = adjacentSeatsOfExpectedSize.OrderByMiddleOfTheRow(rowSize);

            foreach (var adjacentSeats in adjacentSeatsOrdered)
            {
                seatingOptionSuggested.AddSeats(adjacentSeats);

                if (seatingOptionSuggested.MatchExpectation())
                {
                    return seatingOptionSuggested;
                }
            }

            return new SeatingOptionNotAvailable(suggestionRequest);
        }

        public Row Allocate(Seat seat)
        {
            var newVersionOfSeats = new List<Seat>();

            foreach (var currentSeat in Seats)
            {
                if (currentSeat.SameSeatLocation(seat))
                {
                    newVersionOfSeats.Add(new Seat(seat.RowName, seat.Number, seat.PricingCategory, SeatAvailability.Allocated));
                }
                else
                {
                    newVersionOfSeats.Add(currentSeat);
                }
            }

            return new Row(seat.RowName, newVersionOfSeats);
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] {Name, new ListByValue<Seat>(new List<Seat>(Seats))};
        }
    }
}