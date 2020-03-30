using System;
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
                .AddTransient<IInventoryFactory, InventoryFactory>()
                .AddTransient<IItemFactory>(x => new ItemFactory(x.GetService<IItemRegistry>(), x));
        }

        public static IServiceCollection AddItemTypes(this IServiceCollection serviceCollection, IItemRegistry itemRegistry)
        {
            if (itemRegistry == null)
            {
                throw new ArgumentNullException(nameof(itemRegistry));
            }

            serviceCollection.AddTransient(x => itemRegistry);

            var uniqueTypes = itemRegistry.GetItemMeta().Select(x => x.Type).Distinct();

            foreach (var itemType in uniqueTypes)
            {
                serviceCollection.AddTransient(itemType, x => ActivatorUtilities.CreateFactory(itemType, new[] { typeof(ItemMeta) }));
            }

            return serviceCollection;
        }

    }
}
