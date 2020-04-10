using System;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    [PublicAPI]
    public interface IInventoryFactory
    {
        /// <summary>
        /// Creates a new <see cref="IInventory"/> instance.
        /// </summary>
        /// <param name="capacity"></param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is below 0</exception>
        /// <returns>Newly created <see cref="IInventory"/></returns>
        IInventory CreateInventory(int capacity);
    }
}
