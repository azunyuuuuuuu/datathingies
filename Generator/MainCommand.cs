using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

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

    private Task LoadCachedDataAsync(IConsole console)
    {
        throw new NotImplementedException();
    }

    private async ValueTask DownloadDataIntoCacheAsync(IConsole console)
    {
        Directory.CreateDirectory(_cacherootpath);

        using var client = new HttpClient();
        using var stream = await client.GetStreamAsync(@"https://covid.ourworldindata.org/data/owid-covid-data.csv");
        using var filestream = File.Open(Path.Combine(_cacherootpath, @"data.csv"), FileMode.Create);

        await stream.CopyToAsync(filestream);
        await filestream.FlushAsync();
    }
}