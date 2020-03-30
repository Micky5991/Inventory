using System.Collections.Generic;
using Inventory.Example.Items;
using Micky5991.Inventory;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;

namespace Inventory.Example
{
    public class ItemRegistry : IItemRegistry
    {
        public ICollection<ItemMeta> GetItemMeta()
        {
            return new []
            {
                AddItem<AppleItem>(ItemHandle.Apple, "Apple"),
                AddItem<WaterItem>(ItemHandle.Water, "Water"),
                AddItem<DiceItem>(ItemHandle.Dice, "Dice"),
            };
        }

        private ItemMeta AddItem<T>(ItemHandle itemHandle, string displayName, int defaultWeight = 1, ItemFlags flags = ItemFlags.None) where T : BaseItem
        {
            return new ItemMeta(itemHandle.ToString(), typeof(T), displayName, defaultWeight, flags);
        }
    }
}
