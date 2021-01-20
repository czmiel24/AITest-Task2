using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AITest.Startup))]

namespace AITest
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
               .SetBasePath(Environment.CurrentDirectory)
               .AddEnvironmentVariables()
               .AddJsonFile("local.settings.json", true)
               .AddUserSecrets(Assembly.GetExecutingAssembly(), false)
               .Build();

            Configuration configuration = GetConfiguration(config);
            builder.Services.AddSingleton<Configuration>(x => configuration);
            builder.Services.AddTransient<ITrainingClient, ApiTrainingClient>();

            builder.Services.AddHttpClient("", httpClient => 
                httpClient.DefaultRequestHeaders.Add ("Training-key", configuration.TrainingKey)
            );
        }

        private Configuration GetConfiguration(IConfigurationRoot config)
        {
            return new Configuration {
                TrainingKey = config.GetValue<string>("TrainingKey"),
                ProjectId = config.GetValue<string>("ProjectId"),
                TrainingEndpoint = config.GetValue<string>("TrainingEndpoint")
            };
        }
    }
}