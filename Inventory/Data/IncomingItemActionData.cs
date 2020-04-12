using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Data
{
    /// <summary>
    /// Data container to pass to the action with arguments how to run the action.
    /// </summary>
    public class IncomingItemActionData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncomingItemActionData"/> class.
        /// </summary>
        /// <param name="actionRuntimeId">RuntimeId of the action to run.</param>
        public IncomingItemActionData(Guid actionRuntimeId)
        {
            this.ActionRuntimeId = actionRuntimeId;
        }

        /// <summary>
        /// Gets the runtimeId of the <see cref="IItemAction{TOut,TIn}"/> that should be run.
        /// </summary>
        public Guid ActionRuntimeId { get; }
    }
}
