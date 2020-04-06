using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    public partial class Inventory
    {
        public async Task<bool> InsertItemAsync(IItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (IsItemAllowed(item) == false)
            {
                throw new ItemNotAllowedException($"The given item is not allwed in this inventory.");
            }

            if (DoesItemFit(item) == false)
            {
                throw new InventoryCapacityException(nameof(item), item);
            }

            if (item.CurrentInventory == this)
            {
                return false;
            }

            if (item.CurrentInventory != null)
            {
                var oldInventoryRemoveSuccess = await item.CurrentInventory.RemoveItemAsync(item)
                    .ConfigureAwait(false);

                if (oldInventoryRemoveSuccess == false)
                {
                    return false;
                }
            }

            if (await TryMergeItemAsync(item).ConfigureAwait(false))
            {
                return true;
            }

            if (_items.TryAdd(item.RuntimeId, item) == false)
            {
                return false;
            }

            await OnItemAdded(item)
                .ConfigureAwait(false);

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

                await MergeItemsAsync(item, sourceItem)
                    .ConfigureAwait(false);

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

            await OnItemRemoved(item)
                .ConfigureAwait(false);

            return true;
        }

        public void SetItemFilter(InventoryDelegates.ItemFilterDelegate? filter)
        {
            _itemFilter = filter;
        }
    }
}
