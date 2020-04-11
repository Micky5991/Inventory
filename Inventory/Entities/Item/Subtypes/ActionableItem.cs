using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Data;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Entities.Item.Subtypes
{
    /// <summary>
    /// Item that is able to hold actions to interact with.
    /// </summary>
    /// <typeparam name="TOut">Outgoing data type for item actions.</typeparam>
    /// <typeparam name="TIn">Incoming data type for item action execution.</typeparam>
    public abstract class ActionableItem<TOut, TIn> : Item, IItemActionContainer<TOut, TIn>
        where TOut : ItemActionData
    {
        private readonly ConcurrentDictionary<Guid, IItemAction<TOut, TIn>> actions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionableItem{TOut, TIn}"/> class.
        /// </summary>
        /// <param name="meta">Non-NULL instance of the <see cref="ItemMeta"/> that is represented by this instance.</param>
        /// <param name="itemServices">Non-NULL instance of <see cref="AggregatedItemServices"/> which are necessary for this <see cref="Item"/>.</param>
        protected ActionableItem(ItemMeta meta, AggregatedItemServices itemServices)
            : base(meta, itemServices)
        {
            this.actions = new ConcurrentDictionary<Guid, IItemAction<TOut, TIn>>();
        }

        /// <inheritdoc />
        public void ExecuteAction(Guid actionRuntimeId, TIn data)
        {
            if (actionRuntimeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(actionRuntimeId));
            }

            if (this.actions.TryGetValue(actionRuntimeId, out var action) == false)
            {
                throw new ItemActionNotFoundException($"Could not find item action with id {actionRuntimeId}.");
            }

            action.Execute(data);
        }

        /// <inheritdoc />
        public ICollection<TOut> GetAllActionData()
        {
            var result = new List<TOut>();

            foreach (var itemAction in this.actions.Values)
            {
                result.Add(itemAction.BuildActionData());
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
            foreach (var action in this.RegisterAllActions())
            {
                this.actions.TryAdd(action.RuntimeId, action);
            }
        }
    }
}
