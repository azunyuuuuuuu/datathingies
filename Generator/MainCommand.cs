using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using CsvHelper;

[Command]
public class MainCommand : ICommand
{
    private const string _cacherootpath = @"cache";

    [CommandOption("cached", 'c', Description = "Use precached data to generate output data.")]
    public bool UseCachedData { get; set; } = true;

    public async ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine("Hello world!");

        await DownloadDataIntoCacheAsync(console);

        await LoadCachedDataAsync(console);
    }

    private async Task LoadCachedDataAsync(IConsole console)
    {
        using var stream = File.OpenRead(Path.Combine(_cacherootpath, @"owid-covid-data.csv"));
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);

        await foreach (var item in csv.GetRecordsAsync<Covid19DataEntry>())
        {
            await console.Output.WriteLineAsync(item.ToString());
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
