using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SeatsSuggestions.Domain
{
    public class AdjacentSeats : IEnumerable<Seat>
    {
        private readonly List<Seat> _seats;

        public IEnumerator<Seat> GetEnumerator()
        {
            return _seats.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public AdjacentSeats(IEnumerable<Seat> seats)
        {
            _seats = seats.ToList();
        }

        public int ComputeDistanceFromRowCentroid(int rowSize)
        {
            return _seats.Select(s => s.DistanceFromRowCentroid).ToList().Sum() / _seats.Count;
        }

        public override string ToString()
        {
            return $"{string.Join("-", _seats.Select(s => s.ToString()))}";
        }
    }
}