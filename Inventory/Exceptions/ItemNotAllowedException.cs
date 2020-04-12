using System;

namespace Micky5991.Inventory.Exceptions
{
    /// <summary>
    /// Exception that expresses that the given item is not allowed in this inventory.
    /// </summary>
    public class ItemNotAllowedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemNotAllowedException"/> class.
        /// </summary>
        public ItemNotAllowedException()
            : base("The given item is not allowed in this inventory.")
        {
        }
    }
}
