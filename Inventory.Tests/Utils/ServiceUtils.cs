using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Strategies.Handlers;
using Moq;

namespace Micky5991.Inventory.Tests.Utils
{
    public static class ServiceUtils
    {

        public static AggregatedItemServices CreateItemServices(
            IItemMergeStrategyHandler mergeStrategyHandler = null,
            IItemSplitStrategyHandler splitStrategyHandler = null,
            IItemFactory itemFactory = null
        ) {
            if (mergeStrategyHandler == null)
            {
                mergeStrategyHandler = new ItemMergeStrategyHandler();
            }

            if (splitStrategyHandler == null)
            {
                splitStrategyHandler = new ItemSplitStrategyHandler();
            }

            if (itemFactory == null)
            {
                itemFactory = new Mock<IItemFactory>().Object;
            }

            return new AggregatedItemServices(mergeStrategyHandler, splitStrategyHandler, itemFactory);
        }

    }
}
