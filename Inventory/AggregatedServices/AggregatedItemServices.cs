using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.AggregatedServices
{
    public class AggregatedItemServices
    {
        public IItemMergeStrategyHandler ItemMergeStrategyHandler { get; }

        public AggregatedItemServices(IItemMergeStrategyHandler itemMergeStrategyHandler)
        {
            ItemMergeStrategyHandler = itemMergeStrategyHandler;
        }
    }
}
