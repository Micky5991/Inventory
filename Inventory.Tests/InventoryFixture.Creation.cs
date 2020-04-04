using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    public partial class InventoryFixture
    {
        [TestMethod]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity - 1)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity - 2)]
        [DataRow(int.MinValue)]
        public void CreatingInventoryWithInvalidCapacityThrowsException(int capacity)
        {
            Action act = () => new Entities.Inventory.Inventory(capacity, _inventoryServices);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains($"{Entities.Inventory.Inventory.MinimalInventoryCapacity} or higher"));
        }

        [TestMethod]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity + 1)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity + 2)]
        [DataRow(int.MaxValue)]
        public void CreatingInventoryWithCapacityWillSetCapacityValues(int capacity)
        {
            var inventory = new Entities.Inventory.Inventory(capacity, _inventoryServices);

            AssertInventoryCapacity(0, capacity, inventory);
        }
    }
}
