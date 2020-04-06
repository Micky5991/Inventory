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
        /// Non persistant identifier of this inventory that should ONLY be used for communication in runtime and during the lifetime of this inventory.
        /// </summary>
        Guid RuntimeId { get; }

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
        /// Returns all items in this inventory that have the given <paramref name="handle"/>
        /// </summary>
        /// <param name="handle">Handle to search for</param>
        /// <returns>Found items with the given <paramref name="handle"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> is null or whitespace</exception>
        ICollection<IItem> GetItems(string handle);

        /// <summary>
        /// Returns a list of all items that are assignable to <typeparamref name="T"/>.
        /// If <paramref name="handle"/> is not empty, it has to match <paramref name="handle"/> too.
        /// </summary>
        /// <param name="handle">Restriction of items to only include items with specified handle</param>
        /// <typeparam name="T">Parent type of the items that should be included</typeparam>
        /// <returns>List of items that matched the given criteria</returns>
        ICollection<T> GetItems<T>(string? handle = null) where T : IItem;

        /// <summary>
        /// Determines if the given item is allowed in inventory
        /// </summary>
        /// <param name="item">Item that should be checked</param>
        /// <returns>true if item is allwed, false otherwise</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null</exception>
        bool IsItemAllowed([NotNull] IItem item);

        /// <summary>
        /// Determines if the given item would fit into this inventory
        /// </summary>
        /// <param name="item">Item that should be checked</param>
        /// <returns>true if capacity is available, false otherwise</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null</exception>
        bool DoesItemFit([NotNull] IItem item);

        /// <summary>
        /// Determines if the item could be added to the inventory. Capacity and filter is checked.
        /// </summary>
        /// <param name="item">Item that should be checked</param>
        /// <returns>true if item can be added, false otherwise</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null</exception>
        bool CanBeInserted([NotNull] IItem item);

        /// <summary>
        /// Determines if the given <paramref name="handle"/> can be inserted <paramref name="amount"/> of times into this inventory.
        /// </summary>
        /// <param name="handle">ItemMeta identifier to search for</param>
        /// <param name="amount">Multiplication of weight received from <paramref name="handle"/></param>
        /// <returns>true if the <paramref name="handle"/> can be added these amount of times</returns>
        /// <exception cref="ItemMetaNotFoundException"><paramref name="handle"/> is unknown</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="amount"/> is lower than 1</exception>
        bool DoesItemFit(string handle, int amount = 1);

        /// <summary>
        /// Determines if the given <paramref name="meta"/> can be added <paramref name="amount"/> of times into this inventory.
        /// </summary>
        /// <param name="meta">Item information to get the single weight from</param>
        /// <param name="amount">Multiplication of weight received from <paramref name="meta"/></param>
        /// <returns>true if the item would fit, false otherwise</returns>
        /// <exception cref="ArgumentNullException"><paramref name="meta"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="amount"/> is 0 or lower</exception>
        bool DoesItemFit(ItemMeta meta, int amount = 1);

        /// <summary>
        /// Tries to insert the given <paramref name="item"/> into this inventory.
        ///
        /// - If the given item is already in any other inventory, it will be removed from the old one first.
        ///
        /// - If an item, that is ready to be merged with the given <paramref name="item"/> exists in this inventory,
        /// it will be merged into the first existing item and the given <paramref name="item"/> will be disposed.
        /// </summary>
        /// <param name="item">The item that should be added</param>
        /// <param name="force">true if checks regarding capacity and filter should be ignored</param>
        /// <returns>true if the item has been added or merged, false otherwise</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null</exception>
        /// <exception cref="InventoryCapacityException">Adding the <paramref name="item"/> would exceed the current inventory capacity</exception>
        Task<bool> InsertItemAsync([NotNull] IItem item, bool force = false);

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

        /// <summary>
        /// Sets the filter of this inventory to only accept certain items into inventory. To remove the current filter
        /// and allow all items again, set the <paramref name="filter"/> to null.
        /// </summary>
        /// <param name="filter">Filter to set to</param>
        void SetItemFilter(InventoryDelegates.ItemFilterDelegate? filter);

        /// <summary>
        /// Returns a list of items of this inventory which could be inserted into the <paramref name="targetInventory"/>.
        /// The used filters can be specified.
        /// </summary>
        /// <param name="targetInventory"><see cref="IInventory"/> to check items to merge into</param>
        /// <param name="checkCapacity">true if unavailable capacity removes item from possible items, false otherwise</param>
        /// <param name="checkFilter">true if false filter result removes item from possible items, false otherwise</param>
        /// <returns>List of items that fit the created criteria</returns>
        /// <exception cref="ArgumentNullException"><paramref name="targetInventory"/> is null</exception>
        ICollection<IItem> GetInsertableItems(IInventory targetInventory, bool checkCapacity = true, bool checkFilter = true);

    }
}
