using System;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class InventoryFixture
    {
        private const int InventoryCapacity = 100;

        private Inventory _inventory;

        [TestInitialize]
        public void Setup()
        {
            _inventory = new Inventory(InventoryCapacity);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        [DataRow(int.MinValue)]
        public void CreatingInventoryWithInvalidCapacityThrowsException(int capacity)
        {
            Action act = () => new Inventory(capacity);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(100)]
        [DataRow(int.MaxValue)]
        public void CreatingInventoryWithCapacityWillSetCapacityValues(int capacity)
        {
            var inventory = new Inventory(capacity);

            AssertInventoryCapacity(0, capacity, inventory);
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
        public async Task RemovingNullFromInventoryWillThrowException()
        {
            Func<Task> act = () => _inventory.RemoveItemAsync(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task InsertingNullFromInventoryWillThrowException()
        {
            Func<Task> act = () => _inventory.InsertItemAsync(null);

            (await act.Should().ThrowAsync<ArgumentNullException>())
                .Where(x => string.IsNullOrWhiteSpace(x.Message) == false);
        }

        [TestMethod]
        public async Task InsertedItemsWillAppearInItemsList()
        {
            var item = await AddItemToInventoryAsync();

            _inventory.Items.Should()
                .ContainSingle(x => x.Key == item.RuntimeId && x.Value == item);
        }

        [TestMethod]
        public async Task InsertingItemMultipleTimesWillNotChangeDictionaryLength()
        {
            var item = await AddItemToInventoryAsync();

            await _inventory.InsertItemAsync(item);

            _inventory.Items.Should()
                .ContainSingle(x => x.Key == item.RuntimeId && x.Value == item);
        }

        [TestMethod]
        public async Task InsertingItemToInventoryTwiceWillMarkFollowingCallsAsUnsuccessful()
        {
            var item = new FakeItem(10);

            (await _inventory.InsertItemAsync(item)).Should().BeTrue();
            (await _inventory.InsertItemAsync(item)).Should().BeFalse();
        }

        [TestMethod]
        public async Task RemovingItemFromInventoryWillReturnFalseIfNotFound()
        {
            var item = await AddItemToInventoryAsync();

            (await _inventory.RemoveItemAsync(item)).Should().BeTrue();
            (await _inventory.RemoveItemAsync(item)).Should().BeFalse();
        }

        [TestMethod]
        public async Task RemovingItemFromInventoryThatWasNotAddedWillReturnFalse()
        {
            var item = new FakeItem(10);

            (await _inventory.RemoveItemAsync(item)).Should().BeFalse();
        }

        [TestMethod]
        public async Task AddingItemWithHigherWeightThanCapacityWillThrowException()
        {
            var item = new FakeItem(InventoryCapacity + 1);

            Func<Task> act = () => _inventory.InsertItemAsync(item);

            (await act.Should().ThrowAsync<InventoryCapacityException>())
                .Where(x =>
                            x.Message.Contains(item.Handle) &&
                            x.Message.Contains(item.RuntimeId.ToString())
                            );
        }

        [TestMethod]
        public async Task AddingInvalidItemWillNotAddItemToInventory()
        {
            var item = new FakeItem(InventoryCapacity + 1);

            Func<Task> act = () => _inventory.InsertItemAsync(item);

            await act.Should().ThrowAsync<InventoryCapacityException>();

            _inventory.Items.Should().BeEmpty();
        }

        [TestMethod]
        public async Task CheckIfItemFitsWillReturnCorrectValuesAfterItemCollectionChange()
        {
            var item = await AddItemToInventoryAsync(InventoryCapacity);

            _inventory.DoesItemFit(item).Should().BeFalse();
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

        private void AssertInventoryCapacity(int usedCapacity, int capacity = InventoryCapacity, Inventory inventory = null)
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
