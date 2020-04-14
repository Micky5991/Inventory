using System;
using JetBrains.Annotations;
using Micky5991.Inventory.Data;

namespace Micky5991.Inventory.Interfaces
{
    /// <summary>
    /// Item actions are options which are executable on items.
    /// </summary>
    /// <typeparam name="TOut">Outgoing data for this item action.</typeparam>
    /// <typeparam name="TIn">Incoming data for this item action.</typeparam>
    [PublicAPI]
    public interface IItemAction<TOut, TIn>
        where TOut : OutgoingItemActionData
        where TIn : IncomingItemActionData
    {
        /// <summary>
        /// Gets a non persistant identifier of this action that should ONLY be used for communication in runtime and during the lifetime of this item.
        /// </summary>
        Guid RuntimeId { get; }

        /// <summary>
        /// Gets the reference to the item where this action was added to.
        /// </summary>
        IItem? RelatedItem { get; }

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

        /// <summary>
        /// Returns if the item action is visible.
        /// </summary>
        /// <returns>true if the action is visible, false otherwise.</returns>
        bool IsVisible();

        /// <summary>
        /// Returns if the item action is enabled.
        /// </summary>
        /// <returns>true if the action is enabled, false otherwise.</returns>
        bool IsEnabled();

        /// <summary>
        /// Sets the current check that will be used to determine if the action is visible.
        /// </summary>
        /// <param name="visibleCheck">Callback that should be used. Pass null to remove check and always show action.</param>
        /// <returns>Current <see cref="IItemAction{TOut,TIn}"/> instance.</returns>
        IItemAction<TOut, TIn> SetVisibleCheck(InventoryDelegates.ActionVisibleDelegate? visibleCheck);

        /// <summary>
        /// Sets the current check that will be used to determine if the action is enabled. If the action is not visible,
        /// this action is also implicitly disabled.
        /// </summary>
        /// <param name="enabledCheck">Callback that should be used. Pass null to remove check and always enable action.</param>
        /// <returns>Current <see cref="IItemAction{TOut,TIn}"/> instance.</returns>
        IItemAction<TOut, TIn> SetEnabledCheck(InventoryDelegates.ActionEnabledDelegate? enabledCheck);
    }
}
