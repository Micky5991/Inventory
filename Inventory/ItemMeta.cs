using System;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    public class ItemMeta
    {

        public string Handle { get; }

        public Type Type { get; }

        public int DefaultWeight { get; }

        public ItemFlags Flags { get; }

        /// <summary>
        /// Creates an instance of <see cref="ItemMeta"/> that gives basic information about an item without creation.
        /// </summary>
        /// <param name="handle">Unique handle of item</param>
        /// <param name="type">Underlying type of item that this item will be implemented of</param>
        /// <param name="defaultWeight">Default item weight of this item.</param>
        /// <param name="flags">Behaviour flags of this item</param>
        /// <exception cref="ArgumentException"><paramref name="type"/> is invalid</exception>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> or <paramref name="type"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="defaultWeight"/> is 0 or lower</exception>
        public ItemMeta(string handle, Type type, int defaultWeight, ItemFlags flags = ItemFlags.None)
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new ArgumentNullException(nameof(handle));
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
            DefaultWeight = defaultWeight;
            Flags = flags;
        }

    }
}
