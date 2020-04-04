using System;
using System.Threading.Tasks;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Strategies.Handlers
{
    public class ItemSplitStrategyHandler : StrategyHandler<IItemSplitStrategy>, IItemSplitStrategyHandler
    {
        public async Task SplitItemAsync(IItem oldItem, IItem newItem)
        {
            if (oldItem == null)
            {
                throw new ArgumentNullException(nameof(oldItem));
            }

            if (newItem == null)
            {
                throw new ArgumentNullException(nameof(newItem));
            }

            foreach (var strategy in this)
            {
                await strategy.SplitItemAsync(oldItem, newItem);
            }
        }
    }
}
