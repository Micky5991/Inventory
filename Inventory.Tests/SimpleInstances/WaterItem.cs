using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Item;

namespace Micky5991.Inventory.Tests.SimpleInstances
{
    public class WaterItem : Item
    {
        public WaterItem(ItemMeta meta, AggregatedItemServices itemServices) : base(meta, itemServices)
        {
        }
    }
}
