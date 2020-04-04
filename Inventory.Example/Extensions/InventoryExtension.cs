using System.Collections.Generic;
using Micky5991.Inventory.Interfaces;

namespace Inventory.Example.Extensions
{
    public static class InventoryExtension
    {
        public static IItem CreateItem(this IItemFactory itemFactory, ItemHandle itemHandle, int amount)
        {
            return itemFactory.CreateItem(itemHandle.ToString(), amount);
        }

        public static ICollection<IItem> GetItems(this IInventory inventory, ItemHandle handle)
        {
            return inventory.GetItems(handle.ToString());
        }
    }
}
