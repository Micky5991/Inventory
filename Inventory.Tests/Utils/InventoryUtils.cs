using System;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Tests.Fakes;

namespace Micky5991.Inventory.Tests.Utils
{
    public static class InventoryUtils
    {

        public static ItemMeta CreateItemMeta(string itemHandle = "testhandle", Type itemType = null,
            int defaultWeight = 10, ItemFlags flags = ItemFlags.None)
        {
            if (itemType == null)
            {
                itemType = typeof(FakeItem);
            }

            return new ItemMeta(itemHandle, itemType, defaultWeight, flags);
        }

    }
}
