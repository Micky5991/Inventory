using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Strategies
{
    /// <inheritdoc />
    public class BasicItemMergeStrategy : IItemMergeStrategy
    {
        /// <inheritdoc />
        public bool CanBeMerged(IItem targetItem, IItem sourceItem)
        {
            if (targetItem == null)
            {
                throw new ArgumentNullException(nameof(targetItem));
            }

            if (sourceItem == null)
            {
                throw new ArgumentNullException(nameof(sourceItem));
            }

            return sourceItem != targetItem
                   && targetItem.Handle == sourceItem.Handle
                   && sourceItem.Amount > 0
                   && targetItem.Stackable && sourceItem.Stackable
                   && targetItem.SingleWeight == sourceItem.SingleWeight;
        }

        /// <inheritdoc />
        public void MergeItemWith(IItem targetItem, IItem sourceItem)
        {
            if (targetItem == null)
            {
                throw new ArgumentNullException(nameof(targetItem));
            }

            if (sourceItem == null)
            {
                throw new ArgumentNullException(nameof(sourceItem));
            }

            targetItem.SetAmount(targetItem.Amount + sourceItem.Amount);
            sourceItem.SetAmount(0);
        }
    }
}
