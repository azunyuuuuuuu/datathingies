using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

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

        public async Task<Covid19DataEntry[]> GetAllDataAsync()
        {
            await EnsureCsvFileIsFresh();
            await EnsureCsvFileIsLoaded();
            return rawdata.ToArray();
        }

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

    public record Covid19DataEntry
    {
        [Name("iso_code")] public string IsoCode { get; init; }
        [Name("continent")] public string Continent { get; init; }
        [Name("location")] public string Location { get; init; }
        [Name("date")] public DateTime Date { get; init; }
        [Name("total_cases")] public double? TotalCases { get; init; }
        [Name("new_cases")] public double? NewCases { get; init; }
        [Name("new_cases_smoothed")] public double? NewCasesSmoothed { get; init; }
        [Name("total_deaths")] public double? TotalDeaths { get; init; }
        [Name("new_deaths")] public double? NewDeaths { get; init; }
        [Name("new_deaths_smoothed")] public double? NewDeathsSmoothed { get; init; }
        [Name("total_vaccinations")] public double? TotalVaccinations { get; init; }
        [Name("people_vaccinated")] public double? PeopleVaccinated { get; init; }
        [Name("people_fully_vaccinated")] public double? PeopleFullyVaccinated { get; init; }
        [Name("new_vaccinations")] public double? NewVaccinations { get; init; }
        [Name("new_vaccinations_smoothed")] public double? NewVaccinationsSmoothed { get; init; }
        [Name("total_vaccinations_per_hundred")] public double? TotalVaccinationsPerHundred { get; init; }
        [Name("people_vaccinated_per_hundred")] public double? PeopleVaccinatedPerHundred { get; init; }
        [Name("people_fully_vaccinated_per_hundred")] public double? PeopleFullyVaccinatedPerHundred { get; init; }
        [Name("new_vaccinations_smoothed_per_million")] public double? NewVaccinationsSmoothedPerMillion { get; init; }
    }
}
