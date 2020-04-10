using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Inventory
{
    public partial class Inventory
    {
        public async Task<bool> InsertItemAsync(IItem item, bool force)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (force == false)
            {
                if (IsItemAllowed(item) == false)
                {
                    throw new ItemNotAllowedException($"The given item is not allwed in this inventory.");
                }

                if (DoesItemFit(item) == false)
                {
                    throw new InventoryCapacityException(nameof(item), item);
                }

                if (item.MovingLocked)
                {
                    throw new ItemNotMovableException(item);
                }
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

            if (item.MovingLocked)
            {
                throw new ItemNotMovableException(item);
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

        public ICollection<IItem> GetInsertableItems(IInventory targetInventory, bool checkCapacity, bool checkFilter, bool checkMovable)
        {
            if (targetInventory == null)
            {
                throw new ArgumentNullException(nameof(targetInventory));
            }

            bool CapacityFilter(IItem item)
            {
                if (checkCapacity == false)
                {
                    return true;
                }

                return targetInventory.DoesItemFit(item);
            }

            bool AcceptanceFilter(IItem item)
            {
                if (checkFilter == false)
                {
                    return true;
                }

                return targetInventory.IsItemAllowed(item);
            }

            bool MovableFilter(IItem item)
            {
                if (checkMovable == false)
                {
                    return true;
                }

                return item.MovingLocked == false;
            }

            return Items.Where(x => CapacityFilter(x) && AcceptanceFilter(x) && MovableFilter(x)).ToList();
        }
    }
}
