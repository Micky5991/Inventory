using System;
using System.Linq;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Strategies
{
    /// <inheritdoc cref="IItemMergeStrategyHandler"/>
    public class ItemMergeStrategyHandler : StrategyHandler<IItemMergeStrategy>, IItemMergeStrategyHandler
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

            return this.All(x => x.CanBeMerged(targetItem, sourceItem));
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

            foreach (var strategy in this)
            {
                strategy.MergeItemWith(targetItem, sourceItem);
            }
        }
    }
}
