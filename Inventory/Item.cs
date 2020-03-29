using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micky5991.Inventory.Enums;
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

        public int TotalWeight { get; }

        public bool Stackable { get; }

        protected Item(ItemMeta meta)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            RuntimeId = Guid.NewGuid();
            Meta = meta;

            TotalWeight = Meta.DefaultWeight;
            Handle = Meta.Handle;
            DefaultDisplayName = Meta.DisplayName;
            Stackable = (Meta.Flags & ItemFlags.NotStackable) == 0;

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

        public bool CanMergeWith(IItem sourceItem)
        {
            if (sourceItem == this)
            {
                return false;
            }

            return Handle == sourceItem.Handle && Stackable && sourceItem.Stackable;
        }

        public Task MergeItemAsync(IItem sourceItem)
        {
            throw new NotImplementedException();
        }
    }
}
