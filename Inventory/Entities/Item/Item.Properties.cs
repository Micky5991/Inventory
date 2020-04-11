using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Item
{
    /// <content>
    /// Implementation of all properties.
    /// </content>
    public abstract partial class Item
    {
        private int amount;
        private string displayName;
        private IInventory? currentInventory;
        private int singleWeight;
        private bool movingLocked;
        private bool locked;

        /// <inheritdoc />
        public string Handle { get; }

        /// <inheritdoc />
        public Guid RuntimeId { get; }

        /// <inheritdoc />
        public ItemMeta Meta { get; }

        /// <inheritdoc />
        public string DefaultDisplayName { get; }

        /// <inheritdoc />
        public string DisplayName
        {
            get => this.displayName;
            private set
            {
                if (value == this.displayName)
                {
                    return;
                }

                this.displayName = value;

                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public int Amount
        {
            get => this.amount;
            private set
            {
                if (value == this.amount)
                {
                    return;
                }

                this.amount = value;

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.TotalWeight));
            }
        }

        /// <inheritdoc />
        public int SingleWeight
        {
            get => this.singleWeight;
            private set
            {
                if (value == this.singleWeight)
                {
                    return;
                }

                this.singleWeight = value;

                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.TotalWeight));
            }
        }

        /// <inheritdoc />
        public int TotalWeight => this.SingleWeight * this.Amount;

        /// <inheritdoc />
        public bool Stackable { get; }

        /// <inheritdoc />
        public bool WeightChangable { get; }

        /// <inheritdoc />
        public IInventory? CurrentInventory
        {
            get => this.currentInventory;
            private set
            {
                if (Equals(value, this.currentInventory))
                {
                    return;
                }

                this.currentInventory = value;

                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public bool MovingLocked
        {
            get => this.movingLocked || this.Locked;
            set
            {
                if (value == this.movingLocked)
                {
                    return;
                }

                this.movingLocked = value;

                this.OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public bool Locked
        {
            get => this.locked;
            set
            {
                if (value == this.locked)
                {
                    return;
                }

                var oldMovingLockedValue = this.MovingLocked;

                this.locked = value;

                this.OnPropertyChanged();

                if (oldMovingLockedValue != this.MovingLocked)
                {
                    this.OnPropertyChanged(nameof(this.MovingLocked));
                }
            }
        }
    }
}
