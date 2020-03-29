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

        public int Amount { get; private set; }

        public int SingleWeight { get; }

        public string Handle => Meta.Handle;

        public int TotalWeight => Meta.DefaultWeight;

        public bool Stackable { get; } = false;

        public IInventory? CurrentInventory { get; private set; }

        public Func<IItem, bool> IsMergableCheck { get; }

        public FakeItem(ItemMeta meta)
        {
            RuntimeId = Guid.NewGuid();
            Meta = meta;

            DisplayName = meta.DisplayName;
            Amount = 1;
        }

        public FakeItem(int defaultWeight, string handle = "testitem", string displayName = "FakeItem", ItemFlags flags = ItemFlags.None)
        {
            RuntimeId = Guid.NewGuid();

            Meta = new ItemMeta(handle, typeof(FakeItem), displayName, defaultWeight, flags);
        }

        public void SetCurrentInventory(IInventory? inventory)
        {
            CurrentInventory = inventory;
        }

        public void SetAmount(int newAmount)
        {
            Amount = newAmount;
        }

        public void SetDisplayName(string displayName)
        {
            throw new NotImplementedException();
        }

        public bool CanMergeWith(IItem sourceItem)
        {
            return IsMergableCheck == null || IsMergableCheck(sourceItem);
        }

        public Task MergeItemAsync(IItem sourceItem)
        {
            throw new NotImplementedException();
        }
    }
}
