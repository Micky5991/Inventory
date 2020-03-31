using System.Collections.Generic;

namespace Micky5991.Inventory.Tests.SimpleInstances
{
    public class SimpleItemRegistry : BaseItemRegistry
    {
        protected override IEnumerable<ItemMeta> LoadItemMeta()
        {
            yield return CreateItemMeta<AppleItem>("apple", "Apple", 10);
            yield return CreateItemMeta<WaterItem>("water", "Water", 5);
        }
    }
}
