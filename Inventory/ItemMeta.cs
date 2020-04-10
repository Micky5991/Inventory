using System;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    public class ItemMeta
    {
        /// <summary>
        /// Unique handle of this meta information
        /// </summary>
        public string Handle { get; }

        /// <summary>
        /// Implementation type of the item this meta will be represented by.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Default display name of the created item
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Weight of the final item
        /// </summary>
        public int DefaultWeight { get; }

        /// <summary>
        /// Flag collection that represent different item abilities
        /// </summary>
        public ItemFlags Flags { get; }

        /// <summary>
        /// Creates an instance of <see cref="ItemMeta"/> that gives basic information about an item without creation.
        /// </summary>
        /// <param name="handle">Unique handle of item</param>
        /// <param name="type">Underlying type of item that this item will be implemented of</param>
        /// <param name="displayName">Default display name this item should have</param>
        /// <param name="defaultWeight">Default item weight of this item.</param>
        /// <param name="flags">Behaviour flags of this item</param>
        /// <exception cref="ArgumentException"><paramref name="type"/> is invalid</exception>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> or <paramref name="type"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="defaultWeight"/> is 0 or lower</exception>
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

            Handle = handle;
            Type = type;
            DisplayName = displayName;
            DefaultWeight = defaultWeight;
            Flags = flags;
        }
    }
}
