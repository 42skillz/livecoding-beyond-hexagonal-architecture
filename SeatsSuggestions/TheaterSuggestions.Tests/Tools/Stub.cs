using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using NSubstitute;
using SeatsSuggestions.Infra.Helpers;

namespace SeatsSuggestions.Tests.Tools
{
    public static class Stub
    {
        public static IWebClient AWebClientWith(string showId, string layoutTheaterJsonFileName, string bookedSeatsJsonFileName)
        {
            var webClient = Substitute.For<IWebClient>();
            var jsonDirPath = Path.Combine(GetExecutingAssemblyDirectoryFullPath(), "AuditoriumLayouts");
            webClient.GetAsync(Arg.Is<string>(s => s.EndsWith($"/api/v1/data_for_auditoriumSeating/{showId}")))
                .Returns(new HttpResponseMessage {StatusCode = HttpStatusCode.OK, Content = new StringContent(File.ReadAllText($"{Path.Combine(jsonDirPath, layoutTheaterJsonFileName)}"))});

            webClient.GetAsync(Arg.Is<string>(s => s.EndsWith($"api/v1/data_for_reservation_seats/{showId}")))
                .Returns(new HttpResponseMessage {StatusCode = HttpStatusCode.OK, Content = new StringContent(File.ReadAllText($"{Path.Combine(jsonDirPath, bookedSeatsJsonFileName)}"))});

            return webClient;
        }

        private static string GetExecutingAssemblyDirectoryFullPath()
        {
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);

            if (directoryName.StartsWith(@"file:\"))
            {
                directoryName = directoryName.Substring(6);
            }

            if (directoryName.StartsWith(@"file:/"))
            {
                directoryName = directoryName.Substring(5);
            }

            return directoryName;
        }
    }
}