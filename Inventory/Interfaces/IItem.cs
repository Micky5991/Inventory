using System;

namespace Micky5991.Inventory.Interfaces
{
    public interface IItem
    {

        string Handle { get; }

        Guid RuntimeId { get; }

        ItemMeta Meta { get; }

        string DefaultDisplayName { get; }

        string DisplayName { get; }

        int Weight { get; }

        void SetDisplayName(string displayName);

    }
}
