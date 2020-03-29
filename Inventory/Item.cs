using System;
using System.Threading.Tasks;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    public abstract class Item : IItem
    {
        internal const int MinimalItemAmount = 0;

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
            if (newAmount < MinimalItemAmount)
            {
                throw new ArgumentOutOfRangeException(nameof(newAmount), $"Amount should be {MinimalItemAmount} or higher.");
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
