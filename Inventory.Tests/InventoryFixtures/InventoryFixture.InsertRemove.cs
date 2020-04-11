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
            Func<Task> act = () => this.Inventory.RemoveItemAsync(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task InsertingNullFromInventoryWillThrowException()
        {
            Func<Task> act = () => this.Inventory.InsertItemAsync(null);

            (await act.Should().ThrowAsync<ArgumentNullException>())
                .Where(x => string.IsNullOrWhiteSpace(x.Message) == false);
        }

        [TestMethod]
        public async Task InsertedItemsWillAppearInItemsList()
        {
            var item = await this.AddItemToInventoryAsync();

            this.Inventory.Items.Should()
                .ContainSingle(x => x == item);
        }

        [TestMethod]
        public async Task InsertingItemMultipleTimesWillNotChangeDictionaryLength()
        {
            var item = await this.AddItemToInventoryAsync();

            await this.Inventory.InsertItemAsync(item);

            this.Inventory.Items.Should()
                .ContainSingle(x => x == item);
        }

        [TestMethod]
        public async Task InsertingItemToInventoryTwiceWillMarkFollowingCallsAsUnsuccessful()
        {
            var item = new FakeItem(10);

            (await this.Inventory.InsertItemAsync(item)).Should().BeTrue();
            (await this.Inventory.InsertItemAsync(item)).Should().BeFalse();
        }

        [TestMethod]
        public async Task RemovingItemFromInventoryWillReturnFalseIfNotFound()
        {
            var item = await this.AddItemToInventoryAsync();

            (await this.Inventory.RemoveItemAsync(item)).Should().BeTrue();
            (await this.Inventory.RemoveItemAsync(item)).Should().BeFalse();
        }

        [TestMethod]
        public async Task RemovingItemFromInventoryThatWasNotAddedWillReturnFalse()
        {
            var item = new FakeItem(10);

            (await this.Inventory.RemoveItemAsync(item)).Should().BeFalse();
        }

        [TestMethod]
        public async Task RemovingItemFromInventoryWhenItemIsLockedThrowsException()
        {
            await this.Inventory.InsertItemAsync(this.Item);

            this.Item.MovingLocked = true;

            Func<Task> act = () => this.Inventory.RemoveItemAsync(this.Item);

            (await act.Should().ThrowAsync<ItemNotMovableException>())
                .Where(x =>
                    x.Message.Contains(this.Item.Handle)
                    && x.Message.Contains(this.Item.RuntimeId.ToString())
                    && x.Message.Contains("not movable"));

            this.Inventory.Items.Should()
                .ContainSingle(x => x == this.Item);
        }

        [TestMethod]
        public async Task AddingItemWithSameRuntimeKeyWillReturnFalse()
        {
            var item = new FakeItem(10);
            var otherFake = new FakeItem(10);

            otherFake.RuntimeId = item.RuntimeId;
            item.IsMergableCheck = x => false;

            (await this.Inventory.InsertItemAsync(item)).Should().BeTrue();
            (await this.Inventory.InsertItemAsync(otherFake)).Should().BeFalse();

            otherFake.CurrentInventory.Should().BeNull();
        }

        [TestMethod]
        public async Task AddingItemWithCurrentInventoryAlreadyToTargetInventorySetWillReturnFalse()
        {
            var item = new FakeItem(10);

            item.SetCurrentInventory(this.Inventory);

            (await this.Inventory.InsertItemAsync(item)).Should().BeFalse();
        }

        [TestMethod]
        public async Task AddingItemWithHigherWeightThanCapacityWillThrowException()
        {
            var item = new FakeItem(InventoryCapacity + 1);

            Func<Task> act = () => this.Inventory.InsertItemAsync(item);

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

            Func<Task> act = () => this.Inventory.InsertItemAsync(item);

            await act.Should().ThrowAsync<InventoryCapacityException>();

            this.Inventory.Items.Should().BeEmpty();
        }

        [TestMethod]
        public async Task AddingInMovableItemThrowsException()
        {
            this.Item.MovingLocked = true;

            Func<Task> act = () => this.Inventory.InsertItemAsync(this.Item);

            (await act.Should().ThrowAsync<ItemNotMovableException>())
                .Where(x =>
                    x.Message.Contains(this.Item.Handle)
                    && x.Message.Contains(this.Item.RuntimeId.ToString())
                    && x.Message.Contains("not movable"));

            this.Inventory.Items.Should().BeEmpty();
        }

        [TestMethod]
        public async Task InsertingItemThatIsNotAllwedThrowsException()
        {
            this.Inventory.SetItemFilter(x => false);

            Func<Task> act = () => this.Inventory.InsertItemAsync(this.Item);

            await act.Should().ThrowAsync<ItemNotAllowedException>();
        }

        [TestMethod]
        public async Task InsertingItemWithForceEvenWithDisallowingFilterInsertsIt()
        {
            this.Inventory.SetItemFilter(x => false);

            await this.Inventory.InsertItemAsync(this.Item, true);

            this.Inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == this.Item);
        }

        [TestMethod]
        public async Task InsertingItemWithForceEvenWithNotEnoughCapacityShouldAllowInsertion()
        {
            this.Item.SetSingleWeight(10);
            this.Inventory.SetCapacity(5);

            await this.Inventory.InsertItemAsync(this.Item, true);

            this.Inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == this.Item);
        }

        [TestMethod]
        public async Task InsertingItemWithForceEvenInmovableItemShouldAllowInsertion()
        {
            this.Item.MovingLocked = true;

            await this.Inventory.InsertItemAsync(this.Item, true);

            this.Inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == this.Item);
        }
    }
}
