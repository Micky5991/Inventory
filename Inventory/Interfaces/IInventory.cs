using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Micky5991.Inventory.Exceptions;

namespace Micky5991.Inventory.Interfaces
{
    [PublicAPI]
    public interface IInventory : INotifyPropertyChanged
    {
        /// <summary>
        /// Contains all items currently added to the inventory
        /// </summary>
        ICollection<IItem> Items { get; }

        /// <summary>
        /// Current capacity of this inventory
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// Accumulated weight of all items currently in this inventory
        /// </summary>
        int UsedCapacity { get; }

        /// <summary>
        /// The current capacity available in this inventory.
        /// </summary>
        int AvailableCapacity { get; }

        /// <summary>
        /// Determines if the given item would fit into this inventory
        /// </summary>
        /// <param name="item">Item that should be checked</param>
        /// <returns>true if capacity is available, false otherwise</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null</exception>
        bool DoesItemFit([NotNull] IItem item);

        /// <summary>
        /// Tries to insert the given <paramref name="item"/> into this inventory.
        ///
        /// - If the given item is already in any other inventory, it will be removed from the old one first.
        ///
        /// - If an item, that is ready to be merged with the given <paramref name="item"/> exists in this inventory,
        /// it will be merged into the first existing item and the given <paramref name="item"/> will be disposed.
        /// </summary>
        /// <param name="item">The item that should be added</param>
        /// <returns>true if the item has been added or merged, false otherwise</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null</exception>
        /// <exception cref="InventoryCapacityException">Adding the <paramref name="item"/> would exceed the current inventory capacity</exception>
        Task<bool> InsertItemAsync([NotNull] IItem item);

        /// <summary>
        /// Tries to remove the given item <paramref name="item"/> from this inventory.
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>true if the given <paramref name="item"/> was found and successfully removed, false otherwise</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null</exception>
        Task<bool> RemoveItemAsync([NotNull] IItem item);

        /// <summary>
        /// Sets the capacity of this inventory to the given <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">New capacity to set the inventory to</param>
        /// <returns>true if the usedcapacity was below or equal <paramref name="capacity"/> and has been changed successfully, false otherwise</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> was too low</exception>
        bool SetCapacity(int capacity);

    }
}
