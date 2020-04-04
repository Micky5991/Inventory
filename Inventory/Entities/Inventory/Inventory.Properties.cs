using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    internal partial class Inventory
    {
        private readonly ConcurrentDictionary<Guid, IItem> _items;

        private int _capacity;
        private int _usedCapacity;

        public Guid RuntimeId { get; }

        public ICollection<IItem> Items => _items.Values;

        public int Capacity
        {
            get => _capacity;
            private set
            {
                if (value == _capacity)
                {
                    return;
                }

                _capacity = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(AvailableCapacity));
            }
        }

        public int UsedCapacity
        {
            get => _usedCapacity;
            private set
            {
                if (value == _usedCapacity)
                {
                    return;
                }

                _usedCapacity = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(AvailableCapacity));
            }
        }

        public int AvailableCapacity => Capacity - UsedCapacity;

    }
}
