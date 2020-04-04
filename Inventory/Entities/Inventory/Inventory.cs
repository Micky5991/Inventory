using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    public partial class Inventory : IInventory
    {
        internal const int MinimalInventoryCapacity = 0;

        private readonly AggregatedInventoryServices _inventoryServices;

        public Inventory(int capacity, AggregatedInventoryServices inventoryServices)
        {
            if (capacity < MinimalInventoryCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"The capacity has to be {MinimalInventoryCapacity} or higher");
            }

            _inventoryServices = inventoryServices;

            _items = new ConcurrentDictionary<Guid, IItem>();

            RuntimeId = Guid.NewGuid();
            UsedCapacity = 0;
            Capacity = capacity;
        }

        private void RecalculateWeight()
        {
            UsedCapacity = _items.Values.Sum(x => x.TotalWeight);
        }

        public bool DoesItemFit(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return AvailableCapacity >= item.TotalWeight;
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

        public ICollection<IItem> GetItems(string handle)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new ArgumentNullException(nameof(handle));
            }

            return new List<IItem>(Items.Where(x => x.Handle == handle));
        }

        public ICollection<T> GetItems<T>(string? handle = null) where T : IItem
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

            return new List<T>( Items.Where(IncludeItemCheck).Select(x => (T) x));
        }
    }
}
