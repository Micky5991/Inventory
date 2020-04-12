using System;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Exceptions
{
    /// <summary>
    /// Exception expresses that any <see cref="IItemAction{TOut,TIn}"/> is invalid.
    /// </summary>
    public class InvalidActionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidActionException"/> class.
        /// </summary>
        /// <param name="message">Message that explains why the action is invalid.</param>
        public InvalidActionException(string message)
            : base(message)
        {
        }
    }
}
