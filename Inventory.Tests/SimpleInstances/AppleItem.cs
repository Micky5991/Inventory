using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Item;

namespace Micky5991.Inventory.Tests.SimpleInstances
{
    public class AppleItem : Item
    {
        public AppleItem(ItemMeta meta, AggregatedItemServices itemServices) : base(meta, itemServices)
        {
        }
    }
}
