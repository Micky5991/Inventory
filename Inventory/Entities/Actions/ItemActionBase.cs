using System;
using CommunityToolkit.Diagnostics;
using JetBrains.Annotations;
using Micky5991.Inventory.Data;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Actions
{
    /// <summary>
    /// Base class that offers basic functionality for item actions like visible or enabled states.
    /// </summary>
    /// <typeparam name="TOut">Outgoing data for this item action.</typeparam>
    /// <typeparam name="TIn">Incoming data for this item action.</typeparam>
    [PublicAPI]
    public abstract class ItemActionBase<TOut, TIn> : IItemAction<TOut, TIn>
        where TOut : OutgoingItemActionData
        where TIn : IncomingItemActionData
    {
        private InventoryDelegates.ActionVisibleDelegate? visibleCheck;

        private InventoryDelegates.ActionEnabledDelegate? enabledCheck;

        internal ItemActionBase(Guid runtimeId)
        {
            this.RuntimeId = runtimeId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemActionBase{TOut, TIn}"/> class.
        /// </summary>
        protected ItemActionBase()
        {
            this.RuntimeId = Guid.NewGuid();
        }

        /// <inheritdoc />
        public Guid RuntimeId { get; }

        /// <inheritdoc/>
        public IItem? RelatedItem { get; private set; }

        /// <inheritdoc />
        public abstract void Execute(object? executor, TIn? data);

        /// <inheritdoc />
        public abstract TOut BuildActionData(object? receiver);

        /// <inheritdoc />
        public void SetRelatedItem(IItem item)
        {
            Guard.IsNotNull(item);

            this.RelatedItem = item;
        }

        /// <inheritdoc />
        public IItemAction<TOut, TIn> SetVisibleCheck(InventoryDelegates.ActionVisibleDelegate? newVisibleCheck)
        {
            this.visibleCheck = newVisibleCheck;

            return this;
        }

        /// <inheritdoc />
        public IItemAction<TOut, TIn> SetEnabledCheck(InventoryDelegates.ActionEnabledDelegate? newEnabledCheck)
        {
            this.enabledCheck = newEnabledCheck;

            return this;
        }

        /// <inheritdoc/>
        public bool IsVisible(object? receiver)
        {
            if (this.visibleCheck == null)
            {
                return true;
            }

            return this.visibleCheck(receiver);
        }

        /// <inheritdoc/>
        public bool IsEnabled(object? receiver)
        {
            if (this.IsVisible(receiver) == false)
            {
                return false;
            }

            if (this.enabledCheck == null)
            {
                return true;
            }

            return this.enabledCheck(receiver);
        }
    }
}
