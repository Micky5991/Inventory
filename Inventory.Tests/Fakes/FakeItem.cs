using System;
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

        public FakeItem(ItemMeta meta)
        {
            RuntimeId = Guid.NewGuid();
            Meta = meta;

            DisplayName = meta.DisplayName;
        }

        public FakeItem(int defaultWeight, string handle = "testitem", string displayName = "FakeItem")
        {
            RuntimeId = Guid.NewGuid();

            Meta = new ItemMeta(handle, typeof(FakeItem), displayName, defaultWeight);
        }

        public void SetDisplayName(string displayName)
        {
            throw new NotImplementedException();
        }
    }
}
