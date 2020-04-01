using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Item;

namespace Micky5991.Inventory.Tests.Fakes
{
    public class RealItem : Item
    {
        public RealItem(ItemMeta meta, AggregatedItemServices itemServices) : base(meta, itemServices)
        {
        }
    }
}
