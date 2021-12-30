using CsvHelper.Configuration.Attributes;

public record RawDataEntry
{
    [Name("iso_code")] public string? IsoCode { get; init; }
    [Name("continent")] public string? Continent { get; init; }
    [Name("location")] public string? Location { get; init; }
    [Name("date")] public DateOnly? Date { get; init; }
    [Name("total_cases")] public double? TotalCases { get; init; }
    [Name("new_cases")] public double? NewCases { get; init; }
    [Name("new_cases_smoothed")] public double? NewCasesSmoothed { get; init; }
    [Name("total_deaths")] public double? TotalDeaths { get; init; }
    [Name("new_deaths")] public double? NewDeaths { get; init; }
    [Name("new_deaths_smoothed")] public double? NewDeathsSmoothed { get; init; }
    [Name("total_cases_per_million")] public double? TotalCasesPerMillion { get; init; }
    [Name("new_cases_per_million")] public double? NewCasesPerMillion { get; init; }
    [Name("new_cases_smoothed_per_million")] public double? NewCasesSmoothedPerMillion { get; init; }
    [Name("total_deaths_per_million")] public double? TotalDeathsPerMillion { get; init; }
    [Name("new_deaths_per_million")] public double? NewDeathsPerMillion { get; init; }
    [Name("new_deaths_smoothed_per_million")] public double? NewDeathsSmoothedPerMillion { get; init; }
    [Name("reproduction_rate")] public double? ReproductionRate { get; init; }
    [Name("icu_patients")] public double? IcuPatients { get; init; }
    [Name("icu_patients_per_million")] public double? IcuPatientsPerMillion { get; init; }
    [Name("hosp_patients")] public double? HospPatients { get; init; }
    [Name("hosp_patients_per_million")] public double? HospPatientsPerMillion { get; init; }
    [Name("weekly_icu_admissions")] public double? WeeklyIcuAdmissions { get; init; }
    [Name("weekly_icu_admissions_per_million")] public double? WeeklyIcuAdmissionsPerMillion { get; init; }
    [Name("weekly_hosp_admissions")] public double? WeeklyHospAdmissions { get; init; }
    [Name("weekly_hosp_admissions_per_million")] public double? WeeklyHospAdmissionsPerMillion { get; init; }
    [Name("new_tests")] public double? NewTests { get; init; }
    [Name("total_tests")] public double? TotalTests { get; init; }
    [Name("total_tests_per_thousand")] public double? TotalTestsPerThousand { get; init; }
    [Name("new_tests_per_thousand")] public double? NewTestsPerThousand { get; init; }
    [Name("new_tests_smoothed")] public double? NewTestsSmoothed { get; init; }
    [Name("new_tests_smoothed_per_thousand")] public double? NewTestsSmoothedPerThousand { get; init; }
    [Name("positive_rate")] public double? PositiveRate { get; init; }
    [Name("tests_per_case")] public double? TestsPerCase { get; init; }
    [Name("tests_units")] public string? TestsUnits { get; init; }
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
    [Name("stringency_index")] public double? StringencyIndex { get; init; }
    [Name("population")] public double? Population { get; init; }
    [Name("population_density")] public double? PopulationDensity { get; init; }
    [Name("median_age")] public double? MedianAge { get; init; }
    [Name("aged_65_older")] public double? Aged_65_older { get; init; }
    [Name("aged_70_older")] public double? Aged_70_older { get; init; }
    [Name("gdp_per_capita")] public double? GdpPerCapita { get; init; }
    [Name("extreme_poverty")] public double? ExtremePoverty { get; init; }
    [Name("cardiovasc_death_rate")] public double? CardiovascDeathRate { get; init; }
    [Name("diabetes_prevalence")] public double? DiabetesPrevalence { get; init; }
    [Name("female_smokers")] public double? FemaleSmokers { get; init; }
    [Name("male_smokers")] public double? MaleSmokers { get; init; }
    [Name("handwashing_facilities")] public double? HandwashingFacilities { get; init; }
    [Name("hospital_beds_per_thousand")] public double? HospitalBedsPerThousand { get; init; }
    [Name("life_expectancy")] public double? LifeExpectancy { get; init; }
    [Name("human_development_index")] public double? HumanDevelopmentIndex { get; init; }
    [Name("excess_mortality_cumulative_absolute")] public double? ExcessMortalityCumulativeAbsolute { get; init; }
    [Name("excess_mortality_cumulative")] public double? ExcessMortalityCumulative { get; init; }
    [Name("excess_mortality")] public double? ExcessMortality { get; init; }
    [Name("excess_mortality_cumulative_per_million")] public double? ExcessMortalityCumulativePerMillion { get; init; }
}