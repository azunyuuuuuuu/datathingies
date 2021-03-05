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
        private readonly HttpClient _http;
        private static readonly string _datafile = Path.Combine("_data", "covid19.csv");
        private List<Covid19DataEntry> rawdata;

        public Covid19DataService(HttpClient http)
        {
            _http = http;
        }

        public async Task InitializeData()
            => await EnsureCsvFileIsLoaded();

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
                   DataModes.Cases7DayAverage => new TableData(x.Date, x.NewCases7DayAverage ?? 0),
                   DataModes.Deaths7DayAverage => new TableData(x.Date, x.NewDeaths7DayAverage ?? 0),
                   DataModes.Vaccinations => new TableData(x.Date, x.NewVaccinations ?? 0),
                   DataModes.Vaccinations7DayAverage => new TableData(x.Date, x.NewVaccinations7DayAverage ?? 0),
                   _ => new TableData(x.Date, 0)
               });

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

            var colored = grouped
                .Select(x => x with
                {
                    ColorMonday = x.Monday?.LerpWith(highest).ToHexString(),
                    ColorTuesday = x.Tuesday?.LerpWith(highest).ToHexString(),
                    ColorWednesday = x.Wednesday?.LerpWith(highest).ToHexString(),
                    ColorThursday = x.Thursday?.LerpWith(highest).ToHexString(),
                    ColorFriday = x.Friday?.LerpWith(highest).ToHexString(),
                    ColorSaturday = x.Saturday?.LerpWith(highest).ToHexString(),
                    ColorSunday = x.Sunday?.LerpWith(highest).ToHexString(),
                });

            return colored.OrderByDescending(x => x.Week);
        }

        public HeatmapMetadata GetHeatmapMetadataForCountryMode(string country, DataModes mode)
        {
            var data = GetDataForCountry(country);

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
                        MinValue = 0,
                        MinWeeklyValue = 0,
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
                        MinValue = 1,
                        MinWeeklyValue = 1,
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
                case DataModes.Vaccinations: return output with { MinValue = 0, MaxValue = data.Max(x => x.NewVaccinations ?? 0) };
                case DataModes.Vaccinations7DayAverage: return output with { MinValue = 0, MaxValue = data.Max(x => x.NewVaccinations7DayAverage ?? 0) };
                default: return output with { MinValue = 0, MaxValue = 0 };
            }
        }

        internal IEnumerable<Covid19CondensedData> GetCovid19CondensedData()
            => rawdata.Where(x => !string.IsNullOrWhiteSpace(x.Continent))
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
            if (rawdata != null)
                return;

            rawdata = new List<Covid19DataEntry>();

            using var stream = await _http.GetStreamAsync("https://covid.ourworldindata.org/data/owid-covid-data.csv");
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            await foreach (var item in csv.GetRecordsAsync<Covid19DataEntry>())
                rawdata.Add(item);
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
