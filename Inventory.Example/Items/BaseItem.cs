using Micky5991.Inventory;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Item;

namespace Inventory.Example.Items
{
    public class BaseItem : Item
    {
        public BaseItem(ItemMeta meta, AggregatedItemServices itemServices)
            : base(meta, itemServices)
        {
        }
    }
}
