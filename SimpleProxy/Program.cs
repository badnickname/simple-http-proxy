using SimpleProxy.Core;
using SimpleProxy.Js;

if (args.Contains("--help"))
{
    Console.WriteLine("Usage: SimpleProxy.exe --port 1234 --pac pac.js [[ --timeout 5 ]]");
    return;
}

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

var timeout = builder.Configuration.GetValue<int?>("timeout");
var port = builder.Configuration.GetValue<int>("port");
var pac = builder.Configuration.GetValue<string>("pac");
if (string.IsNullOrEmpty(pac)) throw new SimpleProxyException("Define path to PAC file via [ --pac filename.js ]");
if (port == 0) throw new SimpleProxyException("Define port of proxy via [ --port 1234 ]");

builder.Services.AddSimpleProxy(options =>
{
    options.Timeout = timeout ?? 5;
    options.Port = port;
});
builder.Services.AddSimpleProxyJs(options => options.FilePath = pac);
var app = builder.Build();

app.Run();

public sealed class SimpleProxyException(string? message) : Exception(message);