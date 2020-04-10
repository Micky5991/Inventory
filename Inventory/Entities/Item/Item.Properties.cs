using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Item
{
    public abstract partial class Item
    {
        private int amount;
        private string displayName;
        private IInventory? currentInventory;
        private int singleWeight;
        private bool movingLocked;
        private bool locked;

        public string Handle { get; }

        public Guid RuntimeId { get; }

        public ItemMeta Meta { get; }

        public string DefaultDisplayName { get; }

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

        public int TotalWeight => this.SingleWeight * this.Amount;

        public bool Stackable { get; }

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
