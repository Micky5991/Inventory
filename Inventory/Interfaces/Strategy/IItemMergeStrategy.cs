using System.Threading.Tasks;

namespace Micky5991.Inventory.Interfaces.Strategy
{
    /// <summary>
    /// Strategy that defines certain behavior of the merging process of two <see cref="IItem"/> instances.
    /// </summary>
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
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task MergeItemWithAsync(IItem targetItem, IItem sourceItem);
    }
}
