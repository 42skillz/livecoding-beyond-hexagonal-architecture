using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ExternalDependencies.AuditoriumLayoutRepository
{
    public class AuditoriumLayoutRepository : IProvideAuditoriumLayouts
    {
        private readonly Dictionary<string, AuditoriumDto> _repository = new Dictionary<string, AuditoriumDto>();

        public AuditoriumLayoutRepository()
        {
            var directoryName = $"{GetExecutingAssemblyDirectoryFullPath()}\\AuditoriumLayouts\\";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                directoryName = $"{GetExecutingAssemblyDirectoryFullPath()}/AuditoriumLayouts/";
            }

            Console.WriteLine(directoryName);

            foreach (var fileFullName in Directory.EnumerateFiles($"{directoryName}"))
            {
                if (fileFullName.Contains("_theater.json"))
                {
                    var fileName = Path.GetFileName(fileFullName);

                    var eventId = Path.GetFileName(fileName.Split("-")[0]);
                    _repository[eventId] = JsonFile.ReadFromJsonFile<AuditoriumDto>(fileFullName);
                }
            }
        }

        public Task<AuditoriumDto> GetAuditoriumSeatingFor(string showId)
        {
            if (_repository.ContainsKey(showId))
            {
                return Task.FromResult(_repository[showId]);
            }

            return Task.FromResult(new AuditoriumDto());
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