using System;

namespace Micky5991.Inventory.Interfaces
{
    public interface IItem
    {

        Guid RuntimeId { get; }

        ItemMeta Meta { get; }

    }
}
