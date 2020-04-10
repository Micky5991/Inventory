using System;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    /// <summary>
    /// Factory that creates a specific implementation of <see cref="IInventory"/>.
    /// </summary>
    [PublicAPI]
    public interface IInventoryFactory
    {
        /// <summary>
        /// Creates a new <see cref="IInventory"/> instance.
        /// </summary>
        /// <param name="capacity">Capacity limit the inventory should have.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is below 0.</exception>
        /// <returns>Newly created <see cref="IInventory"/>.</returns>
        IInventory CreateInventory(int capacity);
    }
}
