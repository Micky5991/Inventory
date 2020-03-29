using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Micky5991.Inventory.Interfaces
{
    public interface IInventory
    {

        IReadOnlyDictionary<Guid, IItem> Items { get; }

        int Capacity { get; }

        int AvailableCapacity { get; }

        Task InsertItemAsync(IItem item);

        Task RemoveItemAsync(IItem item);

    }
}
