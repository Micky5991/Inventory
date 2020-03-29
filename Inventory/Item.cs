using System;
using System.Threading.Tasks;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    public abstract class Item : IItem
    {
        internal static int MinimalItemAmount { get; set; } = 0;

        public string Handle { get; }

        public Guid RuntimeId { get; }

        public ItemMeta Meta { get; }

        public string DefaultDisplayName { get; }

        public string DisplayName { get; private set; }

        public int Amount { get; private set; }

        public int SingleWeight { get; }

        public int TotalWeight => SingleWeight * Amount;

        public bool Stackable { get; }

        public IInventory? CurrentInventory { get; private set; }

        protected Item(ItemMeta meta)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            RuntimeId = Guid.NewGuid();
            Meta = meta;

            SingleWeight = Meta.DefaultWeight;
            Handle = Meta.Handle;
            DefaultDisplayName = Meta.DisplayName;
            Stackable = (Meta.Flags & ItemFlags.NotStackable) == 0;

            DisplayName = DefaultDisplayName;
            Amount = Math.Max(MinimalItemAmount, 1);
        }

        public void SetCurrentInventory(IInventory? inventory)
        {
            CurrentInventory = inventory;
        }

        public void SetAmount(int newAmount)
        {
            const int hardAmountMinimum = 0;

            var minAmount = Math.Max(MinimalItemAmount, hardAmountMinimum);

            if (newAmount < minAmount)
            {
                throw new ArgumentOutOfRangeException(nameof(newAmount), $"Amount should be {minAmount} or higher.");
            }

            Amount = newAmount;
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
            if (sourceItem == null)
            {
                throw new ArgumentNullException();
            }

            if (sourceItem == this)
            {
                return false;
            }

            return
                Handle == sourceItem.Handle
                && sourceItem.Amount > 0
                && Stackable
                && sourceItem.Stackable
                && SingleWeight == sourceItem.SingleWeight;
        }

        public Task MergeItemAsync(IItem sourceItem)
        {
            if (sourceItem == null)
            {
                throw new ArgumentNullException(nameof(sourceItem));
            }

            if (sourceItem == this)
            {
                throw new ArgumentException("Could not merge item with itself", nameof(sourceItem));
            }

            SetAmount(Amount + sourceItem.Amount);

            sourceItem.SetAmount(0);

            return Task.CompletedTask;
        }
    }
}
