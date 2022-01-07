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
    public bool UseCachedData { get; set; } = false;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine("Hello world!");

        if (UseCachedData == false)
            await DownloadDataIntoCacheAsync(console);

        await GenerateSvgFilesAsync(console);
    }

    private async ValueTask GenerateSvgFilesAsync(IConsole console)
    {
        using var stream = File.OpenRead(Path.Combine(_cacherootpath, @"owid-covid-data.csv"));
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

        var groups = csv.GetRecords<RawDataEntry>().GroupBy(x => x.IsoCode);

        var index = new List<string> { "iso_code,continent,location" };

        foreach (var group in groups)
        {
            index.Add($"{group.Key.ToLowerInvariant()},{group.First().Continent},{group.First().Location}");
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
            })
                .Select(x => x with { DayOfWeek = x.DayOfWeek < 0 ? 6 : x.DayOfWeek })
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

            await GenerateSvgFile(Path.Combine(outputpath, "Cases.svg"), dataCases, template);
            await GenerateSvgFile(Path.Combine(outputpath, "Deaths.svg"), dataDeaths, template);
            await GenerateSvgFile(Path.Combine(outputpath, "Vaccinations.svg"), dataVaccinations, template);
            await GenerateSvgFile(Path.Combine(outputpath, "CasesSmoothed.svg"), dataCasesSmoothed, template);
            await GenerateSvgFile(Path.Combine(outputpath, "DeathsSmoothed.svg"), dataDeathsSmoothed, template);
            await GenerateSvgFile(Path.Combine(outputpath, "VaccinationsSmoothed.svg"), dataVaccinationsSmoothed, template);
            await GenerateSvgFile(Path.Combine(outputpath, "HospitalPatients.svg"), dataHospitalPatients, template);
            await GenerateSvgFile(Path.Combine(outputpath, "IcuPatients.svg"), dataIcuPatients, template);
        }

        await File.WriteAllLinesAsync(Path.Combine(_outputpath, "index.csv"), index);
    }

    private static async Task GenerateSvgFile(string path, IEnumerable<OutputData> dataCases, Template template)
    {
        var list = dataCases.GroupBy(x => $"{x.Year} {x.Week?.ToString("D2")}")
            .OrderBy(x => x.Key)
            .ToList();

        var model = new
        {
            ProcessedData = list,
            Count = list.Count,
            Size = 10,
        };

        var rendered = await template.RenderAsync(model);
        await File.WriteAllTextAsync(path, rendered, System.Text.Encoding.UTF8);
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
