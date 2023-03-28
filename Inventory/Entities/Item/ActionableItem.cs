using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CommunityToolkit.Diagnostics;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Data;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Item
{
    /// <summary>
    /// Item that is able to hold actions to interact with.
    /// </summary>
    /// <typeparam name="TOut">Outgoing data type for item actions.</typeparam>
    /// <typeparam name="TIn">Incoming data type for item action execution.</typeparam>
    public abstract class ActionableItem<TOut, TIn> : Item, IActionableItem<TOut, TIn>
        where TOut : OutgoingItemActionData
        where TIn : IncomingItemActionData
    {
        private readonly ConcurrentDictionary<Guid, IItemAction<TOut, TIn>> actions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionableItem{TOut,TIn}"/> class.
        /// </summary>
        /// <param name="meta">Non-NULL instance of the <see cref="ItemMeta"/> that is represented by this instance.</param>
        /// <param name="itemServices">Non-NULL instance of <see cref="AggregatedItemServices"/> which are necessary for this <see cref="Item"/>.</param>
        protected ActionableItem(ItemMeta meta, AggregatedItemServices itemServices)
            : base(meta, itemServices)
        {
            this.actions = new ConcurrentDictionary<Guid, IItemAction<TOut, TIn>>();
        }

        /// <inheritdoc />
        public void ExecuteAction(object? executor, TIn data)
        {
            Guard.IsNotNull(data);

            if (this.actions.TryGetValue(data.ActionRuntimeId, out var action) == false)
            {
                throw new ItemActionNotFoundException($"Could not find item action with id {data.ActionRuntimeId}.");
            }

            if (action.IsEnabled(executor) == false)
            {
                return;
            }

            action.Execute(executor, data);
        }

        /// <inheritdoc />
        public ICollection<TOut> GetAllActionData(object? receiver)
        {
            var result = new List<TOut>();

            foreach (var itemAction in this.actions.Values)
            {
                if (itemAction.IsVisible(receiver) == false)
                {
                    continue;
                }

                var data = itemAction.BuildActionData(receiver);
                if (data == default(TOut))
                {
                    continue;
                }

                result.Add(data);
            }

            return result;
        }

        /// <summary>
        /// Method that is used to register all available item actions.
        /// </summary>
        /// <returns>List of item actions that are available in this item.</returns>
        protected abstract IEnumerable<IItemAction<TOut, TIn>> RegisterAllActions();

        /// <inheritdoc />
        protected override void SetupItem()
        {
            base.SetupItem();

            this.SetupItemActions();
        }

        private void SetupItemActions()
        {
            var registeredActions = this.RegisterAllActions();
            if (registeredActions == null)
            {
                throw new InvalidActionException($"The method {nameof(this.RegisterAllActions)} returned null.");
            }

            foreach (var action in registeredActions)
            {
                if (action == null)
                {
                    throw new InvalidActionException($"The list returned by {nameof(this.RegisterAllActions)} contained null.");
                }

                if (action.RuntimeId == Guid.Empty)
                {
                    throw new InvalidActionException($"The list returned by {nameof(this.RegisterAllActions)} contained an item with empty guid. ");
                }

                this.actions.TryAdd(action.RuntimeId, action);
            }
        }
    }
}
