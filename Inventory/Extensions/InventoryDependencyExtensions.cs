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

        public static IServiceCollection AddDefaultInventoryServices(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddDefaultFactories()
                .AddDefaultInventoryStrategies()
                .AddInventoryServices();
        }

        public static IServiceCollection AddInventoryServices(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddTransient<AggregatedItemServices>()
                .AddTransient<AggregatedInventoryServices>();
        }

        public static IServiceCollection AddDefaultInventoryStrategies(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddDefaultInventoryMergeStrategy()
                .AddDefaultInventorySplitStrategy();
        }

        public static IServiceCollection AddDefaultInventoryMergeStrategy(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddTransient<IItemMergeStrategyHandler, ItemMergeStrategyHandler>();
        }

        public static IServiceCollection AddDefaultInventorySplitStrategy(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddTransient<IItemSplitStrategyHandler, ItemSplitStrategyHandler>();
        }

        public static IServiceCollection AddDefaultFactories(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddDefaultInventoryFactory()
                .AddDefaultItemFactory();
        }

        public static IServiceCollection AddDefaultInventoryFactory(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddTransient<IInventoryFactory, InventoryFactory>();
        }

        public static IServiceCollection AddDefaultItemFactory(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddTransient<IItemFactory, ItemFactory>();
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
