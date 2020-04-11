using System;
using System.Threading.Tasks;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.EventArgs;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;
using Micky5991.Inventory.Strategies;

namespace Micky5991.Inventory.Entities.Item
{
    /// <inheritdoc />
    public abstract partial class Item : IItem
    {
        private readonly IItemMergeStrategyHandler itemMergeStrategyHandler;
        private readonly IItemSplitStrategyHandler itemSplitStrategyHandler;
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

            this.itemMergeStrategyHandler = itemServices.ItemMergeStrategyHandler;
            this.itemSplitStrategyHandler = itemServices.ItemSplitStrategyHandler;
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

            this.DisplayName = this.DefaultDisplayName;
            this.Amount = Math.Max(MinimalItemAmount, 1);
        }

        internal static int MinimalItemAmount { get; set; } = 0;

        /// <inheritdoc />
        public void Initialize()
        {
            this.SetupStrategies();

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
                throw new ArgumentOutOfRangeException(nameof(amountReduce), $"Value has to be lower or equal than {this.Amount}");
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
        public void SetDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            this.DisplayName = displayName;
        }

        /// <inheritdoc />
        public bool CanMergeWith(IItem sourceItem)
        {
            if (sourceItem == null)
            {
                throw new ArgumentNullException();
            }

            return this.itemMergeStrategyHandler.CanBeMerged(this, sourceItem);
        }

        /// <inheritdoc />
        public async Task MergeItemAsync(IItem sourceItem)
        {
            if (sourceItem == null)
            {
                throw new ArgumentNullException(nameof(sourceItem));
            }

            if (this.itemMergeStrategyHandler.CanBeMerged(this, sourceItem) == false)
            {
                throw new ArgumentException("The item cannot be merged with this instance", nameof(sourceItem));
            }

            await this.itemMergeStrategyHandler.MergeItemWithAsync(this, sourceItem)
                      .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IItem> SplitItemAsync(int targetAmount)
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

            await this.itemSplitStrategyHandler.SplitItemAsync(this, item)
                      .ConfigureAwait(false);

            return item;
        }

        /// <summary>
        /// Initializer to setup <see cref="itemMergeStrategyHandler"/>.
        /// </summary>
        protected virtual void SetupStrategies()
        {
            this.itemMergeStrategyHandler.Add(new BasicItemMergeStrategy());
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
