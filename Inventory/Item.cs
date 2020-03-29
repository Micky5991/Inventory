using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    public abstract class Item : IItem
    {
        public string Handle { get; }

        public Guid RuntimeId { get; }

        public ItemMeta Meta { get; }

        public string DefaultDisplayName { get; }

        public string DisplayName { get; private set; }

        public int Weight { get; }

        protected Item(ItemMeta meta)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            RuntimeId = Guid.NewGuid();
            Meta = meta;

            Weight = Meta.DefaultWeight;
            Handle = Meta.Handle;
            DefaultDisplayName = Meta.DisplayName;

            DisplayName = DefaultDisplayName;
        }

        public void SetDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            DisplayName = displayName;
        }
    }
}
