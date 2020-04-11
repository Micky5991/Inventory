using System;

namespace Micky5991.Inventory.Exceptions
{
    /// <summary>
    /// Exception expresses that a certain item action could not be found.
    /// </summary>
    public class ItemActionNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemActionNotFoundException"/> class.
        /// </summary>
        /// <param name="message">Message that describes which action could not be found.</param>
        public ItemActionNotFoundException(string message)
            : base(message)
        {
        }
    }
}
