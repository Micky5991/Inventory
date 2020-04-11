using System;
using FluentAssertions;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Factories;
using Micky5991.Inventory.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class InventoryFactoryFixture
    {

        private InventoryFactory inventoryFactory;
        private AggregatedInventoryServices inventoryServices;

        private Mock<IItemRegistry> itemRegistry;

        [TestInitialize]
        public void Setup()
        {
            this.itemRegistry = new Mock<IItemRegistry>();
            this.inventoryServices = new AggregatedInventoryServices(this.itemRegistry.Object);

            this.inventoryFactory = new InventoryFactory(this.inventoryServices);
        }

        [TestMethod]
        public void InventoryFactoryCreatesInventory()
        {
            var inventory = this.inventoryFactory.CreateInventory(1);

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
            var inventory = this.inventoryFactory.CreateInventory(capacity);

            inventory.Capacity.Should().Be(capacity);
        }

        [TestMethod]
        public void CreatingInventoryShouldHaveRuntimeId()
        {
            var inventory = this.inventoryFactory.CreateInventory(1);

            inventory.RuntimeId.Should().NotBeEmpty();
        }

        [TestMethod]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity - 1)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity - 2)]
        [DataRow(int.MinValue)]
        public void CreatingInventoryWithInvalidWeightWillThrowException(int capacity)
        {
            Action act = () => this.inventoryFactory.CreateInventory(capacity);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
