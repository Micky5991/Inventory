using System;
using System.Collections.Generic;
using System.Linq;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    /// <inheritdoc />
    public abstract class BaseItemRegistry : IItemRegistry
    {
        private IDictionary<string, ItemMeta>? metaCollection;

        private delegate (bool Valid, string? ErrorMessage) ValidatorDelegate(List<ItemMeta> metaCollection);

        /// <inheritdoc />
        public ICollection<ItemMeta> GetItemMeta()
        {
            this.ValidateAndCacheItemMeta();

            return this.metaCollection!.Values;
        }

        /// <inheritdoc />
        public bool TryGetItemMeta(string handle, out ItemMeta? meta)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new ArgumentNullException(nameof(handle));
            }

            this.ValidateAndCacheItemMeta();

            return this.metaCollection!.TryGetValue(handle, out meta);
        }

        /// <summary>
        /// Validates the supplied <see cref="ItemMeta"/> and throws exception if invalid.
        /// </summary>
        /// <exception cref="InvalidItemRegistryException">Any supplied <see cref="ItemMeta"/> was invalid.</exception>
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

        /// <summary>
        /// Executes a single initialization of all <see cref="ItemMeta"/> instances.
        /// </summary>
        /// <returns>Returns the list of created <see cref="ItemMeta"/>.</returns>
        protected abstract IEnumerable<ItemMeta> LoadItemMeta();

        /// <summary>
        /// Simple utility function to create an <see cref="ItemMeta"/> instance.
        /// </summary>
        /// <param name="itemHandle">Unique handle which identifies this <see cref="ItemMeta"/>.</param>
        /// <param name="displayName">Default display name of this item.</param>
        /// <param name="defaultWeight">Default single-weight of this item.</param>
        /// <param name="flags">Flags which specify certain behavor aspects.</param>
        /// <typeparam name="T">ChildClass that represents this <see cref="ItemMeta"/>.</typeparam>
        /// <returns>The created <see cref="ItemMeta"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="itemHandle"/>, <paramref name="displayName"/> is null.</exception>
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

        private void ValidateItemRegistry(List<ItemMeta> collection)
        {
            var validators = new List<ValidatorDelegate>
            {
                this.ValidateUniqueHandles,
            };

            foreach (var validator in validators)
            {
                var (valid, errorMessage) = validator(collection);

                if (valid == false)
                {
                    throw new InvalidItemRegistryException(errorMessage!);
                }
            }
        }

        private (bool Valid, string? ErrorMessage) ValidateUniqueHandles(List<ItemMeta> collection)
        {
            if (collection.Select(x => x.Handle).Distinct().Count() != collection.Count)
            {
                return (false, "There are duplicate handles inside the registered items!");
            }

            return (true, null);
        }
    }
}
