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

        public ICollection<IItem> Items => this.items.Values;

        public int Capacity
        {
            get => this.capacity;
            private set
            {
                if (value == this.capacity)
                {
                    return;
                }

                this.capacity = value;

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.AvailableCapacity));
            }
        }

        public int UsedCapacity
        {
            get => this.usedCapacity;
            private set
            {
                if (value == this.usedCapacity)
                {
                    return;
                }

                this.usedCapacity = value;

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.AvailableCapacity));
            }
        }

        public int AvailableCapacity => this.Capacity - this.UsedCapacity;
    }
}
