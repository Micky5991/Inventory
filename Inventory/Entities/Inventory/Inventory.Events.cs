using System.ComponentModel;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    /// <content>
    /// Seperated Handlers for Events during Inventory actions.
    /// </content>
    public partial class Inventory
    {
        private void OnItemAdded(IItem item)
        {
            item.SetCurrentInventory(this);

            item.PropertyChanged += this.OnPropertyChanged;

            this.OnAfterItemAddedOrRemoved(item);
        }

        private void OnItemRemoved(IItem item)
        {
            item.SetCurrentInventory(null);

            item.PropertyChanged -= this.OnPropertyChanged;

            this.OnAfterItemAddedOrRemoved(item);
        }

        private void OnAfterItemAddedOrRemoved(IItem item)
        {
            this.RecalculateWeight();

            this.OnPropertyChanged(nameof(this.Items));
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(IItem.TotalWeight):
                    this.RecalculateWeight();

                    break;

                case nameof(IItem.Amount):
                    this.OnItemAmountChange((IItem)sender);

                    break;
            }
        }

        private void OnItemAmountChange(IItem item)
        {
            if (item.Amount <= 0)
            {
                this.RemoveItem(item);
            }
        }
    }
}
