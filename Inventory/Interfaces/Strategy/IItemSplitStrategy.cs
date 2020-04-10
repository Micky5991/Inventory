using System.Threading.Tasks;

namespace Micky5991.Inventory.Interfaces.Strategy
{
    public interface IItemSplitStrategy : IStrategy
    {
        Task SplitItemAsync(IItem oldItem, IItem newItem);
    }
}
