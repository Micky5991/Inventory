using System;

namespace Micky5991.Inventory
{
    /// <summary>
    /// Data which is usable to display an item action in a user interface.
    /// </summary>
    public class ItemActionData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemActionData"/> class.
        /// </summary>
        /// <param name="actionRuntimeId">RuntimeId of the action.</param>
        public ItemActionData(Guid actionRuntimeId)
        {
            this.ActionRuntimeId = actionRuntimeId;
        }

        /// <summary>
        /// Gets the runtime id of the item action that is represented by this data instance.
        /// </summary>
        public Guid ActionRuntimeId { get; }
    }
}
