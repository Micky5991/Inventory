using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.EventArgs;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Tests.Fakes
{
    public class FakeItem : IItem
    {
        public event EventHandler<ItemInitializedEventArgs> Initialized;

        public Guid RuntimeId { get; set; }

        public ItemMeta Meta { get; }

        public string DefaultDisplayName => this.Meta.DisplayName;

        public string DisplayName { get; set; }

        public int Amount { get; private set; }

        public int SingleWeight { get; set; }

        public string Handle => this.Meta.Handle;

        public int TotalWeight => this.Meta.DefaultWeight;

        public bool Stackable { get; set; } = true;

        public IInventory CurrentInventory { get; private set; }
        public bool MovingLocked { get; set; }
        public bool Locked { get; set; }

        public Func<IItem, bool> IsMergableCheck { get; set; }

        public FakeItem(ItemMeta meta)
        {
            this.RuntimeId = Guid.NewGuid();
            this.Meta = meta;

            this.DisplayName = meta.DisplayName;
            this.Amount = 1;
            this.Stackable = (this.Meta.Flags & ItemFlags.NotStackable) == 0;
            this.SingleWeight = this.Meta.DefaultWeight;
        }

        public FakeItem(int defaultWeight, string handle = "testitem", string displayName = "FakeItem", ItemFlags flags = ItemFlags.None)
        {
            this.RuntimeId = Guid.NewGuid();

            this.Meta = new ItemMeta(handle, typeof(FakeItem), displayName, defaultWeight, flags);

            this.Stackable = (this.Meta.Flags & ItemFlags.NotStackable) == 0;
            this.DisplayName = this.Meta.DisplayName;
            this.SingleWeight = defaultWeight;
            this.Amount = 1;
        }

        public void Initialize()
        {
            // Do nothing
        }

        public void SetCurrentInventory(IInventory inventory)
        {
            this.CurrentInventory = inventory;
        }

        public void SetAmount(int newAmount)
        {
            this.Amount = newAmount;
        }

        public void IncreaseAmount(int amountIncrease)
        {
            this.Amount += amountIncrease;
        }

        public void ReduceAmount(int amountReduce)
        {
            this.Amount -= amountReduce;
        }

        public void SetSingleWeight(int weight)
        {
            this.SingleWeight = weight;
        }

        public void SetDisplayName(string displayName)
        {
            throw new NotImplementedException();
        }

        public bool CanMergeWith(IItem sourceItem)
        {
            return this.IsMergableCheck == null || this.IsMergableCheck(sourceItem);
        }

        public Task MergeItemAsync(IItem sourceItem)
        {
            throw new NotImplementedException();
        }

        public Task<IItem> SplitItemAsync(int targetAmount)
        {
            throw new NotImplementedException();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
