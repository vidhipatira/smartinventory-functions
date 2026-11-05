using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.AddApplicationInsights();
    })
    .ConfigureServices(services =>
    {
        services.AddOpenTelemetry()
            .UseAzureMonitorExporter();
    })
    .Build();

host.Run();
