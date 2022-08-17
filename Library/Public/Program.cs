using Autofac;
using Autofac.Versioned;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ProductReservationSystem.Public
{
    public record Configuration(
        Action<ContainerBuilder, IConfiguration> ConfigureStorage,
        IConfiguration Text);

    public class Program
    {
        public static Configuration SampleConfig(
            Action<ContainerBuilder, IConfiguration> storageConfig, 
            params string[] args)
        {
            var textualConfig = new ConfigurationBuilder()
                   .AddCommandLine(args, new Dictionary<string, string> {
                    { "-version", "Version" }
                   })
                   .Build();

            return new Configuration(storageConfig, textualConfig);
        }

        public Program(Configuration config)
        {
            ContainerBuilder cb = new();

            cb.RegisterAssemblyVersions(
                GetType().Assembly,
                config.Text["Version"],
                c => c.RegisterInstance(config.Text));

            config.ConfigureStorage(cb, config.Text);

            Container = cb.Build();
        }

        public IContainer Container { get; }

        public InventoryCount Stocks => Container.Resolve<InventoryCount>();

        public ReservationsCoordinator ReservationsCoordinator => Container.Resolve<ReservationsCoordinator>();
    }
}
