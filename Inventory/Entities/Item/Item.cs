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
        private readonly IItemMergeStrategyHandler _itemMergeStrategyHandler;
        private readonly IItemSplitStrategyHandler _itemSplitStrategyHandler;
        private readonly IItemFactory _itemFactory;

        internal static int MinimalItemAmount { get; set; } = 0;

        protected Item(ItemMeta meta, AggregatedItemServices itemServices)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            _itemMergeStrategyHandler = itemServices.ItemMergeStrategyHandler;
            _itemSplitStrategyHandler = itemServices.ItemSplitStrategyHandler;
            _itemFactory = itemServices.ItemFactory;

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
            _itemMergeStrategyHandler.Add(new BasicItemMergeStrategy());
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

            return _itemMergeStrategyHandler.CanBeMerged(this, sourceItem);
        }

        public async Task MergeItemAsync(IItem sourceItem)
        {
            if (sourceItem == null)
            {
                throw new ArgumentNullException(nameof(sourceItem));
            }

            if (_itemMergeStrategyHandler.CanBeMerged(this, sourceItem) == false)
            {
                throw new ArgumentException("The item cannot be merged with this instance", nameof(sourceItem));
            }

            await _itemMergeStrategyHandler.MergeItemWithAsync(this, sourceItem)
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

            var item = _itemFactory.CreateItem(Meta, targetAmount);

            SetAmount(Amount - targetAmount);

            await _itemSplitStrategyHandler.SplitItemAsync(this, item)
                .ConfigureAwait(false);

            return item;
        }
    }
}
