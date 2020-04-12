using System;

namespace Micky5991.Inventory.Data
{
    /// <summary>
    /// Data which is usable to display an item action in a user interface.
    /// </summary>
    public class OutogingItemActionData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutogingItemActionData"/> class.
        /// </summary>
        /// <param name="actionRuntimeId">RuntimeId of the action.</param>
        public OutogingItemActionData(Guid actionRuntimeId)
        {
            this.ActionRuntimeId = actionRuntimeId;
        }

        /// <summary>
        /// Gets the runtime id of the item action that is represented by this data instance.
        /// </summary>
        public Guid ActionRuntimeId { get; }
    }
}