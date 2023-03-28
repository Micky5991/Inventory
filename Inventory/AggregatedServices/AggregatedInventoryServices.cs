using CommunityToolkit.Diagnostics;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.AggregatedServices
{
    /// <summary>
    /// Simplified aggregation of services an item has.
    /// </summary>
    public class AggregatedInventoryServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatedInventoryServices"/> class.
        /// </summary>
        /// <param name="itemRegistry">Non-null instance of <see cref="IItemRegistry"/>.</param>
        public AggregatedInventoryServices(IItemRegistry itemRegistry)
        {
            Guard.IsNotNull(itemRegistry);

            this.ItemRegistry = itemRegistry;
        }

        /// <summary>
        /// Gets an instance of <see cref="IItemRegistry"/>.
        /// </summary>
        public IItemRegistry ItemRegistry { get; }
    }
}
