using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    public class Item : IItem
    {
        public string Handle { get; }

        public Guid RuntimeId { get; }

        public ItemMeta Meta { get; }

        public int Weight { get; }

        public Item(Guid runtimeId, ItemMeta meta)
        {
            RuntimeId = runtimeId;
            Meta = meta;

            Weight = Meta.DefaultWeight;
            Handle = Meta.Handle;
        }
    }
}
