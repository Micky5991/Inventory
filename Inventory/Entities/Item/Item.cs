using System;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Strategies;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.EventArgs;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Item
{
    /// <inheritdoc />
    public abstract partial class Item : IItem
    {
        private readonly IItemFactory itemFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="meta">Non-NULL instance of the <see cref="ItemMeta"/> that is represented by this instance.</param>
        /// <param name="itemServices">Non-NULL instance of <see cref="AggregatedItemServices"/> which are necessary for this <see cref="Item"/>.</param>
        protected Item(ItemMeta meta, AggregatedItemServices itemServices)
        {
            if (meta == null)
            {
                throw new ArgumentNullException(nameof(meta));
            }

            this.ItemMergeStrategyHandler = itemServices.ItemMergeStrategyHandler;
            this.ItemSplitStrategyHandler = itemServices.ItemSplitStrategyHandler;
            this.itemFactory = itemServices.ItemFactory;

            this.RuntimeId = Guid.NewGuid();
            this.Meta = meta;

            var (valid, errorMessage) = this.ValidateMeta();
            if (valid == false)
            {
                throw new ArgumentException(errorMessage, nameof(meta));
            }

            this.SingleWeight = this.Meta.DefaultWeight;
            this.Handle = this.Meta.Handle;
            this.DefaultDisplayName = this.Meta.DisplayName;
            this.Stackable = (this.Meta.Flags & ItemFlags.NotStackable) == 0;
            this.WeightChangable = (this.Meta.Flags & ItemFlags.WeightChangable) != 0;

            this.displayName = this.DefaultDisplayName;
            this.Amount = Math.Max(MinimalItemAmount, 1);
        }

        internal static int MinimalItemAmount { get; set; } = 0;

        /// <summary>
        /// Gets the handler that governs the behavior how the item splits itself.
        /// </summary>
        protected IItemSplitStrategyHandler ItemSplitStrategyHandler { get; }

        /// <summary>
        /// Gets the handler that governs the behavior how other items are merged into this.
        /// </summary>
        protected IItemMergeStrategyHandler ItemMergeStrategyHandler { get; }

        /// <inheritdoc />
        public void Initialize()
        {
            this.SetupItem();

            this.Initialized?.Invoke(this, new ItemInitializedEventArgs(this));
        }

        /// <inheritdoc />
        public void SetCurrentInventory(IInventory? inventory)
        {
            this.CurrentInventory = inventory;
        }

        /// <inheritdoc />
        public void SetAmount(int newAmount)
        {
            const int hardAmountMinimum = 0;

            var minAmount = Math.Max(MinimalItemAmount, hardAmountMinimum);

            if (this.Stackable == false && newAmount > 1)
            {
                throw new ItemNotStackableException();
            }

            if (newAmount < minAmount)
            {
                throw new ArgumentOutOfRangeException(nameof(newAmount), $"Amount should be {minAmount} or higher.");
            }

            if (this.Amount < newAmount && this.CurrentInventory != null)
            {
                var amountDelta = newAmount - this.Amount;
                var weightDelta = this.SingleWeight * amountDelta;

                if (weightDelta > this.CurrentInventory.AvailableCapacity)
                {
                    throw new InventoryCapacityException(nameof(newAmount), this);
                }
            }

            this.Amount = newAmount;
        }

        /// <inheritdoc />
        public void IncreaseAmount(int amountIncrease)
        {
            if (amountIncrease <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amountIncrease), "Value has to be 1 or higher");
            }

            this.SetAmount(this.Amount + amountIncrease);
        }

        /// <inheritdoc />
        public void ReduceAmount(int amountReduce)
        {
            if (amountReduce > this.Amount)
            {
                throw new ArgumentOutOfRangeException(nameof(amountReduce), $"Value has to be lower or equal to {this.Amount}");
            }

            if (amountReduce <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amountReduce), $"Value has to be 1 or higher.");
            }

            this.SetAmount(this.Amount - amountReduce);
        }

        /// <inheritdoc />
        public void SetSingleWeight(int weight)
        {
            if (weight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight has to be 1 or higher");
            }

            var weightDelta = weight - this.SingleWeight;
            var totalDelta = weightDelta * this.Amount;

            if (this.CurrentInventory != null && weightDelta > 0 && this.CurrentInventory.AvailableCapacity < totalDelta)
            {
                throw new InventoryCapacityException(nameof(weight), this);
            }

            this.SingleWeight = weight;
        }

        /// <inheritdoc />
        public void SetDisplayName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentNullException(nameof(newName));
            }

            this.DisplayName = newName;
        }

        /// <inheritdoc />
        public bool CanMergeWith(IItem sourceItem)
        {
            if (sourceItem == null)
            {
                throw new ArgumentNullException();
            }

            return this.ItemMergeStrategyHandler.CanBeMerged(this, sourceItem);
        }

        /// <inheritdoc />
        public void MergeItem(IItem sourceItem)
        {
            if (sourceItem == null)
            {
                throw new ArgumentNullException(nameof(sourceItem));
            }

            if (this.ItemMergeStrategyHandler.CanBeMerged(this, sourceItem) == false)
            {
                throw new ArgumentException("The item cannot be merged with this instance", nameof(sourceItem));
            }

            this.ItemMergeStrategyHandler.MergeItemWith(this, sourceItem);
        }

        /// <inheritdoc />
        public IItem SplitItem(int targetAmount)
        {
            if (targetAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetAmount), $"The given {nameof(targetAmount)} has to be 1 or higher");
            }

            if (targetAmount >= this.Amount)
            {
                throw new ArgumentOutOfRangeException(nameof(targetAmount), $"The given {nameof(targetAmount)} has to be {this.Amount - 1} or lower");
            }

            var item = this.itemFactory.CreateItem(this.Meta, targetAmount);

            this.SetAmount(this.Amount - targetAmount);

            this.ItemSplitStrategyHandler.SplitItem(this, item);

            return item;
        }

        /// <summary>
        /// Method that initializes actual item data and settings before first usage.
        /// </summary>
        protected virtual void SetupItem()
        {
            this.SetupStrategies();
        }

        /// <summary>
        /// Initializer to setup <see cref="ItemMergeStrategyHandler"/>.
        /// </summary>
        protected virtual void SetupStrategies()
        {
            this.ItemMergeStrategyHandler.Add(new BasicItemMergeStrategy());
        }

        private (bool Valid, string? ErrorMessage) ValidateMeta()
        {
            var currentType = this.GetType();

            if (this.Meta.Type != currentType)
            {
                return (false, "The current type in the provided meta mismatches the actual item type");
            }

            return (true, null);
        }
    }
}
