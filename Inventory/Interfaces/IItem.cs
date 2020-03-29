using System;

namespace Micky5991.Inventory.Interfaces
{
    public interface IItem
    {

        string Handle { get; }

        Guid RuntimeId { get; }

        ItemMeta Meta { get; }

        int Weight { get; }

    }
}
