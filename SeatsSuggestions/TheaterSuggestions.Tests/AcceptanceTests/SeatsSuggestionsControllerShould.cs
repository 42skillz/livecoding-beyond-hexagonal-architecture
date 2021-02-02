using System.Threading.Tasks;
using NFluent;
using NUnit.Framework;
using SeatsSuggestions.Domain;
using SeatsSuggestions.Tests.Tools;

// ReSharper disable InvalidXmlDocComment

namespace SeatsSuggestions.Tests.AcceptanceTests
{
    /// <summary>
    ///     Coarse-grained Acceptance tests involving the whole Hexagon + all the Adapters
    ///     (left and right-side) but not the I/O.
    /// </summary>
    [TestFixture]
    public class SeatsSuggestionsControllerShould
    {
        [Test]
        public async Task Suggest_one_seat_when_Auditorium_contains_one_available_seat_only()
        {
            /// <image url="..\Images\Auditorium-1.JPG" scale="0.5" />
            const string showId = "1";
            const int partyRequested = 1;

            var leftSideAdapter = new HexagonBuilder()
                .WithAuditoriumDefinedForShow(showId, "1-Ford Theater-(2)(0)_theater.json", "1-Ford Theater-(2)(0)_booked_seats.json")
                .BuildHexagonWithAdaptersButWithoutIOs();

            // ACT
            var response = await leftSideAdapter.GetSuggestionsFor(showId, partyRequested);

            var suggestionsMade = response.ExtractValue<SuggestionsMade>();
            Check.That(suggestionsMade.SeatsInFirstPricingCategory).ContainsExactly("A3");
            Check.That(suggestionsMade.SeatsInSecondPricingCategory).IsEmpty();
            Check.That(suggestionsMade.SeatsInThirdPricingCategory).IsEmpty();
        }

        [Test]
        public async Task Return_SeatsNotAvailable_when_Auditorium_has_all_its_seats_already_reserved()
        {
            /// <image url="..\Images\Auditorium-5.JPG" scale="0.7" />
            const string showId = "5";
            const int partyRequested = 1;

            var leftSideAdapter = new HexagonBuilder()
                .WithAuditoriumDefinedForShow(showId, "5-Madison Theater-(2)(0)_theater.json", "5-Madison Theater-(2)(0)_booked_seats.json")
                .BuildHexagonWithAdaptersButWithoutIOs();

            // ACT
            var response = await leftSideAdapter.GetSuggestionsFor(showId, partyRequested);

            var suggestionsMade = response.ExtractValue<SuggestionsMade>();
            Check.That(suggestionsMade.PartyRequested.PartySize).IsEqualTo(partyRequested);
            Check.That(suggestionsMade.ShowId.Id).IsEqualTo(showId);

            Check.That(suggestionsMade).IsInstanceOf<SuggestionNotAvailable>();
            Check.That(suggestionsMade.SeatsInFirstPricingCategory).IsEmpty();
            Check.That(suggestionsMade.SeatsInSecondPricingCategory).IsEmpty();
            Check.That(suggestionsMade.SeatsInThirdPricingCategory).IsEmpty();
            Check.That(suggestionsMade.SeatsInMixedPricingCategory).IsEmpty();
        }

        [Test]
        public async Task Offer_several_suggestions_ie_1_per_PricingCategory_and_other_one_without_category_affinity()
        {
            /// <image url="..\Images\Auditorium-18.JPG" scale="0.5" />
            const string showId = "18";
            const int partyRequested = 1;

            var leftSideAdapter = new HexagonBuilder()
                .WithAuditoriumDefinedForShow(showId, "18-New Amsterdam-(6)(0)_theater.json", "18-New Amsterdam-(6)(0)_booked_seats.json")
                .BuildHexagonWithAdaptersButWithoutIOs();

            // ACT
            var response = await leftSideAdapter.GetSuggestionsFor(showId, partyRequested);

            var suggestionsMade = response.ExtractValue<SuggestionsMade>();
            Check.That(suggestionsMade.SeatsInFirstPricingCategory).ContainsExactly("A5", "A6", "A4");
            Check.That(suggestionsMade.SeatsInSecondPricingCategory).ContainsExactly("A2", "A9", "A1");
            Check.That(suggestionsMade.SeatsInThirdPricingCategory).ContainsExactly("E5", "E6", "E4");
            Check.That(suggestionsMade.SeatsInMixedPricingCategory).ContainsExactly("A5", "A6", "A4");
        }

        [Test]
        public async Task Offer_adjacent_seats_nearer_the_middle_of_a_row()
        {
            /// <image url="..\Images\Auditorium-9.JPG" scale="0.5" />
            const string showId = "9";
            const int partyRequested = 1;

            var leftSideAdapter = new HexagonBuilder()
                .WithAuditoriumDefinedForShow(showId, "9-Mogador Theater-(2)(0)_theater.json", "9-Mogador Theater-(2)(0)_booked_seats.json")
                .BuildHexagonWithAdaptersButWithoutIOs();

            // ACT
            var response = await leftSideAdapter.GetSuggestionsFor(showId, partyRequested);

            var suggestionsMade = response.ExtractValue<SuggestionsMade>();
            Check.That(suggestionsMade.SeatsInFirstPricingCategory).ContainsExactly("A4", "A3", "B5");
        }

        [Test]
        public async Task Offer_adjacent_seats_nearer_the_middle_of_a_row_when_it_is_possible()
        {
            /// <image url="..\Images\Auditorium-3.JPG" scale="0.5" />
            const string showId = "3";
            const int partyRequested = 4;

            var leftSideAdapter = new HexagonBuilder()
                .WithAuditoriumDefinedForShow(showId, "3-Dock Street Theater-(6)(0)_theater.json", "3-Dock Street Theater-(6)(0)_booked_seats.json")
                .BuildHexagonWithAdaptersButWithoutIOs();

            // ACT
            var response = await leftSideAdapter.GetSuggestionsFor(showId, partyRequested);

            var suggestionsMade = response.ExtractValue<SuggestionsMade>();
            Check.That(suggestionsMade.SeatsInFirstPricingCategory).IsEmpty();
            Check.That(suggestionsMade.SeatsInSecondPricingCategory).ContainsExactly("C4-C5-C6-C7", "D4-D5-D6-D7");
            Check.That(suggestionsMade.SeatsInThirdPricingCategory).ContainsExactly("E4-E5-E6-E7", "F4-F5-F6-F7");
            Check.That(suggestionsMade.SeatsInMixedPricingCategory).ContainsExactly("A6-A7-A8-A9", "C4-C5-C6-C7", "D4-D5-D6-D7");
        }
    }
}