using System;
using System.Threading.Tasks;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;
using Micky5991.Inventory.Strategies;

namespace Micky5991.Inventory.Entities.Item
{
    public abstract partial class Item : IItem
    {
        private readonly IItemMergeStrategyHandler itemMergeStrategyHandler;
        private readonly IItemSplitStrategyHandler itemSplitStrategyHandler;
        private readonly IItemFactory itemFactory;

        internal static int MinimalItemAmount { get; set; } = 0;

        protected Item(ItemMeta meta, AggregatedItemServices itemServices)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            itemMergeStrategyHandler = itemServices.ItemMergeStrategyHandler;
            itemSplitStrategyHandler = itemServices.ItemSplitStrategyHandler;
            itemFactory = itemServices.ItemFactory;

            RuntimeId = Guid.NewGuid();
            Meta = meta;

            var (valid, errorMessage) = ValidateMeta();
            if (valid == false)
            {
                throw new ArgumentException(errorMessage, nameof(meta));
            }

            SingleWeight = Meta.DefaultWeight;
            Handle = Meta.Handle;
            DefaultDisplayName = Meta.DisplayName;
            Stackable = (Meta.Flags & ItemFlags.NotStackable) == 0;

            DisplayName = DefaultDisplayName;
            Amount = Math.Max(MinimalItemAmount, 1);
        }

        protected virtual void SetupStrategies()
        {
            itemMergeStrategyHandler.Add(new BasicItemMergeStrategy());
        }

        private (bool Valid, string? ErrorMessage) ValidateMeta()
        {
            var currentType = GetType();

            if (Meta.Type != currentType)
            {
                return (false, "The current type in the provided meta mismatches the actual item type");
            }

            return (true, null);
        }

        public void Initialize()
        {
            SetupStrategies();
        }

        public void SetCurrentInventory(IInventory? inventory)
        {
            CurrentInventory = inventory;
        }

        public void SetAmount(int newAmount)
        {
            const int hardAmountMinimum = 0;

            var minAmount = Math.Max(MinimalItemAmount, hardAmountMinimum);

            if (Stackable == false && newAmount > 1)
            {
                throw new ItemNotStackableException();
            }

            if (newAmount < minAmount)
            {
                throw new ArgumentOutOfRangeException(nameof(newAmount), $"Amount should be {minAmount} or higher.");
            }

            if (Amount < newAmount && CurrentInventory != null)
            {
                var amountDelta = newAmount - Amount;
                var weightDelta = SingleWeight * amountDelta;

                if (weightDelta > CurrentInventory.AvailableCapacity)
                {
                    throw new InventoryCapacityException(nameof(newAmount), this);
                }
            }

            Amount = newAmount;
        }

        public void SetSingleWeight(int weight)
        {
            if (weight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight has to be 1 or higher");
            }

            var weightDelta = weight - SingleWeight;
            var totalDelta = weightDelta * Amount;

            if (CurrentInventory != null && weightDelta > 0 && CurrentInventory.AvailableCapacity < totalDelta)
            {
                throw new InventoryCapacityException(nameof(weight), this);
            }

            SingleWeight = weight;
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

            return itemMergeStrategyHandler.CanBeMerged(this, sourceItem);
        }

        public async Task MergeItemAsync(IItem sourceItem)
        {
            if (sourceItem == null)
            {
                throw new ArgumentNullException(nameof(sourceItem));
            }

            if (itemMergeStrategyHandler.CanBeMerged(this, sourceItem) == false)
            {
                throw new ArgumentException("The item cannot be merged with this instance", nameof(sourceItem));
            }

            await itemMergeStrategyHandler.MergeItemWithAsync(this, sourceItem)
                .ConfigureAwait(false);
        }

        public async Task<IItem> SplitItemAsync(int targetAmount)
        {
            if (targetAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetAmount), $"The given {nameof(targetAmount)} has to be 1 or higher");
            }

            if (targetAmount >= Amount)
            {
                throw new ArgumentOutOfRangeException(nameof(targetAmount), $"The given {nameof(targetAmount)} has to be {Amount - 1} or lower");
            }

            var item = itemFactory.CreateItem(Meta, targetAmount);

            SetAmount(Amount - targetAmount);

            await itemSplitStrategyHandler.SplitItemAsync(this, item)
                .ConfigureAwait(false);

            return item;
        }
    }
}
