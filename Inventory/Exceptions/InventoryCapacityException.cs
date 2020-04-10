using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Exceptions
{
    /// <summary>
    /// Exception that expresses that a certain operation would exceed the maximum capacity of an inventory.
    /// </summary>
    public class InventoryCapacityException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryCapacityException"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter that carries the invalid argument.</param>
        /// <param name="item">Instance of the item that would exceed the capacity of an inventory.</param>
        public InventoryCapacityException(string paramName, IItem item)
            : base($"Item {item.RuntimeId} ({item.Handle}) exceeds available inventory capacity", paramName)
        {
        }
    }
}
