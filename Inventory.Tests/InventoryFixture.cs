using System;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public partial class InventoryFixture : ItemTest
    {
        private const int InventoryCapacity = 100;

        private Entities.Inventory.Inventory _inventory;

        private Mock<IItem> _itemMock;

        [TestInitialize]
        public void Setup()
        {
            SetupItemTest();

            _inventory = new Entities.Inventory.Inventory(InventoryCapacity, _inventoryServices);

            _itemMock = new Mock<IItem>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            TearDownItemTest();
        }

        [TestMethod]
        public async Task InsertingItemToInventoryWillChangeCapacity()
        {
            await AddItemToInventoryAsync(10);

            AssertInventoryCapacity(10);
        }

        [TestMethod]
        public async Task RemovingItemFromInventoryWillChangeCapacity()
        {
            var item = await AddItemToInventoryAsync(10);

            AssertInventoryCapacity(10);

            await _inventory.RemoveItemAsync(item);

            AssertInventoryCapacity(0);
        }

        [TestMethod]
        public async Task CheckIfItemFitsWillReturnCorrectValuesAfterItemCollectionChange()
        {
            var item = await AddItemToInventoryAsync(InventoryCapacity);

            _inventory.DoesItemFit(item).Should().BeFalse();
        }

        [TestMethod]
        public void PassingNullToDoesItemFitWillThrowException()
        {
            Action act = () => _inventory.DoesItemFit(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CheckingIfFillingItemWillFitIntoInventoryShouldReturnTrue()
        {
            var item = new FakeItem(InventoryCapacity);

            _inventory.DoesItemFit(item).Should().BeTrue();
        }

        [TestMethod]
        public async Task CheckingIfSecondFillingItemWillFitIntoInventoryShouldReturnTrue()
        {
            await AddItemToInventoryAsync(InventoryCapacity - 10);

            var item = new FakeItem(10);

            _inventory.DoesItemFit(item).Should().BeTrue();
        }

        [TestMethod]
        public async Task CheckingIfSecondCapacityExceedingItemWillFitIntoInventoryShouldReturnFalse()
        {
            await AddItemToInventoryAsync(InventoryCapacity - 10);

            var item = new FakeItem(11);

            _inventory.DoesItemFit(item).Should().BeFalse();
        }

        private async Task<FakeItem> AddItemToInventoryAsync(int weight = 10)
        {
            var item = new FakeItem(weight);

            await _inventory.InsertItemAsync(item);

            return item;
        }

        private void AssertInventoryCapacity(int usedCapacity, int capacity = InventoryCapacity, Entities.Inventory.Inventory inventory = null)
        {
            if (inventory == null)
            {
                inventory = _inventory;
            }

            inventory.Capacity.Should().Be(capacity);
            inventory.UsedCapacity.Should().Be(usedCapacity);
            inventory.AvailableCapacity.Should().Be(capacity - usedCapacity);
        }

    }
}
