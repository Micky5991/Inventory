using System;
using System.Runtime.CompilerServices;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Interfaces;

[assembly:InternalsVisibleTo("Micky5991.Inventory.Tests")]

namespace Micky5991.Inventory
{
    internal class InventoryFactory : IInventoryFactory
    {
        private readonly AggregatedInventoryServices _inventoryServices;

        public InventoryFactory(AggregatedInventoryServices inventoryServices)
        {
            _inventoryServices = inventoryServices;
        }

        public IInventory CreateInventory(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            return new Entities.Inventory.Inventory(capacity, _inventoryServices);
        }

    }
}
