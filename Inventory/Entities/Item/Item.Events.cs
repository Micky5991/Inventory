using System;
using Micky5991.Inventory.EventArgs;

namespace Micky5991.Inventory.Entities.Item
{
    /// <content>
    /// Handlers for certain general events for items.
    /// </content>
    public partial class Item
    {
        /// <summary>
        /// Event that triggers after item has been initialized for the first time.
        /// </summary>
        public event EventHandler<ItemInitializedEventArgs>? Initialized;
    }
}
