using System;

namespace Micky5991.Inventory.Exceptions
{
    /// <summary>
    /// Exception expresses that the <see cref="ItemMeta"/> could not be found.
    /// </summary>
    public class ItemMetaNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMetaNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Message that describes more details which data was searched for.</param>
        public ItemMetaNotFoundException(string message)
            : base(message)
        {
        }
    }
}
