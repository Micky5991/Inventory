using System;
using System.Linq;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Factories;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;
using Micky5991.Inventory.Strategies;
using Microsoft.Extensions.DependencyInjection;

namespace Micky5991.Inventory.Extensions
{
    /// <summary>
    /// Extensions which simplify the usage of this library.
    /// </summary>
    public static class InventoryDependencyExtensions
    {
        /// <summary>
        /// Registers all default inventory and item services.
        ///
        /// Calls the following extensions:
        /// <see cref="AddDefaultFactories"/>,
        /// <see cref="AddDefaultInventoryStrategies"/>,
        /// <see cref="AddInventoryServices"/>.
        /// </summary>
        /// <param name="serviceCollection">Collection to register the services to.</param>
        /// <returns><paramref name="serviceCollection"/> that has been passed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
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

        /// <summary>
        /// Registers all needed core inventory and item services.
        ///
        /// These services have to be registered regardless of usage.
        /// </summary>
        /// <param name="serviceCollection">Collection to register the services to.</param>
        /// <returns><paramref name="serviceCollection"/> that has been passed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
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

        /// <summary>
        /// Registers all default inventory strategies to <see cref="IServiceCollection"/>.
        ///
        /// Calls the following extensions:
        /// <see cref="AddDefaultInventoryMergeStrategy"/>,
        /// <see cref="AddDefaultInventorySplitStrategy"/>.
        /// </summary>
        /// <param name="serviceCollection">Collection to register the services to.</param>
        /// <returns><paramref name="serviceCollection"/> that has been passed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
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

        /// <summary>
        /// Registers the following services:
        /// <see cref="ItemMergeStrategyHandler"/> as <see cref="IItemMergeStrategyHandler"/>.
        /// </summary>
        /// <param name="serviceCollection">Collection to register the services to.</param>
        /// <returns><paramref name="serviceCollection"/> that has been passed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        public static IServiceCollection AddDefaultInventoryMergeStrategy(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddTransient<IItemMergeStrategyHandler, ItemMergeStrategyHandler>();
        }

        /// <summary>
        /// Registers the following services:
        /// <see cref="ItemSplitStrategyHandler"/> as <see cref="IItemSplitStrategyHandler"/>.
        /// </summary>
        /// <param name="serviceCollection">Collection to register the services to.</param>
        /// <returns><paramref name="serviceCollection"/> that has been passed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        public static IServiceCollection AddDefaultInventorySplitStrategy(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddTransient<IItemSplitStrategyHandler, ItemSplitStrategyHandler>();
        }

        /// <summary>
        /// Registers all default inventory and item factories.
        ///
        /// Calls the following extensions:
        /// <see cref="AddDefaultInventoryFactory"/>,
        /// <see cref="AddDefaultItemFactory"/>.
        /// </summary>
        /// <param name="serviceCollection">Collection to register the services to.</param>
        /// <returns><paramref name="serviceCollection"/> that has been passed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
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

        /// <summary>
        /// Registers default <see cref="IInventoryFactory"/>.
        ///
        /// Registers the following services:
        /// <see cref="InventoryFactory"/> as <see cref="IInventoryFactory"/>.
        /// </summary>
        /// <param name="serviceCollection">Collection to register the services to.</param>
        /// <returns><paramref name="serviceCollection"/> that has been passed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        public static IServiceCollection AddDefaultInventoryFactory(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddTransient<IInventoryFactory, InventoryFactory>();
        }

        /// <summary>
        /// Registers default <see cref="IItemFactory"/>
        ///
        /// Registers the following services:
        /// <see cref="ItemFactory"/> as <see cref="IItemFactory"/>.
        /// </summary>
        /// <param name="serviceCollection">Collection to register the services to.</param>
        /// <returns><paramref name="serviceCollection"/> that has been passed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> is null.</exception>
        public static IServiceCollection AddDefaultItemFactory(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            return serviceCollection
                .AddTransient<IItemFactory, ItemFactory>();
        }

        /// <summary>
        /// Reads the given <paramref name="itemRegistry"/> and registers all <see cref="ItemMeta"/> instances to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection"><see cref="IServiceCollection"/> to register all <see cref="ItemMeta"/> to.</param>
        /// <param name="itemRegistry"><see cref="IItemRegistry"/> that will be read.</param>
        /// <returns><see cref="IServiceCollection"/> that was passed by <paramref name="serviceCollection"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="serviceCollection"/> or <paramref name="itemRegistry"/> is null.</exception>
        /// <exception cref="InvalidItemRegistryException"><paramref name="itemRegistry"/> is not valid. See exception for more details.</exception>
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

            if (metaCollection.Contains(null!))
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
