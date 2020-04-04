using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.AggregatedServices
{
    public class AggregatedInventoryServices
    {
        public IItemRegistry ItemRegistry { get; }

        public AggregatedInventoryServices(IItemRegistry itemRegistry)
        {
            ItemRegistry = itemRegistry;
        }
    }
}
