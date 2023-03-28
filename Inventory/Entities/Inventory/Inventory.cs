using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    /// <inheritdoc />
    public partial class Inventory : IInventory
    {
        internal const int MinimalInventoryCapacity = 0;

        private readonly IItemRegistry itemRegistry;

        private InventoryDelegates.ItemFilterDelegate? itemFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Inventory"/> class.
        /// </summary>
        /// <param name="capacity">Capacity of this inventory.</param>
        /// <param name="inventoryServices">Needed services of this inventory.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is too low.</exception>
        public Inventory(int capacity, AggregatedInventoryServices inventoryServices)
        {
            Guard.IsNotNull(inventoryServices);
            Guard.IsGreaterThanOrEqualTo(capacity, MinimalInventoryCapacity);

            this.itemRegistry = inventoryServices.ItemRegistry;

            this.items = new ConcurrentDictionary<Guid, IItem>();

            this.RuntimeId = Guid.NewGuid();
            this.UsedCapacity = 0;
            this.Capacity = capacity;
        }

        /// <inheritdoc/>
        public bool IsItemAllowed(IItem item)
        {
            Guard.IsNotNull(item);

            if (this.itemFilter == null)
            {
                return true;
            }

            return this.itemFilter(item);
        }

        /// <inheritdoc/>
        public bool DoesItemFit(IItem item)
        {
            Guard.IsNotNull(item);

            return this.AvailableCapacity >= item.TotalWeight;
        }

        /// <inheritdoc/>
        public bool CanBeInserted(IItem item)
        {
            Guard.IsNotNull(item);

            return this.DoesItemFit(item) && this.IsItemAllowed(item) && item.MovingLocked == false;
        }

        /// <inheritdoc/>
        public bool DoesItemFit(string handle, int amount = 1)
        {
            Guard.IsNotNullOrWhiteSpace(handle);

            if (this.itemRegistry.TryGetItemMeta(handle, out var meta) == false)
            {
                throw new ItemMetaNotFoundException($"Could not find the given handle {handle}");
            }

            return this.DoesItemFit(meta!, amount);
        }

        /// <inheritdoc />
        public bool DoesItemFit(ItemMeta meta, int amount = 1)
        {
            Guard.IsNotNull(meta);
            Guard.IsGreaterThanOrEqualTo(amount, 1);

            var neededSpace = meta.DefaultWeight * amount;

            return this.AvailableCapacity >= neededSpace;
        }

        /// <inheritdoc />
        public bool SetCapacity(int newCapacity)
        {
            Guard.IsGreaterThanOrEqualTo(newCapacity, MinimalInventoryCapacity);

            if (this.UsedCapacity > newCapacity)
            {
                return false;
            }

            this.Capacity = newCapacity;

            return true;
        }

        /// <inheritdoc/>
        public int GetItemFitAmount(string handle)
        {
            Guard.IsNotNullOrWhiteSpace(handle);

            if (this.itemRegistry.TryGetItemMeta(handle, out var meta) == false)
            {
                throw new ItemMetaNotFoundException($"Could not find item meta with handle for handle {handle}");
            }

            return this.GetItemFitAmount(meta!);
        }

        /// <inheritdoc/>
        public int GetItemFitAmount(ItemMeta meta)
        {
            Guard.IsNotNull(meta);

            return this.GetItemFitAmount(meta.DefaultWeight);
        }

        /// <inheritdoc/>
        public int GetItemFitAmount(IItem item)
        {
            Guard.IsNotNull(item);

            return this.GetItemFitAmount(item.SingleWeight);
        }

        /// <inheritdoc/>
        public int GetItemFitAmount(int itemWeight)
        {
            Guard.IsGreaterThanOrEqualTo(itemWeight, 1);

            return Math.DivRem(this.AvailableCapacity, itemWeight, out _);
        }

        /// <inheritdoc/>
        public ICollection<IItem> GetItems(string handle)
        {
            Guard.IsNotNullOrWhiteSpace(handle);

            return new List<IItem>(this.Items.Where(x => x.Handle == handle));
        }

        /// <inheritdoc />
        public ICollection<T> GetItems<T>(string? handle = null)
            where T : IItem
        {
            bool IncludeItemCheck(IItem item)
            {
                if (item is T == false)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(handle) == false && item.Handle != handle)
                {
                    return false;
                }

                return true;
            }

            return new List<T>(this.Items.Where(IncludeItemCheck).Select(x => (T)x));
        }

        /// <inheritdoc />
        public IItem? GetItem(Guid runtimeId)
        {
            Guard.IsNotEqualTo(runtimeId, Guid.Empty);

            if (this.items.TryGetValue(runtimeId, out var item) == false)
            {
                return default;
            }

            return item;
        }

        /// <inheritdoc />
        public T? GetItem<T>(Guid runtimeId)
            where T : class, IItem
        {
            Guard.IsNotEqualTo(runtimeId, Guid.Empty);

            var item = this.GetItem(runtimeId);

            if (item != default(IItem) && item is T convertedItem)
            {
                return convertedItem;
            }

            return default;
        }

        private void RecalculateWeight()
        {
            this.UsedCapacity = this.items.Values.Sum(x => x.TotalWeight);
        }
    }
}
