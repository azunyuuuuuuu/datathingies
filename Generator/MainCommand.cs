using System.Globalization;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using CsvHelper;
using Scriban;

[Command]
public class MainCommand : ICommand
{
    private const string _cacherootpath = @"cache";
    private const string _outputpath = @"output";

    [CommandOption("cached", 'c', Description = "Use precached data to generate output data.")]
    public bool UseCachedData { get; set; } = true;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine("Hello world!");

        if (UseCachedData == false)
            await DownloadDataIntoCacheAsync(console);

        // await LoadCachedDataAsync(console);

        await GenerateSvgFilesAsync(console);
    }

    private async ValueTask GenerateSvgFilesAsync(IConsole console)
    {
        using var stream = File.OpenRead(Path.Combine(_cacherootpath, @"owid-covid-data.csv"));
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

        var groups = csv.GetRecords<RawDataEntry>().GroupBy(x => x.IsoCode);

        foreach (var group in groups)
        {
            var outputpath = Path.Combine(_outputpath, group.Key.ToLowerInvariant());
            Directory.CreateDirectory(outputpath);

            var casesMax = group.Max(x => x.NewCases);
            var deathsMax = group.Max(x => x.NewDeaths);
            var vaccinationsMax = group.Max(x => x.NewVaccinations);
            var casesSmoothedMax = group.Max(x => x.NewCasesSmoothed);
            var deathsSmoothedMax = group.Max(x => x.NewDeathsSmoothed);
            var vaccinationsSmoothedMax = group.Max(x => x.NewVaccinationsSmoothed);
            var hospPatientsMax = group.Max(x => x.HospPatients);
            var icuPatientsMax = group.Max(x => x.IcuPatients);

            var output = group.Select(x => new
            {
                Date = x.Date,
                DayOfWeek = ((int?)x.Date?.ToDateTime(TimeOnly.MinValue).DayOfWeek) - 1,
                Week = ISOWeek.GetWeekOfYear(x.Date?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue),
                Year = ISOWeek.GetYear(x.Date?.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue),
                Cases = x.NewCases,
                Deaths = x.NewDeaths,
                Vaccinations = x.NewVaccinations,
                CasesSmoothed = x.NewCasesSmoothed,
                DeathsSmoothed = x.NewDeathsSmoothed,
                VaccinationsSmoothed = x.NewVaccinationsSmoothed,
                HospitalPatients = x.HospPatients,
                IcuPatients = x.IcuPatients,
            }).Select(x => x with { DayOfWeek = x.DayOfWeek < 0 ? 6 : x.DayOfWeek })
            .OrderBy(x => x.Date);

            var gradient = new ColorGradient
            {
                Colors = new List<Color> { @"#63BE7B".ToColor(), @"#FFEB84".ToColor(), @"#F8696B".ToColor(), }
            };
            var gradientreverse = new ColorGradient
            {
                Colors = new List<Color> { @"#F8696B".ToColor(), @"#FFEB84".ToColor(), @"#63BE7B".ToColor(), }
            };

            var dataCases = output.Select(x => new OutputData(x.Date, x.DayOfWeek, x.Week, x.Year, x.Cases, gradient.GetColorAt(1 / casesMax * x.Cases).ToHexString()));
            var dataDeaths = output.Select(x => new OutputData(x.Date, x.DayOfWeek, x.Week, x.Year, x.Deaths, gradient.GetColorAt(1 / deathsMax * x.Deaths).ToHexString()));
            var dataVaccinations = output.Select(x => new OutputData(x.Date, x.DayOfWeek, x.Week, x.Year, x.Vaccinations, gradientreverse.GetColorAt(1 / vaccinationsMax * x.Vaccinations).ToHexString()));
            var dataCasesSmoothed = output.Select(x => new OutputData(x.Date, x.DayOfWeek, x.Week, x.Year, x.CasesSmoothed, gradient.GetColorAt(1 / casesSmoothedMax * x.CasesSmoothed).ToHexString()));
            var dataDeathsSmoothed = output.Select(x => new OutputData(x.Date, x.DayOfWeek, x.Week, x.Year, x.DeathsSmoothed, gradient.GetColorAt(1 / deathsSmoothedMax * x.DeathsSmoothed).ToHexString()));
            var dataVaccinationsSmoothed = output.Select(x => new OutputData(x.Date, x.DayOfWeek, x.Week, x.Year, x.VaccinationsSmoothed, gradientreverse.GetColorAt(1 / vaccinationsSmoothedMax * x.VaccinationsSmoothed).ToHexString()));
            var dataHospitalPatients = output.Select(x => new OutputData(x.Date, x.DayOfWeek, x.Week, x.Year, x.HospitalPatients, gradient.GetColorAt(1 / hospPatientsMax * x.HospitalPatients).ToHexString()));
            var dataIcuPatients = output.Select(x => new OutputData(x.Date, x.DayOfWeek, x.Week, x.Year, x.IcuPatients, gradient.GetColorAt(1 / icuPatientsMax * x.IcuPatients).ToHexString()));

            var template = Template.Parse(await File.ReadAllTextAsync("svg.template"));

            var list = dataCases.GroupBy(x => $"{x.Year}{x.Week}")
                .OrderBy(x => x.Key)
                .ToList();

            var model = new
            {
                ProcessedData = list,
                Count = list.Count,
                Size = 10,
            };

            var rendered = await template.RenderAsync(model);
            await File.WriteAllTextAsync(Path.Combine(outputpath, "raw.svg"), rendered, System.Text.Encoding.UTF8);

            // var weeks = output.GroupBy(x => $"{x.Year} {x.Week}")
            //     .OrderByDescending(x => x.Key).ToList();

            // var outputstring = string.Empty;

            // foreach (var week in weeks)
            // {
            //     var svgWeekStart = $"<g transform=\"translate({weeks.IndexOf(week) * 12}, 0)\">";

            //     var svgDays = string.Join(Environment.NewLine, week.OrderBy(x => x.DayOfWeek)
            //         .Select(x => $"<rect transform=\"translate(0, {x.DayOfWeek * 12})\" width=\"10\" height=\"10\" style=\"fill: @Metadata.GetColorAtAsHex(week.Monday)\">"));

            //     var svgWeekEnd = $"</g>";

            //     var svgWeek = string.Join(Environment.NewLine, svgWeekStart, svgDays, svgWeekEnd);

            //     outputstring += svgWeek + Environment.NewLine;
            // }


        }
    }

    private async ValueTask LoadCachedDataAsync(IConsole console)
    {
        Directory.CreateDirectory(_outputpath);

        using var stream = File.OpenRead(Path.Combine(_cacherootpath, @"owid-covid-data.csv"));
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

        var groups = csv.GetRecords<CondensedDataEntry>().GroupBy(x => x.IsoCode);

        foreach (var group in groups)
        {
            using var writer = File.CreateText(Path.Combine(_outputpath, $"{group.Key}.csv"));
            using var writecsv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture);

            await writecsv.WriteRecordsAsync<CondensedDataEntry>(group);
        }
    }

    private async ValueTask DownloadDataIntoCacheAsync(IConsole console)
    {
        Directory.CreateDirectory(_cacherootpath);

        using var client = new HttpClient();
        using var stream = await client.GetStreamAsync(@"https://covid.ourworldindata.org/data/owid-covid-data.csv");
        using var filestream = File.Open(Path.Combine(_cacherootpath, @"owid-covid-data.csv"), FileMode.Create);

        await stream.CopyToAsync(filestream);
        await filestream.FlushAsync();
    }
}

internal record OutputData(DateOnly? Date, int? DayOfWeek, int? Week, int? Year, double? Count, string Color);
