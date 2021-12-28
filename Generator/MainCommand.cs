using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using CsvHelper;

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

        await DownloadDataIntoCacheAsync(console);

        await LoadCachedDataAsync(console);
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
