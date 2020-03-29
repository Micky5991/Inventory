using System.Collections.Generic;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    [PublicAPI]
    public interface IItemRegistry
    {

        ICollection<ItemMeta> GetItemMeta();

    }
}
