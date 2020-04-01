using System.Linq;
using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Strategies.Handlers
{
    public class ItemMergeStrategyHandler : StrategyHandler<IItemMergeStrategy>, IItemMergeStrategyHandler
    {
        public bool CanBeMerged(IItem targetItem, IItem sourceItem)
        {
            return GetAllStrategies().All(x => x.CanBeMerged(targetItem, sourceItem));
        }

        public async Task MergeItemWithAsync(IItem targetItem, IItem sourceItem)
        {
            foreach (var strategy in GetAllStrategies())
            {
                await strategy.MergeItemWithAsync(targetItem, sourceItem);
            }
        }
    }
}
