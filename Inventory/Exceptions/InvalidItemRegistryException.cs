using System;

namespace Micky5991.Inventory.Exceptions
{
    /// <summary>
    /// Exception that expresses a violation of a setup rule of <see cref="ItemMeta"/>. See the exception message for more details.
    /// </summary>
    public class InvalidItemRegistryException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidItemRegistryException"/> class.
        /// </summary>
        /// <param name="message">Message that describes the violation.</param>
        public InvalidItemRegistryException(string message)
            : base(message)
        {
        }
    }
}
