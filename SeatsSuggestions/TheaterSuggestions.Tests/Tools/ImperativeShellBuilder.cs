using ExternalDependencies;
using SeatsSuggestions.Api.Controllers;
using SeatsSuggestions.Infra;
using SeatsSuggestions.Infra.Adapter;

namespace SeatsSuggestions.Tests.Tools
{
    /// <summary>
    ///     Build Hexagons with their real adapters (stubs only for the last miles I/Os)
    /// </summary>
    public class ImperativeShellBuilder
    {
        private string _showId;
        private string _theaterBookedSeatsJson;
        private string _theaterJson;

        public ImperativeShellBuilder WithAuditoriumDefinedForShow(string showId, string theaterJson, string theaterBookedSeatsJson)
        {
            _showId = showId;
            _theaterJson = theaterJson;
            _theaterBookedSeatsJson = theaterBookedSeatsJson;

            return this;
        }

        /// <summary>
        ///     Build and return a full hexagon + its adapters, stubbing only the last-miles HTTP calls.
        /// </summary>
        /// <returns></returns>
        public SeatsSuggestionsController BuildImperativeShellWithAdaptersButWithoutIOs()
        {
            // Instantiate the Right-side adapter
            var rightSideAdapter = InstantiateRightSideAdapter(_showId, _theaterJson, _theaterBookedSeatsJson);

            // Instantiate the Left-side adapter
            var leftSideAdapter = new SeatsSuggestionsController(rightSideAdapter);

            return leftSideAdapter;
        }

        private static AuditoriumSeatingAdapter InstantiateRightSideAdapter(string showId, string theaterJson, string theaterBookedSeatsJson)
        {
            var webClient = Stub.AWebClientWith(showId, theaterJson, theaterBookedSeatsJson);

            // But first its dependencies
            IProvideAuditoriumLayouts auditoriumLayoutProviderWebClient = new AuditoriumWebClient("http://fakehost:50950/", webClient);
            IProvideCurrentReservations seatsReservationsProviderWebClient = new SeatReservationsWebClient("http://fakehost:50951/", webClient);

            // Now the Adapter for the right-side port: IProvideUpToDateAuditoriumSeating
            var rightSideAdapter = new AuditoriumSeatingAdapter(auditoriumLayoutProviderWebClient, seatsReservationsProviderWebClient);
            return rightSideAdapter;
        }
    }
}