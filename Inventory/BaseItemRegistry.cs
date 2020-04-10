using System;
using System.Collections.Generic;
using System.Linq;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    public abstract class BaseItemRegistry : IItemRegistry
    {
        private IDictionary<string, ItemMeta>? metaCollection;

        private delegate (bool Valid, string? ErrorMessage) ValidatorDelegate(List<ItemMeta> metaCollection);

        protected abstract IEnumerable<ItemMeta> LoadItemMeta();

        public ICollection<ItemMeta> GetItemMeta()
        {
            this.ValidateAndCacheItemMeta();

            return this.metaCollection!.Values;
        }

        public bool TryGetItemMeta(string handle, out ItemMeta? meta)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new ArgumentNullException(nameof(handle));
            }

            this.ValidateAndCacheItemMeta();

            return this.metaCollection!.TryGetValue(handle, out meta);
        }

        protected ItemMeta CreateItemMeta<T>(string itemHandle, string displayName, int defaultWeight = 1, ItemFlags flags = ItemFlags.None)
            where T : IItem
        {
            if (string.IsNullOrWhiteSpace(itemHandle))
            {
                throw new ArgumentNullException(nameof(itemHandle));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            return new ItemMeta(itemHandle, typeof(T), displayName, defaultWeight, flags);
        }

        public void ValidateAndCacheItemMeta()
        {
            if (this.metaCollection != null)
            {
                return;
            }

            var createdMeta = this.LoadItemMeta();
            if (createdMeta == null)
            {
                throw new InvalidItemRegistryException($"The method {nameof(this.LoadItemMeta)} returns an null enumerable instance!");
            }

            var loadedMeta = new List<ItemMeta>(createdMeta.Where(x => x != null));

            this.ValidateItemRegistry(loadedMeta);

            this.metaCollection = loadedMeta.ToDictionary(x => x.Handle, x => x);
        }

        private void ValidateItemRegistry(List<ItemMeta> metaCollection)
        {
            var validators = new List<ValidatorDelegate>
            {
                this.ValidateUniqueHandles
            };

            foreach (var validator in validators)
            {
                var (valid, errorMessage) = validator(metaCollection);

                if (valid == false)
                {
                    throw new InvalidItemRegistryException(errorMessage!);
                }
            }
        }

        private (bool Valid, string? ErrorMessage) ValidateUniqueHandles(List<ItemMeta> metaCollection)
        {
            if (metaCollection.Select(x => x.Handle).Distinct().Count() != metaCollection.Count)
            {
                return (false, "There are duplicate handles inside the registered items!");
            }

            return (true, null);
        }
    }
}
