using System;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    [PublicAPI]
    public interface IItemFactory
    {

        /// <summary>
        /// Creates an instance of <see cref="IItem"/> and search <see cref="IItemRegistry"/> for the given <paramref name="handle"/>.
        /// </summary>
        /// <param name="handle">Handle to search for</param>
        /// <param name="amount">Positive amount of items the resulting item shout have</param>
        /// <returns>Instance of the requested <paramref name="handle"/>, null if no <see cref="ItemMeta"/> has been found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> is null, empty or whitespace</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="amount"/> is too low</exception>
        IItem? CreateItem(string handle, int amount);

        /// <summary>
        /// Creates an instance of <see cref="IItem"/> from the given <paramref name="meta"/>.
        /// </summary>
        /// <param name="meta"><see cref="ItemMeta"/> to create the item from</param>
        /// <param name="amount">Positive amount of items the resulting item shout have</param>
        /// <returns>Instance of the requested <paramref name="meta"/></returns>
        /// <exception cref="ArgumentNullException"><paramref name="meta"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="amount"/> is too low</exception>
        IItem CreateItem(ItemMeta meta, int amount);

    }
}
