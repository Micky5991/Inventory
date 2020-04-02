using System.ComponentModel;
using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    internal partial class Inventory
    {

        private async Task OnItemAdded(IItem item)
        {
            item.SetCurrentInventory(this);

            item.PropertyChanged += OnPropertyChanged;

            await OnAfterItemAddedOrRemoved(item);
        }

        private async Task OnItemRemoved(IItem item)
        {
            item.SetCurrentInventory(null);

            item.PropertyChanged -= OnPropertyChanged;

            await OnAfterItemAddedOrRemoved(item);
        }

        private Task OnAfterItemAddedOrRemoved(IItem item)
        {
            RecalculateWeight();

            OnPropertyChanged(nameof(Items));

            return Task.CompletedTask;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(IItem.TotalWeight):
                    RecalculateWeight();

                    break;
            }
        }

    }
}
