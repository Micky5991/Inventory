using Micky5991.Inventory;
using Micky5991.Inventory.AggregatedServices;

namespace Inventory.Example.Items
{
    public class DiceItem : BaseItem
    {
        public DiceItem(ItemMeta meta, AggregatedItemServices itemServices) : base(meta, itemServices)
        {
        }
    }
}
