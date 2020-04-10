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
            get => displayName;
            private set
            {
                if (value == displayName)
                {
                    return;
                }

                displayName = value;

                OnPropertyChanged();
            }
        }

        public int Amount
        {
            get => amount;
            private set
            {
                if (value == amount)
                {
                    return;
                }

                amount = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalWeight));
            }
        }

        public int SingleWeight
        {
            get => singleWeight;
            private set
            {
                if (value == singleWeight)
                {
                    return;
                }

                singleWeight = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalWeight));
            }
        }

        public int TotalWeight => SingleWeight * Amount;

        public bool Stackable { get; }

        public IInventory? CurrentInventory
        {
            get => currentInventory;
            private set
            {
                if (Equals(value, currentInventory))
                {
                    return;
                }

                currentInventory = value;

                OnPropertyChanged();
            }
        }

        public bool MovingLocked
        {
            get => movingLocked || Locked;
            set
            {
                if (value == movingLocked)
                {
                    return;
                }

                movingLocked = value;

                OnPropertyChanged();
            }
        }

        public bool Locked
        {
            get => locked;
            set
            {
                if (value == locked)
                {
                    return;
                }

                var oldMovingLockedValue = MovingLocked;

                locked = value;

                OnPropertyChanged();

                if (oldMovingLockedValue != MovingLocked)
                {
                    OnPropertyChanged(nameof(MovingLocked));
                }
            }
        }
    }
}
