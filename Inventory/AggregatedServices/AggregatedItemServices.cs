using CommunityToolkit.Diagnostics;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.AggregatedServices
{
    /// <summary>
    /// Simplified aggregation of services an item has.
    /// </summary>
    public class AggregatedItemServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatedItemServices"/> class.
        /// </summary>
        /// <param name="itemMergeStrategyHandler">An non null-implementation of <see cref="IItemMergeStrategyHandler"/>.</param>
        /// <param name="itemSplitStrategyHandler">An non null-implementation of <see cref="IItemSplitStrategyHandler"/>.</param>
        /// <param name="itemFactory">An non null-implementation of <see cref="IItemFactory"/>.</param>
        public AggregatedItemServices(
            IItemMergeStrategyHandler itemMergeStrategyHandler,
            IItemSplitStrategyHandler itemSplitStrategyHandler,
            IItemFactory itemFactory)
        {
            Guard.IsNotNull(itemMergeStrategyHandler);
            Guard.IsNotNull(itemSplitStrategyHandler);
            Guard.IsNotNull(itemFactory);

            this.ItemMergeStrategyHandler = itemMergeStrategyHandler;
            this.ItemSplitStrategyHandler = itemSplitStrategyHandler;
            this.ItemFactory = itemFactory;
        }

        /// <summary>
        /// Gets the passed <see cref="IItemMergeStrategyHandler"/> service.
        /// </summary>
        public IItemMergeStrategyHandler ItemMergeStrategyHandler { get; }

        /// <summary>
        /// Gets the passed <see cref="IItemFactory"/> service.
        /// </summary>
        public IItemFactory ItemFactory { get; }

        /// <summary>
        /// Gets the passed <see cref="IItemSplitStrategyHandler"/> service.
        /// </summary>
        public IItemSplitStrategyHandler ItemSplitStrategyHandler { get; }
    }
}
