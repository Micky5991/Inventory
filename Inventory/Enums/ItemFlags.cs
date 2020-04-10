using System;

namespace Micky5991.Inventory.Enums
{
    /// <summary>
    /// Specifies which certain behaviors an item should have.
    /// </summary>
    [Flags]
    public enum ItemFlags : uint
    {
        /// <summary>
        /// This item has default behavior.
        /// </summary>
        None = 0,

        /// <summary>
        /// An item with this flag is not stackable with other items and only has an amount of 1.
        /// </summary>
        NotStackable = 1,

        /// <summary>
        /// The weight of this item is changable and weight calculations could be inaccurate before creation.
        /// </summary>
        WeightChangable = 1 << 1,
    }
}
