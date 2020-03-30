using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    internal partial class Inventory : IInventory
    {
        internal const int MinimalInventoryCapacity = 0;

        internal Inventory(int capacity)
        {
            if (capacity < MinimalInventoryCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), $"The capacity has to be {MinimalInventoryCapacity} or higher");
            }

            _items = new ConcurrentDictionary<Guid, IItem>();
            Items = _items;

            UsedCapacity = 0;
            Capacity = capacity;
        }

        private void RecalculateWeight()
        {
            UsedCapacity = _items.Values.Sum(x => x.TotalWeight);
        }

        public bool DoesItemFit(IItem item)
        {
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
    }
}
