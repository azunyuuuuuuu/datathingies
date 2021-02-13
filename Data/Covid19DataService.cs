using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;

namespace datathingies.Data
{
    public class Covid19DataService
    {
        private readonly IHttpClientFactory _http;
        private static readonly string _datafile = "covid19.csv";
        private List<Covid19DataEntry> rawdata;

        public Covid19DataService(IHttpClientFactory http)
        {
            _http = http;
        }

        public async Task InitializeData()
        {
            await EnsureCsvFileIsFresh();
            await EnsureCsvFileIsLoaded();
        }

        public IEnumerable<Covid19DataEntry> GetAllDataAsync()
            => rawdata;

        public IEnumerable<string> GetCountries()
            => rawdata.Where(x => !string.IsNullOrWhiteSpace(x.Continent))
                .GroupBy(x => x.Location)
                .Select(x => x.Key)
                .OrderBy(x => x);

        public IEnumerable<Covid19DataEntry> GetTop25CountriesByCases()
            => rawdata.Where(x => !string.IsNullOrWhiteSpace(x.Continent))
                .GroupBy(x => x.Location)
                .Select(x => x.OrderByDescending(y => y.TotalCases).First())
                .OrderByDescending(x => x.TotalCases)
                .Take(25);

        public IEnumerable<Covid19DataEntry> GetTop25CountriesByDeaths()
            => rawdata.Where(x => !string.IsNullOrWhiteSpace(x.Continent))
                .GroupBy(x => x.Location)
                .Select(x => x.OrderByDescending(y => y.TotalDeaths).First())
                .OrderByDescending(x => x.TotalDeaths)
                .Take(25);

        public IEnumerable<Covid19DataEntry> GetTop25CountriesByVaccinations()
            => rawdata.Where(x => !string.IsNullOrWhiteSpace(x.Continent))
                .GroupBy(x => x.Location)
                .Select(x => x.OrderByDescending(y => y.TotalVaccinations).First())
                .OrderByDescending(x => x.TotalVaccinations)
                .Take(25);

        public IEnumerable<Covid19WeeklyData> GetHeatmapDataForCountry(string country)
            => rawdata.Where(x => x.Location.ToLower() == country.ToLower())
                .GroupBy(x => x.Date.WeekYear())
                .Select(x => new Covid19WeeklyData
                {
                    Week = x.Key,
                    Month = x.FirstOrDefault().Date.ToString("MMMM"),
                    Monday = x.FirstOrDefault(x => x.Date.DayOfWeek == DayOfWeek.Monday),
                    Tuesday = x.FirstOrDefault(x => x.Date.DayOfWeek == DayOfWeek.Tuesday),
                    Wednesday = x.FirstOrDefault(x => x.Date.DayOfWeek == DayOfWeek.Wednesday),
                    Thursday = x.FirstOrDefault(x => x.Date.DayOfWeek == DayOfWeek.Thursday),
                    Friday = x.FirstOrDefault(x => x.Date.DayOfWeek == DayOfWeek.Friday),
                    Saturday = x.FirstOrDefault(x => x.Date.DayOfWeek == DayOfWeek.Saturday),
                    Sunday = x.FirstOrDefault(x => x.Date.DayOfWeek == DayOfWeek.Sunday),
                })
                .OrderByDescending(x => x.Week);

        private async Task EnsureCsvFileIsLoaded()
        {
            if (rawdata == null)
            {
                rawdata = new List<Covid19DataEntry>();

                var contents = await File.ReadAllBytesAsync(_datafile);
                using var stream = new MemoryStream(contents);
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                rawdata.AddRange(csv.GetRecords<Covid19DataEntry>());
            }
        }

        private async Task EnsureCsvFileIsFresh()
        {
            if (System.IO.File.Exists(_datafile) && System.IO.File.GetLastWriteTimeUtc(_datafile) >= (DateTime.UtcNow - TimeSpan.FromHours(3)))
                return;

            using var client = _http.CreateClient();
            var contents = await client.GetStringAsync("https://covid.ourworldindata.org/data/owid-covid-data.csv");
            await File.WriteAllTextAsync(_datafile, contents);
        }
    }
}
