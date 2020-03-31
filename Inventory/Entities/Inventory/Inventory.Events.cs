using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    internal partial class Inventory
    {

        private async Task OnItemAdded(IItem item)
        {
            item.SetCurrentInventory(this);

            await OnAfterItemAddedOrRemoved(item);
        }

        private async Task OnItemRemoved(IItem item)
        {
            item.SetCurrentInventory(null);

            await OnAfterItemAddedOrRemoved(item);
        }

        private Task OnAfterItemAddedOrRemoved(IItem item)
        {
            RecalculateWeight();

            OnPropertyChanged(nameof(Items));

            return Task.CompletedTask;
        }

    }
}