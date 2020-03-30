using System;
using Micky5991.Inventory.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Micky5991.Inventory
{
    internal class ItemFactory : IItemFactory
    {
        private readonly IItemRegistry _registry;
        private readonly IServiceProvider _serviceProvider;

        public ItemFactory(IItemRegistry registry, IServiceProvider serviceProvider)
        {
            _registry = registry;
            _serviceProvider = serviceProvider;
        }

        public IItem CreateItem(string handle, int amount)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new ArgumentNullException(nameof(handle));
            }

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Item amount has to be 1 or higher");
            }

            if(_registry.TryGetItemMeta(handle, out var meta) == false)
            {
                return null;
            }

            return BuildItemFromMeta(meta);
        }

        private IItem BuildItemFromMeta(ItemMeta meta)
        {
            var factory = (ObjectFactory) _serviceProvider.GetService(meta.Type);

            var item = (IItem) factory(_serviceProvider, new [] { (object) meta });

            return item;
        }
    }
}
