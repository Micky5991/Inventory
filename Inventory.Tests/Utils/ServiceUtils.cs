using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Strategies.Handlers;

namespace Micky5991.Inventory.Tests.Utils
{
    public static class ServiceUtils
    {

        public static AggregatedItemServices CreateItemServices(IItemMergeStrategyHandler mergeStrategyHandler = null)
        {
            if (mergeStrategyHandler == null)
            {
                mergeStrategyHandler = new ItemMergeStrategyHandler();
            }

            return new AggregatedItemServices(mergeStrategyHandler);
        }

    }
}
