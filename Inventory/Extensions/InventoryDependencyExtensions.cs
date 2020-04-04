using System;
using System.Linq;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;
using Micky5991.Inventory.Strategies.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Micky5991.Inventory.Extensions
{
    public static class InventoryDependencyExtensions
    {

        public static IServiceCollection AddInventoryServices(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddTransient<IInventoryFactory, InventoryFactory>()
                .AddTransient<IItemFactory>(x => new ItemFactory(x.GetRequiredService<IItemRegistry>(), x))

                .AddTransient<IItemMergeStrategyHandler, ItemMergeStrategyHandler>()
                .AddTransient<IItemSplitStrategyHandler, ItemSplitStrategyHandler>()
                .AddTransient<AggregatedItemServices>();
        }

        public static IServiceCollection AddItemTypes(this IServiceCollection serviceCollection, IItemRegistry itemRegistry)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (itemRegistry == null)
            {
                throw new ArgumentNullException(nameof(itemRegistry));
            }

            serviceCollection.AddTransient(x => itemRegistry);

            var metaCollection = itemRegistry.GetItemMeta();
            if (metaCollection == null)
            {
                throw new InvalidItemRegistryException($"The method {itemRegistry.GetType()}.{nameof(IItemRegistry.GetItemMeta)} returns a null enumerable instance!");
            }

            if (metaCollection.Contains(null))
            {
                throw new InvalidItemRegistryException($"The method {itemRegistry.GetType()}.{nameof(IItemRegistry.GetItemMeta)} contains a null value inside the collection");
            }

            var uniqueTypes = metaCollection.Select(x => x.Type).Distinct();
            foreach (var itemType in uniqueTypes)
            {
                serviceCollection.AddTransient(itemType, x => ActivatorUtilities.CreateFactory(itemType, new[] { typeof(ItemMeta) }));
            }

            return serviceCollection;
        }

    }
}
