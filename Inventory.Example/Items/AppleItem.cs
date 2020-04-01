using Micky5991.Inventory;
using Micky5991.Inventory.AggregatedServices;

namespace Inventory.Example.Items
{
    public class AppleItem : BaseItem
    {
        public AppleItem(ItemMeta meta, AggregatedItemServices itemServices)
            : base(meta, itemServices)
        {
        }
    }
}
