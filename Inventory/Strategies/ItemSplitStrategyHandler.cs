using System;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;

namespace Micky5991.Inventory.Strategies
{
    /// <inheritdoc cref="IItemSplitStrategyHandler"/>
    public class ItemSplitStrategyHandler : StrategyHandler<IItemSplitStrategy>, IItemSplitStrategyHandler
    {
        /// <inheritdoc />
        public void SplitItem(IItem oldItem, IItem newItem)
        {
            if (oldItem == null)
            {
                throw new ArgumentNullException(nameof(oldItem));
            }

            if (newItem == null)
            {
                throw new ArgumentNullException(nameof(newItem));
            }

            foreach (var strategy in this)
            {
                strategy.SplitItem(oldItem, newItem);
            }
        }
    }
}
