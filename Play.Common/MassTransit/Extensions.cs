using System.Reflection;
using GreenPipes;
using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class MassTransitExt
    {
        public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection services)
        {
            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumers(Assembly.GetEntryAssembly());

                busConfigurator.UsingRabbitMq((context, configurator) =>
                {
                    // Get & Deserialize Settings From AppSettings...
                    var configs = context.GetService<IConfiguration>();
                    var rabbitMqSettings = configs.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    var serviceSettings = configs.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

                    configurator.Host(rabbitMqSettings?.Host);
                    configurator.ConfigureEndpoints(context,
                    new KebabCaseEndpointNameFormatter(serviceSettings?.ServiceName, false));
                    configurator.UseMessageRetry(retryConfigurator =>
                    {
                        retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });

            services.AddMassTransitHostedService();

            return services;
        }
    }
}