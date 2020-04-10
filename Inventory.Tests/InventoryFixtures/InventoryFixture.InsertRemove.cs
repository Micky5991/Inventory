using System;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests.InventoryFixtures
{
    public partial class InventoryFixture
    {
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
                .ContainSingle(x => x == item);
        }

        [TestMethod]
        public async Task InsertingItemMultipleTimesWillNotChangeDictionaryLength()
        {
            var item = await AddItemToInventoryAsync();

            await _inventory.InsertItemAsync(item);

            _inventory.Items.Should()
                .ContainSingle(x => x == item);
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
        public async Task RemovingItemFromInventoryWhenItemIsLockedThrowsException()
        {
            await _inventory.InsertItemAsync(_item);

            _item.MovingLocked = true;

            Func<Task> act = () => _inventory.RemoveItemAsync(_item);

            (await act.Should().ThrowAsync<ItemNotMovableException>())
                .Where(x =>
                    x.Message.Contains(_item.Handle)
                    && x.Message.Contains(_item.RuntimeId.ToString())
                    && x.Message.Contains("not movable"));

            _inventory.Items.Should()
                .ContainSingle(x => x == _item);
        }

        [TestMethod]
        public async Task AddingItemWithSameRuntimeKeyWillReturnFalse()
        {
            var item = new FakeItem(10);
            var otherFake = new FakeItem(10);

            otherFake.RuntimeId = item.RuntimeId;
            item.IsMergableCheck = x => false;

            (await _inventory.InsertItemAsync(item)).Should().BeTrue();
            (await _inventory.InsertItemAsync(otherFake)).Should().BeFalse();

            otherFake.CurrentInventory.Should().BeNull();
        }

        [TestMethod]
        public async Task AddingItemWithCurrentInventoryAlreadyToTargetInventorySetWillReturnFalse()
        {
            var item = new FakeItem(10);

            item.SetCurrentInventory(_inventory);

            (await _inventory.InsertItemAsync(item)).Should().BeFalse();
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
        public async Task AddingInMovableItemThrowsException()
        {
            _item.MovingLocked = true;

            Func<Task> act = () => _inventory.InsertItemAsync(_item);

            (await act.Should().ThrowAsync<ItemNotMovableException>())
                .Where(x =>
                    x.Message.Contains(_item.Handle)
                    && x.Message.Contains(_item.RuntimeId.ToString())
                    && x.Message.Contains("not movable"));

            _inventory.Items.Should().BeEmpty();
        }

        [TestMethod]
        public async Task InsertingItemThatIsNotAllwedThrowsException()
        {
            _inventory.SetItemFilter(x => false);

            Func<Task> act = () => _inventory.InsertItemAsync(_item);

            await act.Should().ThrowAsync<ItemNotAllowedException>();
        }

        [TestMethod]
        public async Task InsertingItemWithForceEvenWithDisallowingFilterInsertsIt()
        {
            _inventory.SetItemFilter(x => false);

            await _inventory.InsertItemAsync(_item, true);

            _inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == _item);
        }

        [TestMethod]
        public async Task InsertingItemWithForceEvenWithNotEnoughCapacityShouldAllowInsertion()
        {
            _item.SetSingleWeight(10);
            _inventory.SetCapacity(5);

            await _inventory.InsertItemAsync(_item, true);

            _inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == _item);
        }

        [TestMethod]
        public async Task InsertingItemWithForceEvenInmovableItemShouldAllowInsertion()
        {
            _item.MovingLocked = true;

            await _inventory.InsertItemAsync(_item, true);

            _inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == _item);
        }
    }
}
