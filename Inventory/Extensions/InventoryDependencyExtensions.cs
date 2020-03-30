using System.Linq;
using Micky5991.Inventory.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Micky5991.Inventory.Extensions
{
    public static class InventoryDependencyExtensions
    {

        public static IServiceCollection AddInventoryServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddTransient<IInventoryFactory, InventoryFactory>();
        }

        public static IServiceCollection AddItemTypes(this IServiceCollection serviceCollection, IItemRegistry itemRegistry)
        {
            var uniqueTypes = itemRegistry.GetItemMeta().Select(x => x.Type).Distinct();

            foreach (var itemType in uniqueTypes)
            {
                serviceCollection.AddTransient(itemType);
            }

            serviceCollection.AddTransient(x => itemRegistry);

            return serviceCollection;
        }

    }
}
