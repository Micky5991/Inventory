using System;
using System.Collections.Generic;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Micky5991.Inventory
{
    public class ItemFactory : IItemFactory
    {
        private readonly IItemRegistry registry;
        private readonly IServiceProvider serviceProvider;

        public ItemFactory(IItemRegistry registry, IServiceProvider serviceProvider)
        {
            this.registry = registry;
            this.serviceProvider = serviceProvider;
        }

        public IItem? CreateItem(string handle, int amount)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new ArgumentNullException(nameof(handle));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Item amount has to be 1 or higher");
            }

            if (registry.TryGetItemMeta(handle, out var meta) == false)
            {
                return null;
            }

            if ((meta!.Flags & ItemFlags.NotStackable) != 0 && amount > 1)
            {
                throw new ItemNotStackableException();
            }

            return SetupItemPostCreate(BuildItemFromMeta(meta!), amount);
        }

        public ICollection<IItem>? CreateItems(string handle, int amount)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new ArgumentNullException(nameof(handle));
            }

            if(registry.TryGetItemMeta(handle, out var meta) == false)
            {
                return null;
            }

            return CreateItems(meta!, amount);
        }

        public IItem CreateItem(ItemMeta meta, int amount)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Item amount has to be 1 or higher");
            }

            if ((meta.Flags & ItemFlags.NotStackable) != 0 && amount > 1)
            {
                throw new ItemNotStackableException();
            }

            return SetupItemPostCreate(BuildItemFromMeta(meta), amount);
        }

        public ICollection<IItem> CreateItems(ItemMeta meta, int amount)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            if ((meta.Flags & ItemFlags.NotStackable) == 0)
            {
                return new List<IItem>
                {
                    CreateItem(meta, amount),
                };
            }

            var items = new List<IItem>();

            for (int i = 0; i < amount; i++)
            {
                items.Add(CreateItem(meta, 1));
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
            var factory = (ObjectFactory)serviceProvider.GetService(meta.Type);

            var item = (IItem)factory(serviceProvider, new[] { (object)meta });

            item.Initialize();

            return item;
        }
    }
}
