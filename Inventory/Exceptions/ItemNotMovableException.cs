using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Exceptions
{
    /// <summary>
    /// Exception that expresses that the item is currently unable to move.
    /// </summary>
    public class ItemNotMovableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemNotMovableException"/> class.
        /// </summary>
        /// <param name="item">Instance of the item which is not movable.</param>
        public ItemNotMovableException(IItem item)
            : base($"The item {item.Handle} ({item.RuntimeId}) is not movable")
        {
        }
    }
}
