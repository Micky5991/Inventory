using System;
using Micky5991.Inventory.Data;

namespace Micky5991.Inventory.Interfaces
{
    /// <summary>
    /// Item actions are options which are executable on items.
    /// </summary>
    /// <typeparam name="TOut">Outgoing data for this item action.</typeparam>
    /// <typeparam name="TIn">Incoming data for this item action.</typeparam>
    public interface IItemAction<TOut, TIn>
        where TOut : OutogingItemActionData
        where TIn : IncomingItemActionData
    {
        /// <summary>
        /// Gets a non persistant identifier of this action that should ONLY be used for communication in runtime and during the lifetime of this item.
        /// </summary>
        Guid RuntimeId { get; }

        /// <summary>
        /// Sets the item reference where this action was added to.
        /// </summary>
        /// <param name="item">Reference to the related item.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null.</exception>
        void SetRelatedItem(IItem item);

        /// <summary>
        /// Executes this action with given data.
        /// </summary>
        /// <param name="data">Data that holds information about the actual usage of this action.</param>
        /// <exception cref="ArgumentNullException"><paramref name="data"/> is null.</exception>
        void Execute(TIn data);

        /// <summary>
        /// Builds the item action data that can be used for communication with an user interface.
        /// </summary>
        /// <returns>Created action data of this instance.</returns>
        TOut BuildActionData();
    }
}
