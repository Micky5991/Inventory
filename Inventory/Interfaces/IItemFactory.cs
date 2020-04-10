using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    /// <summary>
    /// Factory that creates actual implementations for <see cref="IItem"/>.
    /// </summary>
    [PublicAPI]
    public interface IItemFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="IItem"/> and search <see cref="IItemRegistry"/> for the given <paramref name="handle"/>.
        /// </summary>
        /// <param name="handle">Handle to search for.</param>
        /// <param name="amount">Positive amount of items the resulting item shout have.</param>
        /// <returns>Instance of the requested <paramref name="handle"/>, null if no <see cref="ItemMeta"/> has been found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> is null, empty or whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="amount"/> is too low.</exception>
        IItem? CreateItem(string handle, int amount);

        /// <summary>
        /// Creates a list of items from the given <paramref name="handle"/>.
        ///
        /// If the item is not stackable, it will create multiple items with item amount of 1 each.
        /// </summary>
        /// <param name="handle">Item identifier to search for.</param>
        /// <param name="amount">Positive amount of items that should be created.</param>
        /// <returns>Created collection of items, null if no <see cref="ItemMeta"/> could be found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> is null, empty or whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="amount"/> is 0 or lower.</exception>
        ICollection<IItem>? CreateItems(string handle, int amount);

        /// <summary>
        /// Creates an instance of <see cref="IItem"/> from the given <paramref name="meta"/>.
        /// </summary>
        /// <param name="meta"><see cref="ItemMeta"/> to create the item from.</param>
        /// <param name="amount">Positive amount of items the resulting item shout have.</param>
        /// <returns>Instance of the requested <paramref name="meta"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="meta"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="amount"/> is too low.</exception>
        IItem CreateItem(ItemMeta meta, int amount);

        /// <summary>
        /// Creates a list of items from the given <see cref="ItemMeta"/>.
        /// If the item is not stackable, multiple items will be created with amount of 1 each.
        /// </summary>
        /// <param name="meta">Definition of the item that should be created.</param>
        /// <param name="amount">Positive amount of items that should be created.</param>
        /// <returns>The list of created items.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="meta"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="amount"/> is 0 or lower.</exception>
        ICollection<IItem> CreateItems(ItemMeta meta, int amount);
    }
}
