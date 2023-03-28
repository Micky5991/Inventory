using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Interfaces;

[assembly:InternalsVisibleTo("Micky5991.Inventory.Tests")]

namespace Micky5991.Inventory.Entities.Factories
{
    /// <inheritdoc />
    public class InventoryFactory : IInventoryFactory
    {
        private readonly AggregatedInventoryServices inventoryServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryFactory"/> class.
        /// </summary>
        /// <param name="inventoryServices">Services which are required to run a default <see cref="Inventory"/> instance.</param>
        public InventoryFactory(AggregatedInventoryServices inventoryServices)
        {
            Guard.IsNotNull(inventoryServices);

            this.inventoryServices = inventoryServices;
        }

        /// <inheritdoc />
        public IInventory CreateInventory(int capacity)
        {
            Guard.IsGreaterThanOrEqualTo(capacity, 0);

            return new Entities.Inventory.Inventory(capacity, this.inventoryServices);
        }
    }
}
