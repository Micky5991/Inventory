using System.Collections.Generic;
using Inventory.Example.Items;
using Micky5991.Inventory;
using Micky5991.Inventory.Enums;

namespace Inventory.Example
{
    public class ItemRegistry : BaseItemRegistry
    {
        protected override IEnumerable<ItemMeta> LoadItemMeta()
        {
            yield return CreateItemMeta<AppleItem>(ItemHandle.Apple, "Apple");
            yield return CreateItemMeta<WaterItem>(ItemHandle.Water, "Water");
            yield return CreateItemMeta<DiceItem>(ItemHandle.Dice, "Dice");
        }

        private ItemMeta CreateItemMeta<T>(ItemHandle itemHandle, string displayName, int defaultWeight = 1, ItemFlags flags = ItemFlags.None) where T : BaseItem
        {
            return CreateItemMeta<T>(itemHandle.ToString(), displayName, defaultWeight, flags);
        }
    }
}