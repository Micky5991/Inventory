using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    public class Item : IItem
    {
        public Guid RuntimeId { get; }

        public ItemMeta Meta { get; }

        public Item(Guid runtimeId, ItemMeta meta)
        {
            RuntimeId = runtimeId;
            Meta = meta;
        }
    }
}
