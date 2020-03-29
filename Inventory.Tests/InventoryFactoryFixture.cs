using FluentAssertions;
using Micky5991.Inventory.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class InventoryFactoryFixture
    {

        private InventoryFactory _inventoryFactory;

        [TestInitialize]
        public void Setup()
        {
            _inventoryFactory = new InventoryFactory();
        }

        [TestMethod]
        public void InventoryFactoryCreatesInventory()
        {
            var inventory = _inventoryFactory.CreateInventory(1);

            inventory.Should()
                .NotBeNull()
                .And.BeAssignableTo<IInventory>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void SetsCorrectInitialCapacityToInventory(int capacity)
        {
            var inventory = _inventoryFactory.CreateInventory(capacity);

            inventory.Capacity.Should().Be(capacity);
        }
    }
}
