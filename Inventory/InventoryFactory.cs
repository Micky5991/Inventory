using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    internal class InventoryFactory : IInventoryFactory
    {

        public IInventory CreateInventory(int capacity)
        {
            return new Inventory(capacity);
        }

    }
}
