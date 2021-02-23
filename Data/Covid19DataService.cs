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
        private static readonly string _datafile = Path.Combine("_data", "covid19.csv");
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

        public IEnumerable<Covid19DataEntry> GetDataForCountry(string country)
            => rawdata.Where(x => x.Location.ToLower() == country.ToLower())
                .OrderByDescending(x => x.Date);

        public IEnumerable<Covid19WeeklyData> GetHeatmapForCountryMode(string country, DataModes mode)
        {
            var data = GetDataForCountry(country)
               .Select(x => mode switch
               {
                   DataModes.Cases => new TableData(x.Date, x.NewCases ?? 0),
                   DataModes.Deaths => new TableData(x.Date, x.NewDeaths ?? 0),
                   DataModes.CasesSmoothed => new TableData(x.Date, x.NewCasesSmoothed ?? 0),
                   DataModes.DeathsSmoothed => new TableData(x.Date, x.NewDeathsSmoothed ?? 0),
                   DataModes.Vaccinations => new TableData(x.Date, x.NewVaccinations ?? 0),
                   DataModes.VaccinationsSmoothed => new TableData(x.Date, x.NewVaccinationsSmoothed ?? 0),
                   _ => new TableData(x.Date, 0)
               });

            var highest = data.Max(x => x.value);

            var grouped = data.GroupBy(x => x.date.WeekYear())
                .Select(x => new Covid19WeeklyData
                {
                    Week = x.Key,
                    Month = x.FirstOrDefault().date.ToString("MMMM"),
                    Monday = x.FirstOrDefault(x => x.date.DayOfWeek == DayOfWeek.Monday)?.value,
                    Tuesday = x.FirstOrDefault(x => x.date.DayOfWeek == DayOfWeek.Tuesday)?.value,
                    Wednesday = x.FirstOrDefault(x => x.date.DayOfWeek == DayOfWeek.Wednesday)?.value,
                    Thursday = x.FirstOrDefault(x => x.date.DayOfWeek == DayOfWeek.Thursday)?.value,
                    Friday = x.FirstOrDefault(x => x.date.DayOfWeek == DayOfWeek.Friday)?.value,
                    Saturday = x.FirstOrDefault(x => x.date.DayOfWeek == DayOfWeek.Saturday)?.value,
                    Sunday = x.FirstOrDefault(x => x.date.DayOfWeek == DayOfWeek.Sunday)?.value,
                    Weekly = x.Sum(x => x.value),
                });
            var colored = grouped
                .Select(x => x with
                {
                    ColorMonday = x.Monday?.LerpWith(highest),
                    ColorTuesday = x.Tuesday?.LerpWith(highest),
                    ColorWednesday = x.Wednesday?.LerpWith(highest),
                    ColorThursday = x.Thursday?.LerpWith(highest),
                    ColorFriday = x.Friday?.LerpWith(highest),
                    ColorSaturday = x.Saturday?.LerpWith(highest),
                    ColorSunday = x.Sunday?.LerpWith(highest),
                });
            return colored.OrderByDescending(x => x.Week);
        }

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

        public record TableData(DateTime date, double value = 0);

        public enum DataModes
        {
            Cases,
            CasesSmoothed,
            Deaths,
            DeathsSmoothed,
            Vaccinations,
            VaccinationsSmoothed,
        }
    }
}
