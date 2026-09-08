using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ViewYourPayments.DataImport.Domain;
using IConfigurationProvider = Microsoft.Extensions.Configuration.IConfigurationProvider;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var providers = new List<IConfigurationProvider>();

foreach (var descriptor in builder.Services.Where(descriptor => descriptor.ServiceType == typeof(IConfiguration)).ToList())
{
    var existingConfiguration = descriptor.ImplementationInstance as IConfigurationRoot;
    if (existingConfiguration is null)
    {
        continue;
    }
    providers.AddRange(existingConfiguration.Providers);
    builder.Services.Remove(descriptor);
}

var config = builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddEnvironmentVariables()
    .Build();

providers.AddRange(config.Providers);

builder.Services.AddSingleton<IConfiguration>(new ConfigurationRoot(providers));

builder.Services
    .AddSingleton(new DataImportSetting());

builder.Build().Run();