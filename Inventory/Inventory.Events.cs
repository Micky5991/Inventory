using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    internal partial class Inventory
    {

        private async Task OnItemAdded(IItem item)
        {
            await OnAfterItemAddedOrRemoved(item);
        }

        private async Task OnItemRemoved(IItem item)
        {
            await OnAfterItemAddedOrRemoved(item);
        }

        private async Task OnAfterItemAddedOrRemoved(IItem item)
        {
            RecalculateWeight();
        }

    }
}
