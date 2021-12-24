using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CsvHelper;

namespace datathingies.Data
{
    public class Covid19DataService
    {
        private readonly HttpClient _http;
        private List<Covid19DataEntry> _rawdata = new List<Covid19DataEntry>();
        private List<Covid19IndexData> _index = new List<Covid19IndexData>();

        public Covid19DataService(HttpClient http)
        {
            _http = http;
            _http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true
            };
        }

        public async Task InitializeIndex()
            => await EnsureIndexIsLoaded();

        private async Task EnsureIndexIsLoaded()
        {
            if (_index.Count > 0)
                return;

            using var stream = await _http.GetStreamAsync("data/index.csv");
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            await foreach (var item in csv.GetRecordsAsync<Covid19IndexData>())
                _index.Add(item);
        }

        private async Task EnsureDataForCountryIsLoaded(string isocode)
        {
            var contents = await _http.GetStringAsync($"data/{isocode}.csv");
            using var reader = new StringReader(contents);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var items = csv.GetRecords<Covid19DataEntry>().ToList();

            foreach (var item in items)
                if (_rawdata.Where(x => x.IsoCode.ToLower() == isocode.ToLower())
                    .Where(x => x.Date == item.Date)
                    .Count() == 0)
                    _rawdata.Add(item);
        }

        public async Task InitializeData()
            => await EnsureCsvFileIsLoaded();

        public IEnumerable<Covid19DataEntry> GetAllDataAsync()
            => _rawdata;

        public async Task<IEnumerable<Covid19IndexData>> GetCountries()
        {
            await EnsureIndexIsLoaded();

            return _index.OrderBy(x => x.Location);
        }

        public async Task<IEnumerable<Covid19DataEntry>> GetDataForCountry(string isocode)
        {
            await EnsureDataForCountryIsLoaded(isocode);

            return _rawdata.Where(x => x.IsoCode.ToLower() == isocode.ToLower())
                .OrderByDescending(x => x.Date);
        }

        public async Task<IEnumerable<Covid19WeeklyData>> GetHeatmapForCountryMode(string isocode, DataModes mode)
        {
            await EnsureDataForCountryIsLoaded(isocode);

            var data = (await GetDataForCountry(isocode))
               .Select(x => mode switch
               {
                   DataModes.Cases => new TableData(x.Date, x.NewCases ?? 0),
                   DataModes.Deaths => new TableData(x.Date, x.NewDeaths ?? 0),
                   DataModes.Cases7DayAverage => new TableData(x.Date, x.NewCases7DayAverage ?? 0),
                   DataModes.Deaths7DayAverage => new TableData(x.Date, x.NewDeaths7DayAverage ?? 0),
                   DataModes.Vaccinations => new TableData(x.Date, x.NewVaccinations ?? 0),
                   DataModes.Vaccinations7DayAverage => new TableData(x.Date, x.NewVaccinations7DayAverage ?? 0),
                   _ => new TableData(x.Date, 0)
               })
               .ToList();

            var highest = data.Max(x => x.value);

            var grouped = data.GroupBy(x => x.date.WeekYear())
                .Select(x => new Covid19WeeklyData
                {
                    Date = x.OrderBy(x => x.date).FirstOrDefault().date,
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

            return grouped;
        }

        public async Task<HeatmapMetadata> GetHeatmapMetadataForCountryMode(string isocode, DataModes mode)
        {
            var data = await GetDataForCountry(isocode);

            var output = new HeatmapMetadata();

            switch (mode)
            {
                default:
                case DataModes.Cases:
                case DataModes.Deaths:
                case DataModes.Cases7DayAverage:
                case DataModes.Deaths7DayAverage:
                    output = output with
                    {
                        Gradient = new ColorGradient
                        {
                            Colors = new List<Color>{
                                @"#63BE7B".ToColor(),
                                @"#FFEB84".ToColor(),
                                @"#F8696B".ToColor(),
                            }
                        }
                    };
                    break;

                case DataModes.Vaccinations:
                case DataModes.Vaccinations7DayAverage:
                    output = output with
                    {
                        Gradient = new ColorGradient
                        {
                            Colors = new List<Color>{
                                @"#F8696B".ToColor(),
                                @"#FFEB84".ToColor(),
                                @"#63BE7B".ToColor(),
                            }
                        }
                    };
                    break;
            };

            switch (mode)
            {
                case DataModes.Cases: return output with { MinValue = 0, MaxValue = data.Max(x => x.NewCases ?? 0) };
                case DataModes.Deaths: return output with { MinValue = 0, MaxValue = data.Max(x => x.NewDeaths ?? 0) };
                case DataModes.Cases7DayAverage: return output with { MinValue = 0, MaxValue = data.Max(x => x.NewCases7DayAverage ?? 0) };
                case DataModes.Deaths7DayAverage: return output with { MinValue = 0, MaxValue = data.Max(x => x.NewDeaths7DayAverage ?? 0) };
                case DataModes.Vaccinations: return output with { MinValue = 1, MaxValue = data.Max(x => x.NewVaccinations ?? 0) };
                case DataModes.Vaccinations7DayAverage: return output with { MinValue = 1, MaxValue = data.Max(x => x.NewVaccinations7DayAverage ?? 0) };
                default: return output with { MinValue = 0, MaxValue = 0 };
            }
        }

        internal IEnumerable<Covid19CondensedData> GetCovid19CondensedData()
            => _rawdata.Where(x => !string.IsNullOrWhiteSpace(x.Continent))
                .GroupBy(x => x.Location)
                .Select(x => new Covid19CondensedData
                {
                    Location = x.Key,
                    TotalCases = x.Sum(x => x.NewCases),
                    TotalDeaths = x.Sum(x => x.NewDeaths),
                    TotalVaccinations = x.Sum(x => x.NewVaccinations)
                });

        public IEnumerable<Covid19CondensedData> GetHighest25CountriesByCases()
            => GetCovid19CondensedData()
                .OrderByDescending(x => x.TotalCases)
                .Take(25);

        public IEnumerable<Covid19CondensedData> GetHighest25CountriesByDeaths()
            => GetCovid19CondensedData()
                .OrderByDescending(x => x.TotalDeaths)
                .Take(25);

        public IEnumerable<Covid19CondensedData> GetHighest25CountriesByVaccinations()
            => GetCovid19CondensedData()
                .OrderByDescending(x => x.TotalVaccinations)
                .Take(25);

        private async Task EnsureCsvFileIsLoaded()
        {
            if (_rawdata != null)
                return;

            using var stream = await _http.GetStreamAsync("https://covid.ourworldindata.org/data/owid-covid-data.csv");
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            await foreach (var item in csv.GetRecordsAsync<Covid19DataEntry>())
                _rawdata.Add(item);
        }

        public record TableData(DateTime date, double value = 0);

        public enum DataModes
        {
            Cases,
            Cases7DayAverage,
            Deaths,
            Deaths7DayAverage,
            Vaccinations,
            Vaccinations7DayAverage,
        }
    }
}
