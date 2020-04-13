using System.Collections.Generic;
using Inventory.Example.Items;
using Micky5991.Inventory;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;

namespace Inventory.Example
{
    public class ItemRegistry : BaseItemRegistry
    {
        protected override IEnumerable<ItemMeta> LoadItemMeta()
        {
            yield return this.CreateItemMeta<AppleItem>(ItemHandle.Apple, "Apple");
            yield return this.CreateItemMeta<WaterItem>(ItemHandle.Water, "Water");
            yield return this.CreateItemMeta<DiceItem>(ItemHandle.Dice, "Dice", flags: ItemFlags.NotStackable);
        }

        private ItemMeta CreateItemMeta<T>(ItemHandle itemHandle, string displayName, int defaultWeight = 1, ItemFlags flags = ItemFlags.None) where T : IItem
        {
            return this.CreateItemMeta<T>(itemHandle.ToString(), displayName, defaultWeight, flags);
        }
    }
}
