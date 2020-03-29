using System;
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
        [DataRow(Inventory.MinimalInventoryCapacity)]
        [DataRow(Inventory.MinimalInventoryCapacity + 1)]
        [DataRow(Inventory.MinimalInventoryCapacity + 10)]
        [DataRow(Inventory.MinimalInventoryCapacity + 100)]
        [DataRow(int.MaxValue)]
        public void SetsCorrectInitialCapacityToInventory(int capacity)
        {
            var inventory = _inventoryFactory.CreateInventory(capacity);

            inventory.Capacity.Should().Be(capacity);
        }

        [TestMethod]
        [DataRow(Inventory.MinimalInventoryCapacity - 1)]
        [DataRow(Inventory.MinimalInventoryCapacity - 2)]
        [DataRow(int.MinValue)]
        public void CreatingInventoryWithInvalidWeightWillThrowException(int capacity)
        {
            Action act = () => _inventoryFactory.CreateInventory(capacity);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
