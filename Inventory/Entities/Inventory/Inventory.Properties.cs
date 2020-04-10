using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    public partial class Inventory
    {
        private readonly ConcurrentDictionary<Guid, IItem> items;

        private int capacity;
        private int usedCapacity;

        public Guid RuntimeId { get; }

        public ICollection<IItem> Items => items.Values;

        public int Capacity
        {
            get => capacity;
            private set
            {
                if (value == capacity)
                {
                    return;
                }

                capacity = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(AvailableCapacity));
            }
        }

        public int UsedCapacity
        {
            get => usedCapacity;
            private set
            {
                if (value == usedCapacity)
                {
                    return;
                }

                usedCapacity = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(AvailableCapacity));
            }
        }

        public int AvailableCapacity => Capacity - UsedCapacity;
    }
}
