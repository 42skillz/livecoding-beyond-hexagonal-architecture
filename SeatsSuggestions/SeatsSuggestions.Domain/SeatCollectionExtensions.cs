using System;
using System.Collections.Generic;
using System.Linq;

namespace SeatsSuggestions.Domain
{
    public static class SeatCollectionExtensions
    {
        public static IEnumerable<Seat> SelectAvailableSeatsCompliant(this IEnumerable<Seat> seats, PricingCategory pricingCategory)
        {
            return seats.Where(s => s.IsAvailable() && s.MatchCategory(pricingCategory));
        }

        public static IEnumerable<AdjacentSeats> SelectAdjacentSeats(this IEnumerable<Seat> seats, PartyRequested partyRequested)
        {
            var adjacentSeatsGroups = new List<AdjacentSeats>();
            var adjacentSeats = new List<Seat>();

            if (partyRequested.PartySize == 1)
            {
                return seats.Select(s => new AdjacentSeats(new List<Seat> {s}));
            }

            foreach (var candidateSeat in seats.OrderBy(s => s.DistanceFromRowCentroid))
            {
                if (!adjacentSeats.Any())
                {
                    adjacentSeats.Add(candidateSeat);
                    continue;
                }

                adjacentSeats = adjacentSeats.OrderBy(s => s.Number).ToList();

                if (DoesNotExceedPartyRequestedAndCandidateSeatIsAdjacent(candidateSeat, adjacentSeats, partyRequested))
                {
                    adjacentSeats.Add(candidateSeat);
                }
                else
                {
                    if (adjacentSeats.Any())
                    {
                        adjacentSeatsGroups.Add(new AdjacentSeats(adjacentSeats));
                    }

                    adjacentSeats = new List<Seat> {candidateSeat};
                }
            }

            return adjacentSeatsGroups.Where(adjacent => adjacent.Count() == partyRequested.PartySize);
        }

        private static bool DoesNotExceedPartyRequestedAndCandidateSeatIsAdjacent(Seat candidateSeat, List<Seat> adjacentSeats, PartyRequested partyRequested)
        {
            return candidateSeat.IsAdjacentWith(adjacentSeats) && adjacentSeats.Count < partyRequested.PartySize;
        }

        public static IEnumerable<AdjacentSeats> OrderByMiddleOfTheRow(this IEnumerable<AdjacentSeats> adjacentSeats, int rowSize)
        {
            var sortedAdjacentSeatsGroups = new SortedList<int, List<AdjacentSeats>>();

            foreach (var adjacentSeat in adjacentSeats)
            {
                var distanceFromRowCentroid = adjacentSeat.ComputeDistanceFromRowCentroid(rowSize);

                if (!sortedAdjacentSeatsGroups.ContainsKey(distanceFromRowCentroid))
                {
                    sortedAdjacentSeatsGroups.Add(distanceFromRowCentroid, new List<AdjacentSeats>());
                }

                sortedAdjacentSeatsGroups[distanceFromRowCentroid].Add(adjacentSeat);
            }

            return sortedAdjacentSeatsGroups.Values.SelectMany(_ => _);
        }

        internal static int CentroidIndex(this int rowSize)
        {
            return (int) Math.Ceiling((decimal) rowSize / 2);
        }

        internal static int ComputeDistanceFromCentroid(this uint seatLocation, int rowSize)
        {
            return (int) Math.Abs(seatLocation - rowSize.CentroidIndex());
        }

        internal static bool IsCentroid(this uint seatLocation, int rowSize)
        {
            var centroidIndex = rowSize.CentroidIndex();
            return seatLocation == centroidIndex;
        }

        internal static bool IsOdd(this int rowSize)
        {
            return rowSize % 2 != 0;
        }
    }
}