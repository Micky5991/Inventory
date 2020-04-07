using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Item
{
    public abstract partial class Item
    {
        private int _amount;
        private string _displayName;
        private IInventory? _currentInventory;
        private int _singleWeight;
        private bool _movingLocked;
        private bool _locked;

        public string Handle { get; }

        public Guid RuntimeId { get; }

        public ItemMeta Meta { get; }

        public string DefaultDisplayName { get; }

        public string DisplayName
        {
            get => _displayName;
            private set
            {
                if (value == _displayName)
                {
                    return;
                }

                _displayName = value;

                OnPropertyChanged();
            }
        }

        public int Amount
        {
            get => _amount;
            private set
            {
                if (value == _amount)
                {
                    return;
                }

                _amount = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalWeight));
            }
        }

        public int SingleWeight
        {
            get => _singleWeight;
            private set
            {
                if (value == _singleWeight)
                {
                    return;
                }

                _singleWeight = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalWeight));
            }
        }

        public int TotalWeight => SingleWeight * Amount;

        public bool Stackable { get; }

        public IInventory? CurrentInventory
        {
            get => _currentInventory;
            private set
            {
                if (Equals(value, _currentInventory))
                {
                    return;
                }

                _currentInventory = value;

                OnPropertyChanged();
            }
        }

        public bool MovingLocked
        {
            get => _movingLocked || Locked;
            set
            {
                if (value == _movingLocked)
                {
                    return;
                }

                _movingLocked = value;

                OnPropertyChanged();
            }
        }

        public bool Locked
        {
            get => _locked;
            set
            {
                if (value == _locked)
                {
                    return;
                }

                _locked = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(MovingLocked));
            }
        }
    }
}
