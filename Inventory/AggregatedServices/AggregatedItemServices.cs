using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.AggregatedServices
{
    public class AggregatedItemServices
    {
        public IItemMergeStrategyHandler ItemMergeStrategyHandler { get; }
        public IItemFactory ItemFactory { get; }
        public IItemSplitStrategyHandler ItemSplitStrategyHandler { get; }

        public AggregatedItemServices(
            IItemMergeStrategyHandler itemMergeStrategyHandler,
            IItemSplitStrategyHandler itemSplitStrategyHandler,
            IItemFactory itemFactory)
        {
            ItemMergeStrategyHandler = itemMergeStrategyHandler;
            ItemSplitStrategyHandler = itemSplitStrategyHandler;
            ItemFactory = itemFactory;
        }
    }
}
