using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Data
{
    /// <summary>
    /// Data container to pass to the action with arguments how to run the action.
    /// </summary>
    public class IncomingItemActionData : ItemActionData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncomingItemActionData"/> class.
        /// </summary>
        /// <param name="actionRuntimeId">Guid of the related <see cref="IItemAction{TOut,TIn}"/> instance.</param>
        public IncomingItemActionData(Guid actionRuntimeId)
            : base(actionRuntimeId)
        {
        }
    }
}
