using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Data
{
    /// <summary>
    /// Base class that repreents any data related to <see cref="IItemAction{TOut,TIn}"/>.
    /// </summary>
    public abstract class ItemActionData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemActionData"/> class.
        /// </summary>
        /// <param name="actionRuntimeId">Guid of the related <see cref="IItemAction{TOut,TIn}"/> instance.</param>
        protected ItemActionData(Guid actionRuntimeId)
        {
            if (actionRuntimeId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(actionRuntimeId));
            }

            this.ActionRuntimeId = actionRuntimeId;
        }

        /// <summary>
        /// Gets the RuntimeId of the related <see cref="IItemAction{TOut,TIn}"/> of this payload.
        /// </summary>
        public Guid ActionRuntimeId { get; }
    }
}
