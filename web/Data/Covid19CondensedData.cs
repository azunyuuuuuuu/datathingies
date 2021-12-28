namespace datathingies.Data
{
    public record Covid19CondensedData
    {
        public string Location { get; init; }
        public double? TotalCases { get; init; }
        public double? TotalDeaths { get; init; }
        public double? TotalVaccinations { get; init; }
    }
}
