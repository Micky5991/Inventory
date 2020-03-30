using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    [PublicAPI]
    public interface IInventory : INotifyPropertyChanged
    {
        IReadOnlyDictionary<Guid, IItem> Items { get; }

        int Capacity { get; }

        int UsedCapacity { get; }

        int AvailableCapacity { get; }

        bool DoesItemFit([NotNull] IItem item);

        Task<bool> InsertItemAsync([NotNull] IItem item);

        Task<bool> RemoveItemAsync([NotNull] IItem item);

        bool SetCapacity(int capacity);

    }
}
