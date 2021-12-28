// See https://aka.ms/new-console-template for more information
using CliFx;

await new CliApplicationBuilder()
    .AddCommandsFromThisAssembly()
    .Build()
    .RunAsync();
