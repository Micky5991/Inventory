using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    /// <summary>
    /// Strategy that defines certain behavior of the merging process of two <see cref="IItem"/> instances.
    /// </summary>
    [PublicAPI]
    public interface IItemMergeStrategy : IStrategy
    {
        /// <summary>
        /// Returns if the two given <see cref="IItem"/> instances can be merged or not.
        /// </summary>
        /// <param name="targetItem">Instance of <see cref="IItem"/> that should merge <paramref name="sourceItem"/> into itself.</param>
        /// <param name="sourceItem">Instance of <see cref="IItem"/> that privides any needed data.</param>
        /// <returns>true if those given items can be merged.</returns>
        bool CanBeMerged(IItem targetItem, IItem sourceItem);

        /// <summary>
        /// Executes the actual merge process of <paramref name="sourceItem"/> into <paramref name="targetItem"/>.
        /// </summary>
        /// <param name="targetItem">Instance of <see cref="IItem"/> that should merge <paramref name="sourceItem"/> into itself.</param>
        /// <param name="sourceItem">Instance of <see cref="IItem"/> that privides any needed data.</param>
        void MergeItemWith(IItem targetItem, IItem sourceItem);
    }
}
