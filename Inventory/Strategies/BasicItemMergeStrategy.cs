using System;
using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;

namespace Micky5991.Inventory.Strategies
{
    public class BasicItemMergeStrategy : IItemMergeStrategy
    {
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

        public Task MergeItemWithAsync(IItem targetItem, IItem sourceItem)
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

            return Task.CompletedTask;
        }
    }
}
