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

            if (_items.ContainsKey(item.RuntimeId) || item.CurrentInventory == this)
            {
                return false;
            }

            if (item.CurrentInventory != null && item.CurrentInventory != this)
            {
                var oldInventoryRemoveSuccess = await item.CurrentInventory.RemoveItemAsync(item);

                if (oldInventoryRemoveSuccess == false)
                {
                    return false;
                }
            }

            var merged = await TryMergeItemAsync(item);
            if (merged)
            {
                return true;
            }

            var success = _items.TryAdd(item.RuntimeId, item);

            if (success == false)
            {
                return false;
            }

            await OnItemAdded(item);

            return true;
        }

        private async Task<bool> TryMergeItemAsync(IItem sourceItem)
        {
            foreach (var item in _items.Values)
            {
                if (item.CanMergeWith(sourceItem) == false)
                {
                    continue;
                }

                await MergeItemsAsync(item, sourceItem);

                return true;
            }

            return false;
        }

        private Task MergeItemsAsync(IItem targetItem, IItem sourceItem)
        {
            return targetItem.MergeItemAsync(sourceItem);
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
