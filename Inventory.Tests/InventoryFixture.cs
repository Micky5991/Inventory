using System;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public partial class InventoryFixture
    {
        private const int InventoryCapacity = 100;

        private Entities.Inventory.Inventory _inventory;

        private Mock<IItem> _itemMock;

        [TestInitialize]
        public void Setup()
        {
            _inventory = new Entities.Inventory.Inventory(InventoryCapacity);

            _itemMock = new Mock<IItem>();
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

        [TestMethod]
        public async Task AddingItemWillSetCurrentInventoryCorrectly()
        {
            _itemMock.Setup(x => x.SetCurrentInventory(_inventory));

            await _inventory.InsertItemAsync(_itemMock.Object);

            _itemMock.Verify(x => x.SetCurrentInventory(_inventory));
        }

        [TestMethod]
        public async Task RemovingItemWillSetCurrentInventoryToNull()
        {
            _itemMock.Setup(x => x.SetCurrentInventory(It.IsAny<IInventory>()));

            await _inventory.InsertItemAsync(_itemMock.Object);
            await _inventory.RemoveItemAsync(_itemMock.Object);

            _itemMock.Verify(x => x.SetCurrentInventory(null), Times.Once);
        }

        [TestMethod]
        public async Task InsertingItemIntoInventoryWillRemoveItemFromOldInventoryFirst()
        {
            var otherInventoryMock = new Mock<IInventory>();

            _itemMock
                .SetupGet(x => x.CurrentInventory)
                .Returns(otherInventoryMock.Object);

            _itemMock
                .Setup(x => x.SetCurrentInventory(_inventory));

            otherInventoryMock
                .Setup(x => x.RemoveItemAsync(_itemMock.Object))
                .ReturnsAsync(true);

            await _inventory.InsertItemAsync(_itemMock.Object);

            _itemMock.VerifyGet(x => x.CurrentInventory, Times.AtLeastOnce);
            otherInventoryMock.Verify(x => x.RemoveItemAsync(_itemMock.Object), Times.Once);

            _itemMock.Verify(x => x.SetCurrentInventory(_inventory), Times.Once);
        }

        [TestMethod]
        public async Task FailingRemovingOldInventoryWillResultInFailedInsertion()
        {
            var otherInventoryMock = new Mock<IInventory>();

            _itemMock
                .SetupGet(x => x.CurrentInventory)
                .Returns(otherInventoryMock.Object);

            otherInventoryMock
                .Setup(x => x.RemoveItemAsync(_itemMock.Object))
                .ReturnsAsync(false);

            var success = await _inventory.InsertItemAsync(_itemMock.Object);

            success.Should().BeFalse();

            _itemMock.Verify(x => x.SetCurrentInventory(It.IsAny<IInventory>()), Times.Never);
        }

        [TestMethod]
        public async Task AddingItemWillNotifyUsedCapacityAndAvailableCapacity()
        {
            using var monitoredInventory = _inventory.Monitor();

            await AddItemToInventoryAsync(10);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.UsedCapacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task RemovingItemWillNotifyUsedCapacityAndAvailableCapacity()
        {
            var item = await AddItemToInventoryAsync(10);

            using var monitoredInventory = _inventory.Monitor();

            await _inventory.RemoveItemAsync(item);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.UsedCapacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task ChangingCapacityOfInventoryWillNotify()
        {
            var item = await AddItemToInventoryAsync(10);

            using var monitoredInventory = _inventory.Monitor();

            _inventory.SetCapacity(20);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.Capacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task SettingCapacityOfInventoryWontNotify()
        {
            await AddItemToInventoryAsync(10);

            using var monitoredInventory = _inventory.Monitor();

            _inventory.SetCapacity(_inventory.Capacity);

            monitoredInventory.Should().NotRaisePropertyChangeFor(x => x.Capacity);
            monitoredInventory.Should().NotRaisePropertyChangeFor(x => x.AvailableCapacity);
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
