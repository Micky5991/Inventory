using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Data
{
    /// <summary>
    /// Data which is usable to display an item action in a user interface.
    /// </summary>
    public class OutgoingItemActionData : ItemActionData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingItemActionData"/> class.
        /// </summary>
        /// <param name="actionRuntimeId">Guid of the related <see cref="IItemAction{TOut,TIn}"/> instance.</param>
        public OutgoingItemActionData(Guid actionRuntimeId)
            : base(actionRuntimeId)
        {
        }
    }
}
