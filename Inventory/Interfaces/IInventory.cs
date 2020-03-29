using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Micky5991.Inventory.Interfaces
{
    public interface IInventory
    {
        IReadOnlyDictionary<Guid, IItem> Items { get; }

        int Capacity { get; }

        int UsedCapacity { get; }

        int AvailableCapacity { get; }

        bool DoesItemFit(IItem item);

        Task<bool> InsertItemAsync(IItem item);

        Task<bool> RemoveItemAsync(IItem item);

    }
}
