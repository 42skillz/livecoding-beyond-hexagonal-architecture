using System.Collections.Generic;
using System.Linq;
using Value;

namespace SeatsSuggestions.Domain
{
    public class Seat : ValueType<Seat>
    {
        public string RowName { get; }
        public uint Number { get; }
        public PricingCategory PricingCategory { get; }
        public SeatAvailability SeatAvailability { get; }
        public int DistanceFromRowCentroid { get; }


        public Seat(string rowName, uint number, PricingCategory pricingCategory, SeatAvailability seatAvailability, int distanceFromRowCentroid = 0)
        {
            RowName = rowName;
            Number = number;
            PricingCategory = pricingCategory;
            SeatAvailability = seatAvailability;
            DistanceFromRowCentroid = distanceFromRowCentroid;
        }

        public bool IsAvailable()
        {
            return SeatAvailability == SeatAvailability.Available;
        }

        public bool MatchCategory(PricingCategory pricingCategory)
        {
            if (pricingCategory == PricingCategory.Mixed)
            {
                return true;
            }

            return PricingCategory == pricingCategory;
        }

        public Seat Allocate()
        {
            if (SeatAvailability == SeatAvailability.Available)
            {
                return new Seat(RowName, Number, PricingCategory, SeatAvailability.Allocated);
            }

            return this;
        }

        public bool SameSeatLocation(Seat seat)
        {
            return RowName == seat.RowName && Number == seat.Number;
        }

        public int ComputeDistanceFromRowCentroid(int rowSize)
        {
            var seatLocation = Number;

            if (rowSize.IsOdd())
            {
                return seatLocation.ComputeDistanceFromCentroid(rowSize);
            }

            if (seatLocation.IsCentroid(rowSize))
            {
                return 0;
            }

            if (seatLocation < rowSize.CentroidIndex())
            {
                return seatLocation.ComputeDistanceFromCentroid(rowSize);
            }

            return seatLocation.ComputeDistanceFromCentroid(rowSize) - 1;
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] {RowName, Number, PricingCategory, SeatAvailability, DistanceFromRowCentroid};
        }

        public override string ToString()
        {
            return $"{RowName}{Number}";
        }

        public bool IsAdjacentWith(List<Seat> seats)
        {
            var orderedSeats = seats.OrderBy(s => s.Number).ToList();

            var seat = orderedSeats.First();

            if (Number + 1 == seat.Number || Number - 1 == seat.Number)
            {
                return true;
            }

            seat = seats.Last();

            return Number + 1 == seat.Number || Number - 1 == seat.Number;
        }
    }
}