using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory
{
    /// <summary>
    /// Collection of available delegates of this project.
    /// </summary>
    public static class InventoryDelegates
    {
        /// <summary>
        /// Delegate to specify if the given <paramref name="item"/> matches these requirements.
        /// </summary>
        /// <param name="item">Instance of <see cref="IItem"/> to check.</param>
        /// <returns>true if requirements are met, false otherwise.</returns>
        public delegate bool ItemFilterDelegate(IItem item);
    }
}
