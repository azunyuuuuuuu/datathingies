using System;
using CsvHelper.Configuration.Attributes;

namespace datathingies.Data
{
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
