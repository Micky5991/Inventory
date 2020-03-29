using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    [PublicAPI]
    public interface IInventory
    {
        IReadOnlyDictionary<Guid, IItem> Items { get; }

        int Capacity { get; }

        int UsedCapacity { get; }

        int AvailableCapacity { get; }

        bool DoesItemFit(IItem item);

        Task<bool> InsertItemAsync(IItem item);

        Task<bool> RemoveItemAsync(IItem item);

        bool SetCapacity(int capacity);

    }
}
