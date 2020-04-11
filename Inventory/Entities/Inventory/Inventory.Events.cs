using System.ComponentModel;
using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    /// <content>
    /// Seperated Handlers for Events during Inventory actions.
    /// </content>
    public partial class Inventory
    {
        private async Task OnItemAdded(IItem item)
        {
            item.SetCurrentInventory(this);

            item.PropertyChanged += this.OnPropertyChanged;

            await this.OnAfterItemAddedOrRemoved(item)
                      .ConfigureAwait(false);
        }

        private async Task OnItemRemoved(IItem item)
        {
            item.SetCurrentInventory(null);

            item.PropertyChanged -= this.OnPropertyChanged;

            await this.OnAfterItemAddedOrRemoved(item)
                      .ConfigureAwait(false);
        }

        private Task OnAfterItemAddedOrRemoved(IItem item)
        {
            this.RecalculateWeight();

            this.OnPropertyChanged(nameof(this.Items));

            return Task.CompletedTask;
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

        private async void OnItemAmountChange(IItem item)
        {
            if (item.Amount <= 0)
            {
                await this.RemoveItemAsync(item);
            }
        }
    }
}
