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
            Func<Task> act = () => this._inventory.RemoveItemAsync(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task InsertingNullFromInventoryWillThrowException()
        {
            Func<Task> act = () => this._inventory.InsertItemAsync(null);

            (await act.Should().ThrowAsync<ArgumentNullException>())
                .Where(x => string.IsNullOrWhiteSpace(x.Message) == false);
        }

        [TestMethod]
        public async Task InsertedItemsWillAppearInItemsList()
        {
            var item = await this.AddItemToInventoryAsync();

            this._inventory.Items.Should()
                .ContainSingle(x => x == item);
        }

        [TestMethod]
        public async Task InsertingItemMultipleTimesWillNotChangeDictionaryLength()
        {
            var item = await this.AddItemToInventoryAsync();

            await this._inventory.InsertItemAsync(item);

            this._inventory.Items.Should()
                .ContainSingle(x => x == item);
        }

        [TestMethod]
        public async Task InsertingItemToInventoryTwiceWillMarkFollowingCallsAsUnsuccessful()
        {
            var item = new FakeItem(10);

            (await this._inventory.InsertItemAsync(item)).Should().BeTrue();
            (await this._inventory.InsertItemAsync(item)).Should().BeFalse();
        }

        [TestMethod]
        public async Task RemovingItemFromInventoryWillReturnFalseIfNotFound()
        {
            var item = await this.AddItemToInventoryAsync();

            (await this._inventory.RemoveItemAsync(item)).Should().BeTrue();
            (await this._inventory.RemoveItemAsync(item)).Should().BeFalse();
        }

        [TestMethod]
        public async Task RemovingItemFromInventoryThatWasNotAddedWillReturnFalse()
        {
            var item = new FakeItem(10);

            (await this._inventory.RemoveItemAsync(item)).Should().BeFalse();
        }

        [TestMethod]
        public async Task RemovingItemFromInventoryWhenItemIsLockedThrowsException()
        {
            await this._inventory.InsertItemAsync(this._item);

            this._item.MovingLocked = true;

            Func<Task> act = () => this._inventory.RemoveItemAsync(this._item);

            (await act.Should().ThrowAsync<ItemNotMovableException>())
                .Where(x =>
                    x.Message.Contains(this._item.Handle)
                    && x.Message.Contains(this._item.RuntimeId.ToString())
                    && x.Message.Contains("not movable"));

            this._inventory.Items.Should()
                .ContainSingle(x => x == this._item);
        }

        [TestMethod]
        public async Task AddingItemWithSameRuntimeKeyWillReturnFalse()
        {
            var item = new FakeItem(10);
            var otherFake = new FakeItem(10);

            otherFake.RuntimeId = item.RuntimeId;
            item.IsMergableCheck = x => false;

            (await this._inventory.InsertItemAsync(item)).Should().BeTrue();
            (await this._inventory.InsertItemAsync(otherFake)).Should().BeFalse();

            otherFake.CurrentInventory.Should().BeNull();
        }

        [TestMethod]
        public async Task AddingItemWithCurrentInventoryAlreadyToTargetInventorySetWillReturnFalse()
        {
            var item = new FakeItem(10);

            item.SetCurrentInventory(this._inventory);

            (await this._inventory.InsertItemAsync(item)).Should().BeFalse();
        }

        [TestMethod]
        public async Task AddingItemWithHigherWeightThanCapacityWillThrowException()
        {
            var item = new FakeItem(InventoryCapacity + 1);

            Func<Task> act = () => this._inventory.InsertItemAsync(item);

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

            Func<Task> act = () => this._inventory.InsertItemAsync(item);

            await act.Should().ThrowAsync<InventoryCapacityException>();

            this._inventory.Items.Should().BeEmpty();
        }

        [TestMethod]
        public async Task AddingInMovableItemThrowsException()
        {
            this._item.MovingLocked = true;

            Func<Task> act = () => this._inventory.InsertItemAsync(this._item);

            (await act.Should().ThrowAsync<ItemNotMovableException>())
                .Where(x =>
                    x.Message.Contains(this._item.Handle)
                    && x.Message.Contains(this._item.RuntimeId.ToString())
                    && x.Message.Contains("not movable"));

            this._inventory.Items.Should().BeEmpty();
        }

        [TestMethod]
        public async Task InsertingItemThatIsNotAllwedThrowsException()
        {
            this._inventory.SetItemFilter(x => false);

            Func<Task> act = () => this._inventory.InsertItemAsync(this._item);

            await act.Should().ThrowAsync<ItemNotAllowedException>();
        }

        [TestMethod]
        public async Task InsertingItemWithForceEvenWithDisallowingFilterInsertsIt()
        {
            this._inventory.SetItemFilter(x => false);

            await this._inventory.InsertItemAsync(this._item, true);

            this._inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == this._item);
        }

        [TestMethod]
        public async Task InsertingItemWithForceEvenWithNotEnoughCapacityShouldAllowInsertion()
        {
            this._item.SetSingleWeight(10);
            this._inventory.SetCapacity(5);

            await this._inventory.InsertItemAsync(this._item, true);

            this._inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == this._item);
        }

        [TestMethod]
        public async Task InsertingItemWithForceEvenInmovableItemShouldAllowInsertion()
        {
            this._item.MovingLocked = true;

            await this._inventory.InsertItemAsync(this._item, true);

            this._inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == this._item);
        }
    }
}
