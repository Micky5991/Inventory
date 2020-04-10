using System;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    /// <summary>
    /// This type gives basic information about an item without creation.
    /// </summary>
    public class ItemMeta
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemMeta"/> class.
        /// </summary>
        /// <param name="handle">Unique handle of item.</param>
        /// <param name="type">Underlying type of item that this item will be implemented of.</param>
        /// <param name="displayName">Default display name this item should have.</param>
        /// <param name="defaultWeight">Default item weight of this item.</param>
        /// <param name="flags">Behaviour flags of this item.</param>
        /// <exception cref="ArgumentException"><paramref name="type"/> is invalid.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> or <paramref name="type"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="defaultWeight"/> is 0 or lower.</exception>
        public ItemMeta(string handle, Type type, string displayName, int defaultWeight, ItemFlags flags = ItemFlags.None)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new ArgumentNullException(nameof(handle));
            }

            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentNullException(nameof(displayName));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (typeof(IItem).IsAssignableFrom(type) == false)
            {
                throw new ArgumentException($"The given item type does not implement {typeof(IItem)}.", nameof(type));
            }

            if (defaultWeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(defaultWeight), "The given defaultWeight has to be 1 or higher");
            }

            this.Handle = handle;
            this.Type = type;
            this.DisplayName = displayName;
            this.DefaultWeight = defaultWeight;
            this.Flags = flags;
        }

        /// <summary>
        /// Gets the unique handle of this meta information.
        /// </summary>
        public string Handle { get; }

        /// <summary>
        /// Gets the implementation type of the item this meta will be represented by.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the default display name of the created item.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the weight of the final item.
        /// </summary>
        public int DefaultWeight { get; }

        /// <summary>
        /// Gets the flag collection that represent different item abilities.
        /// </summary>
        public ItemFlags Flags { get; }
    }
}
