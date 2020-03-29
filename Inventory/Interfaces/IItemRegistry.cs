using System.Collections;
using System.Collections.Generic;

namespace Micky5991.Inventory.Interfaces
{
    public interface IItemRegistry
    {

        ICollection<ItemMeta> GetItemMeta();

    }
}
