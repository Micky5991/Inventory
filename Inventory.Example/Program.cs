using System;
using System.Threading.Tasks;
using Inventory.Example.Services;
using Micky5991.Inventory.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Example
{
    class Program
    {
        private readonly ServiceProvider _serviceProvider;

        static async Task Main(string[] args)
        {
            var program = new Program();

            await program.RunInventoryAsync();
        }

        public Program()
        {
            _serviceProvider = SetupServices();
        }

        private ServiceProvider SetupServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddDefaultInventoryServices()
                .AddItemTypes(new ItemRegistry());

            serviceCollection
                .AddTransient<InventoryBuilderService>();

            return serviceCollection.BuildServiceProvider();
        }

        private async Task RunInventoryAsync()
        {
            var inventoryBuilder = _serviceProvider.GetService<InventoryBuilderService>();

            await inventoryBuilder.SetupInventoryAsync();
        }
    }
}
