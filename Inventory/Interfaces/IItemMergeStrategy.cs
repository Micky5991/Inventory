using System.Threading.Tasks;

namespace Micky5991.Inventory.Interfaces
{
    public interface IItemMergeStrategy : IStrategy
    {
        bool CanBeMerged(IItem targetItem, IItem sourceItem);
        Task MergeItemWithAsync(IItem targetItem, IItem sourceItem);
    }
}
