using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    internal partial class Inventory
    {
        public Task InsertItemAsync(IItem item)
        {
            _items.TryAdd(item.RuntimeId, item);

            RecalculateWeight();

            return Task.CompletedTask;
        }

        public Task RemoveItemAsync(IItem item)
        {
            _items.TryRemove(item.RuntimeId, out _);

            RecalculateWeight();

            return Task.CompletedTask;
        }

    }
}
