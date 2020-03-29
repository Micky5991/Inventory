using System;
using System.Runtime.CompilerServices;
using Micky5991.Inventory.Interfaces;

[assembly:InternalsVisibleTo("Micky5991.Inventory.Tests")]

namespace Micky5991.Inventory
{
    internal class InventoryFactory : IInventoryFactory
    {

        public IInventory CreateInventory(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            return new Inventory(capacity);
        }

    }
}
