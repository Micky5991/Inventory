using System.Collections.Generic;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Tests.Fakes
{
    public class ItemRegistry : BaseItemRegistry
    {
        private IList<ItemMeta> _itemMetas = new List<ItemMeta>();

        public int LoadedAmount { get; private set; }

        protected override IEnumerable<ItemMeta> LoadItemMeta()
        {
            this.LoadedAmount++;

            return this._itemMetas;
        }

        public void AddItemMeta(ItemMeta meta)
        {
            this._itemMetas.Add(meta);
        }

        public void SetItemMetasNull()
        {
            this._itemMetas = null;
        }

        public ItemMeta CreateItemMetaForward<T>(string itemHandle, string displayName, int defaultWeight = 1,
            ItemFlags flags = ItemFlags.None) where T : IItem
        {
            return this.CreateItemMeta<T>(itemHandle, displayName, defaultWeight, flags);
        }
    }
}
