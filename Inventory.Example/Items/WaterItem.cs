using Micky5991.Inventory;
using Micky5991.Inventory.AggregatedServices;

namespace Inventory.Example.Items
{
    public class WaterItem : BaseItem
    {
        public WaterItem(ItemMeta meta, AggregatedItemServices itemServices) : base(meta, itemServices)
        {
        }
    }
}
