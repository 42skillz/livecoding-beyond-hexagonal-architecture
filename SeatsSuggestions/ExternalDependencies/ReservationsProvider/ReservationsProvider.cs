using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ExternalDependencies.ReservationsProvider
{
    public class ReservationsProvider : IProvideCurrentReservations
    {
        private readonly Dictionary<string, ReservedSeatsDto> _repository = new Dictionary<string, ReservedSeatsDto>();

        public ReservationsProvider()
        {
            var directoryName = $"{GetExecutingAssemblyDirectoryFullPath()}\\AuditoriumLayouts\\";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                directoryName = $"{GetExecutingAssemblyDirectoryFullPath()}/AuditoriumLayouts/";
            }


            foreach (var fileFullName in Directory.EnumerateFiles($"{directoryName}"))

            {
                if (fileFullName.Contains("_booked_seats.json"))
                {
                    var fileName = Path.GetFileName(fileFullName);

                    var eventId = Path.GetFileName(fileName.Split("-")[0]);

                    _repository[eventId] = JsonFile.ReadFromJsonFile<ReservedSeatsDto>(fileFullName);
                }
            }
        }

        public Task<ReservedSeatsDto> GetReservedSeats(string showId)
        {
            if (_repository.ContainsKey(showId))
            {
                return Task.FromResult(_repository[showId]);
            }

            return Task.FromResult(new ReservedSeatsDto());
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