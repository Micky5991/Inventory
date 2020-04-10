using System.Threading.Tasks;

namespace Micky5991.Inventory.Interfaces.Strategy
{
    /// <summary>
    /// Strategy that specifies the behavior how to split an item into a second one.
    /// </summary>
    public interface IItemSplitStrategy : IStrategy
    {
        /// <summary>
        /// Executes a split from <paramref name="oldItem"/> into the item <paramref name="newItem"/>.
        /// </summary>
        /// <param name="oldItem">Instance of <see cref="IItem"/> to extract the data from.</param>
        /// <param name="newItem">Instance of <see cref="IItem"/> to apply the splitted data to.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SplitItemAsync(IItem oldItem, IItem newItem);
    }
}
