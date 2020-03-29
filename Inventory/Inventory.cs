﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    internal partial class Inventory : IInventory
    {
        public IReadOnlyDictionary<Guid, IItem> Items { get; }

        public int Capacity { get; private set; }

        public int UsedCapacity { get; private set; }

        public int AvailableCapacity => Capacity - UsedCapacity;

        private readonly ConcurrentDictionary<Guid, IItem> _items;

        internal Inventory(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _items = new ConcurrentDictionary<Guid, IItem>();
            Items = _items;

            UsedCapacity = 0;
            Capacity = capacity;
        }

        private void RecalculateWeight()
        {
            UsedCapacity = _items.Values.Sum(x => x.Weight);
        }

    }
}