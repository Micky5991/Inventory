using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    public partial class Inventory : IInventory
    {
        internal const int MinimalInventoryCapacity = 0;

        private readonly IItemRegistry itemRegistry;

        private InventoryDelegates.ItemFilterDelegate? itemFilter;

        public Inventory(int capacity, AggregatedInventoryServices inventoryServices)
        {
            if (capacity < MinimalInventoryCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"The capacity has to be {MinimalInventoryCapacity} or higher");
            }

            itemRegistry = inventoryServices.ItemRegistry;

            items = new ConcurrentDictionary<Guid, IItem>();

            RuntimeId = Guid.NewGuid();
            UsedCapacity = 0;
            Capacity = capacity;
        }

        private void RecalculateWeight()
        {
            UsedCapacity = items.Values.Sum(x => x.TotalWeight);
        }

        public bool IsItemAllowed(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (itemFilter == null)
            {
                return true;
            }

            return itemFilter(item);
        }

        public bool DoesItemFit(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return AvailableCapacity >= item.TotalWeight;
        }

        public bool CanBeInserted(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return DoesItemFit(item) && IsItemAllowed(item) && item.MovingLocked == false;
        }

        public bool DoesItemFit(string handle, int amount = 1)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new ArgumentNullException(nameof(handle));
            }

            if (itemRegistry.TryGetItemMeta(handle, out var meta) == false)
            {
                throw new ItemMetaNotFoundException($"Could not find the given handle {handle}");
            }

            return DoesItemFit(meta!, amount);
        }

        public bool DoesItemFit(ItemMeta meta, int amount = 1)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            if (amount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), $"The given {nameof(amount)} has to be 1 or higher");
            }

            var neededSpace = meta.DefaultWeight * amount;

            return AvailableCapacity >= neededSpace;
        }

        public bool SetCapacity(int capacity)
        {
            if (capacity < MinimalInventoryCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"The capacity has to be {MinimalInventoryCapacity} or higher");
            }

            if (UsedCapacity > capacity)
            {
                return false;
            }

            Capacity = capacity;

            return true;
        }

        public int GetItemFitAmount(string handle)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new ArgumentNullException(nameof(handle));
            }

            if (itemRegistry.TryGetItemMeta(handle, out var meta) == false)
            {
                throw new ItemMetaNotFoundException($"Could not find item meta with handle for handle {handle}");
            }

            return GetItemFitAmount(meta!);
        }

        public int GetItemFitAmount(ItemMeta meta)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            return GetItemFitAmount(meta.DefaultWeight);
        }

        public int GetItemFitAmount(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return GetItemFitAmount(item.SingleWeight);
        }

        public int GetItemFitAmount(int itemWeight)
        {
            if (itemWeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(itemWeight), $"{nameof(itemWeight)} has to be 1 or higher");
            }

            return Math.DivRem(AvailableCapacity, itemWeight, out _);
        }

        public ICollection<IItem> GetItems(string handle)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new ArgumentNullException(nameof(handle));
            }

            return new List<IItem>(Items.Where(x => x.Handle == handle));
        }

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

            return new List<T>(Items.Where(IncludeItemCheck).Select(x => (T)x));
        }
    }
}
