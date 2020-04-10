using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    /// <summary>
    /// Registry of <see cref="ItemMeta"/> indexed by item handle.
    /// </summary>
    /// <seealso cref="BaseItemRegistry"/>
    [PublicAPI]
    public interface IItemRegistry
    {
        /// <summary>
        /// Returns a list of all available <see cref="ItemMeta"/> instances available to the inventory framework.
        /// </summary>
        /// <returns>List of <see cref="ItemMeta"/>.</returns>
        ICollection<ItemMeta> GetItemMeta();

        /// <summary>
        /// Returns the instance of <see cref="ItemMeta"/> for the given <paramref name="handle"/>.
        /// </summary>
        /// <param name="handle">Unique handle to search for</param>
        /// <param name="meta">Instance of <see cref="ItemMeta"/> that has been found by <paramref name="handle"/>.</param>
        /// <returns>true if an <see cref="ItemMeta"/> instance has been found, false otherwise</returns>
        /// <exception cref="ArgumentNullException"><paramref name="meta"/> is null, empty or whitespace</exception>
        bool TryGetItemMeta(string handle, out ItemMeta? meta);
    }
}
