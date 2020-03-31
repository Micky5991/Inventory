using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    internal partial class Inventory
    {
        public async Task<bool> InsertItemAsync(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (DoesItemFit(item) == false)
            {
                throw new InventoryCapacityException(nameof(item), item);
            }

            if (item.CurrentInventory != null && item.CurrentInventory != this)
            {
                var oldInventoryRemoveSuccess = await item.CurrentInventory.RemoveItemAsync(item);

                if (oldInventoryRemoveSuccess == false)
                {
                    return false;
                }
            }

            var success = _items.TryAdd(item.RuntimeId, item);

            if (success == false)
            {
                return false;
            }

            await OnItemAdded(item);

            return true;
        }

        public async Task<bool> RemoveItemAsync([NotNull] IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var success = _items.TryRemove(item.RuntimeId, out _);

            if (success == false)
            {
                return false;
            }

            await OnItemRemoved(item);

            return true;
        }

    }
}
