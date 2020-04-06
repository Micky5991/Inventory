using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    public static class InventoryDelegates
    {
        public delegate bool ItemFilterDelegate(IItem item);
    }
}
