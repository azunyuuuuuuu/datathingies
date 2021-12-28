using CsvHelper.Configuration.Attributes;

public record CondensedDataEntry
{
    // basics
    [Name("iso_code")] public string? IsoCode { get; init; }
    [Name("continent")] public string? Continent { get; init; }
    [Name("location")] public string? Location { get; init; }
    [Name("date")] public DateOnly? Date { get; init; }

    // cases
    [Name("total_cases")] public double? TotalCases { get; init; }
    [Name("new_cases")] public double? NewCases { get; init; }
    [Name("new_cases_smoothed")] public double? NewCasesSmoothed { get; init; }
    
    [Name("total_cases_per_million")] public double? TotalCasesPerMillion { get; init; }
    [Name("new_cases_per_million")] public double? NewCasesPerMillion { get; init; }
    [Name("new_cases_smoothed_per_million")] public double? NewCasesSmoothedPerMillion { get; init; }

    // deaths
    [Name("total_deaths")] public double? TotalDeaths { get; init; }
    [Name("new_deaths")] public double? NewDeaths { get; init; }
    [Name("new_deaths_smoothed")] public double? NewDeathsSmoothed { get; init; }
    
    [Name("total_deaths_per_million")] public double? TotalDeathsPerMillion { get; init; }
    [Name("new_deaths_per_million")] public double? NewDeathsPerMillion { get; init; }
    [Name("new_deaths_smoothed_per_million")] public double? NewDeathsSmoothedPerMillion { get; init; }
    
    // other values
    [Name("reproduction_rate")] public double? ReproductionRate { get; init; }

    // hospital
    [Name("icu_patients")] public double? IcuPatients { get; init; }
    [Name("icu_patients_per_million")] public double? IcuPatientsPerMillion { get; init; }
    [Name("hosp_patients")] public double? HospPatients { get; init; }
    [Name("hosp_patients_per_million")] public double? HospPatientsPerMillion { get; init; }
    [Name("weekly_icu_admissions")] public double? WeeklyIcuAdmissions { get; init; }
    [Name("weekly_icu_admissions_per_million")] public double? WeeklyIcuAdmissionsPerMillion { get; init; }
    [Name("weekly_hosp_admissions")] public double? WeeklyHospAdmissions { get; init; }
    [Name("weekly_hosp_admissions_per_million")] public double? WeeklyHospAdmissionsPerMillion { get; init; }

    // vaccination
    [Name("total_vaccinations")] public double? TotalVaccinations { get; init; }
    [Name("people_vaccinated")] public double? PeopleVaccinated { get; init; }
    [Name("people_fully_vaccinated")] public double? PeopleFullyVaccinated { get; init; }
    [Name("total_boosters")] public double? TotalBoosters { get; init; }
    [Name("new_vaccinations")] public double? NewVaccinations { get; init; }
    [Name("new_vaccinations_smoothed")] public double? NewVaccinationsSmoothed { get; init; }
    
    [Name("total_vaccinations_per_hundred")] public double? TotalVaccinationsPerHundred { get; init; }
    [Name("people_vaccinated_per_hundred")] public double? PeopleVaccinatedPerHundred { get; init; }
    [Name("people_fully_vaccinated_per_hundred")] public double? PeopleFullyVaccinatedPerHundred { get; init; }
    [Name("total_boosters_per_hundred")] public double? TotalBoostersPerHundred { get; init; }
    [Name("new_vaccinations_smoothed_per_million")] public double? NewVaccinationsSmoothedPerMillion { get; init; }
    [Name("new_people_vaccinated_smoothed")] public double? NewPeopleVaccinatedSmoothed { get; init; }
    [Name("new_people_vaccinated_smoothed_per_hundred")] public double? NewPeopleVaccinatedSmoothedPerHundred { get; init; }
}
