using System;

namespace Micky5991.Inventory.Exceptions
{
    /// <summary>
    /// Exception that expresses that the Item is not stackable and the operation would infringe this rule.
    /// </summary>
    public class ItemNotStackableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemNotStackableException"/> class.
        /// </summary>
        public ItemNotStackableException()
            : base("This operation is not possible, because non-stackable items have to have an mount of 1 or lower")
        {
        }
    }
}
