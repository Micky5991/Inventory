using System;
using System.Collections.Generic;
using CommunityToolkit.Diagnostics;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Micky5991.Inventory.Entities.Factories
{
    /// <inheritdoc />
    public class ItemFactory : IItemFactory
    {
        private readonly IItemRegistry registry;
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemFactory"/> class.
        /// </summary>
        /// <param name="registry">Needed registry-service reference.</param>
        /// <param name="serviceProvider">Needed serviceprovider reference.</param>
        public ItemFactory(IItemRegistry registry, IServiceProvider serviceProvider)
        {
            Guard.IsNotNull(registry);
            Guard.IsNotNull(serviceProvider);

            this.registry = registry;
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public IItem? CreateItem(string handle, int amount)
        {
            Guard.IsNotNullOrWhiteSpace(handle);
            Guard.IsGreaterThanOrEqualTo(amount, 1);

            if (this.registry.TryGetItemMeta(handle, out var meta) == false)
            {
                return null;
            }

            if ((meta!.Flags & ItemFlags.NotStackable) != 0 && amount > 1)
            {
                throw new ItemNotStackableException();
            }

            return this.SetupItemPostCreate(this.BuildItemFromMeta(meta!), amount);
        }

        /// <inheritdoc/>
        public ICollection<IItem>? CreateItems(string handle, int amount)
        {
            Guard.IsNotNullOrWhiteSpace(handle);

            if (this.registry.TryGetItemMeta(handle, out var meta) == false)
            {
                return null;
            }

            return this.CreateItems(meta!, amount);
        }

        /// <inheritdoc/>
        public IItem CreateItem(ItemMeta meta, int amount)
        {
            Guard.IsNotNull(meta);
            Guard.IsGreaterThanOrEqualTo(amount, 1);

            if ((meta.Flags & ItemFlags.NotStackable) != 0 && amount > 1)
            {
                throw new ItemNotStackableException();
            }

            return this.SetupItemPostCreate(this.BuildItemFromMeta(meta), amount);
        }

        /// <inheritdoc/>
        public ICollection<IItem> CreateItems(ItemMeta meta, int amount)
        {
            Guard.IsNotNull(meta);

            if ((meta.Flags & ItemFlags.NotStackable) == 0)
            {
                return new List<IItem>
                {
                    this.CreateItem(meta, amount),
                };
            }

            var items = new List<IItem>();

            for (int i = 0; i < amount; i++)
            {
                items.Add(this.CreateItem(meta, 1));
            }

            return items;
        }

        private IItem SetupItemPostCreate(IItem item, int amount)
        {
            item.SetAmount(amount);

            return item;
        }

        private IItem BuildItemFromMeta(ItemMeta meta)
        {
            var factory = (ObjectFactory)this.serviceProvider.GetService(meta.Type);

            var item = (IItem)factory(this.serviceProvider, new[] { (object)meta });

            item.Initialize();

            return item;
        }
    }
}
