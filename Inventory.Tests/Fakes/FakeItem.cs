using System;
using System.Threading.Tasks;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Tests.Fakes
{
    public class FakeItem : IItem
    {
        public Guid RuntimeId { get; }

        public ItemMeta Meta { get; }

        public string DefaultDisplayName => Meta.DisplayName;

        public string DisplayName { get; set; }

        public string Handle => Meta.Handle;

        public int Weight => Meta.DefaultWeight;

        public bool Stackable { get; } = false;

        public FakeItem(ItemMeta meta)
        {
            RuntimeId = Guid.NewGuid();
            Meta = meta;

            DisplayName = meta.DisplayName;
        }

        public FakeItem(int defaultWeight, string handle = "testitem", string displayName = "FakeItem", ItemFlags flags = ItemFlags.None)
        {
            RuntimeId = Guid.NewGuid();

            Meta = new ItemMeta(handle, typeof(FakeItem), displayName, defaultWeight, flags);
        }

        public void SetDisplayName(string displayName)
        {
            throw new NotImplementedException();
        }

        public bool CanMergeWith(IItem sourceItem)
        {
            throw new NotImplementedException();
        }

        public Task MergeItemAsync(IItem sourceItem)
        {
            throw new NotImplementedException();
        }
    }
}
