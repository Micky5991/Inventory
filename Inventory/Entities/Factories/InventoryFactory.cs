using System;
using System.Runtime.CompilerServices;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Interfaces;

[assembly:InternalsVisibleTo("Micky5991.Inventory.Tests")]

namespace Micky5991.Inventory.Entities.Factories
{
    internal class InventoryFactory : IInventoryFactory
    {
        private readonly AggregatedInventoryServices inventoryServices;

        public InventoryFactory(AggregatedInventoryServices inventoryServices)
        {
            this.inventoryServices = inventoryServices;
        }

        public IInventory CreateInventory(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            return new Entities.Inventory.Inventory(capacity, this.inventoryServices);
        }
    }
}
