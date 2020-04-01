using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Strategies
{
    public class BasicItemMergeStrategy : IItemMergeStrategy
    {
        public bool CanBeMerged(IItem targetItem, IItem sourceItem)
        {
            return targetItem.Handle == sourceItem.Handle
                   && sourceItem.Amount > 0
                   && targetItem.Stackable && sourceItem.Stackable
                   && targetItem.SingleWeight == sourceItem.SingleWeight;
        }

        public Task MergeItemWithAsync(IItem targetItem, IItem sourceItem)
        {
            throw new System.NotImplementedException();
        }
    }
}
