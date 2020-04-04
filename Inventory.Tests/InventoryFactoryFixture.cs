using System;
using FluentAssertions;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class InventoryFactoryFixture
    {

        private InventoryFactory _inventoryFactory;
        private AggregatedInventoryServices _inventoryServices;

        [TestInitialize]
        public void Setup()
        {
            _inventoryFactory = new InventoryFactory(_inventoryServices);
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
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity + 1)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity + 10)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity + 100)]
        [DataRow(int.MaxValue)]
        public void SetsCorrectInitialCapacityToInventory(int capacity)
        {
            var inventory = _inventoryFactory.CreateInventory(capacity);

            inventory.Capacity.Should().Be(capacity);
        }

        [TestMethod]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity - 1)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity - 2)]
        [DataRow(int.MinValue)]
        public void CreatingInventoryWithInvalidWeightWillThrowException(int capacity)
        {
            Action act = () => _inventoryFactory.CreateInventory(capacity);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
