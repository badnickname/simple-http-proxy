using System.Net.Sockets;
using SimpleProxy;
using SimpleProxy.Pac;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddHostedService<ProxyWorker>()
    .AddTransient<IProxyAutoConfigurator, ProxyAutoConfigurator>()
    .AddTransient(_ => new TcpListener(1234))
    .AddTransient<HttpListener>();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();