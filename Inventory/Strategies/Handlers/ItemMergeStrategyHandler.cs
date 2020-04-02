using System;
using System.Linq;
using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Strategies.Handlers
{
    public class ItemMergeStrategyHandler : StrategyHandler<IItemMergeStrategy>, IItemMergeStrategyHandler
    {
        public bool CanBeMerged(IItem targetItem, IItem sourceItem)
        {
            if (targetItem == null)
            {
                throw new ArgumentNullException(nameof(targetItem));
            }

            if (sourceItem == null)
            {
                throw new ArgumentNullException(nameof(sourceItem));
            }

            return this.All(x => x.CanBeMerged(targetItem, sourceItem));
        }

        public async Task MergeItemWithAsync(IItem targetItem, IItem sourceItem)
        {
            if (targetItem == null)
            {
                throw new ArgumentNullException(nameof(targetItem));
            }

            if (sourceItem == null)
            {
                throw new ArgumentNullException(nameof(sourceItem));
            }

            foreach (var strategy in this)
            {
                await strategy.MergeItemWithAsync(targetItem, sourceItem);
            }
        }
    }
}
