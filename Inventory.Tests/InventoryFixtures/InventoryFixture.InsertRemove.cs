using System;
using FluentAssertions;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests.InventoryFixtures
{
    public partial class InventoryFixture
    {
        [TestMethod]
        public void RemovingNullFromInventoryWillThrowException()
        {
            Action act = () => this.Inventory.RemoveItem(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void InsertingNullFromInventoryWillThrowException()
        {
            Action act = () => this.Inventory.InsertItem(null);

            act.Should().Throw<ArgumentNullException>()
               .Where(x => string.IsNullOrWhiteSpace(x.Message) == false);
        }

        [TestMethod]
        public void InsertedItemsWillAppearInItemsList()
        {
            var item = this.AddItemToInventory();

            this.Inventory.Items.Should()
                .ContainSingle(x => x == item);
        }

        [TestMethod]
        public void InsertingItemMultipleTimesWillNotChangeDictionaryLength()
        {
            var item = this.AddItemToInventory();

            this.Inventory.InsertItem(item);

            this.Inventory.Items.Should()
                .ContainSingle(x => x == item);
        }

        [TestMethod]
        public void InsertingItemToInventoryTwiceWillMarkFollowingCallsAsUnsuccessful()
        {
            var item = new FakeItem(10);

            (this.Inventory.InsertItem(item)).Should().BeTrue();
            (this.Inventory.InsertItem(item)).Should().BeFalse();
        }

        [TestMethod]
        public void RemovingItemFromInventoryWillReturnFalseIfNotFound()
        {
            var item = this.AddItemToInventory();

            (this.Inventory.RemoveItem(item)).Should().BeTrue();
            (this.Inventory.RemoveItem(item)).Should().BeFalse();
        }

        [TestMethod]
        public void RemovingItemFromInventoryThatWasNotAddedWillReturnFalse()
        {
            var item = new FakeItem(10);

            (this.Inventory.RemoveItem(item)).Should().BeFalse();
        }

        [TestMethod]
        public void RemovingItemFromInventoryWhenItemIsLockedThrowsException()
        {
            this.Inventory.InsertItem(this.Item);

            this.Item.MovingLocked = true;

            Action act = () => this.Inventory.RemoveItem(this.Item);

            (act.Should().Throw<ItemNotMovableException>())
                .Where(x =>
                    x.Message.Contains(this.Item.Handle)
                    && x.Message.Contains(this.Item.RuntimeId.ToString())
                    && x.Message.Contains("not movable"));

            this.Inventory.Items.Should()
                .ContainSingle(x => x == this.Item);
        }

        [TestMethod]
        public void AddingItemWithSameRuntimeKeyWillReturnFalse()
        {
            var item = new FakeItem(10);
            var otherFake = new FakeItem(10);

            otherFake.RuntimeId = item.RuntimeId;
            item.IsMergableCheck = x => false;

            (this.Inventory.InsertItem(item)).Should().BeTrue();
            (this.Inventory.InsertItem(otherFake)).Should().BeFalse();

            otherFake.CurrentInventory.Should().BeNull();
        }

        [TestMethod]
        public void AddingItemWithCurrentInventoryAlreadyToTargetInventorySetWillReturnFalse()
        {
            var item = new FakeItem(10);

            item.SetCurrentInventory(this.Inventory);

            (this.Inventory.InsertItem(item)).Should().BeFalse();
        }

        [TestMethod]
        public void AddingItemWithHigherWeightThanCapacityWillThrowException()
        {
            var item = new FakeItem(InventoryCapacity + 1);

            Action act = () => this.Inventory.InsertItem(item);

            (act.Should().Throw<InventoryCapacityException>())
                .Where(x =>
                            x.Message.Contains(item.Handle) &&
                            x.Message.Contains(item.RuntimeId.ToString())
                            );
        }

        [TestMethod]
        public void AddingInvalidItemWillNotAddItemToInventory()
        {
            var item = new FakeItem(InventoryCapacity + 1);

            Action act = () => this.Inventory.InsertItem(item);

            act.Should().Throw<InventoryCapacityException>();

            this.Inventory.Items.Should().BeEmpty();
        }

        [TestMethod]
        public void AddingInMovableItemThrowsException()
        {
            this.Item.MovingLocked = true;

            Action act = () => this.Inventory.InsertItem(this.Item);

            (act.Should().Throw<ItemNotMovableException>())
                .Where(x =>
                    x.Message.Contains(this.Item.Handle)
                    && x.Message.Contains(this.Item.RuntimeId.ToString())
                    && x.Message.Contains("not movable"));

            this.Inventory.Items.Should().BeEmpty();
        }

        [TestMethod]
        public void InsertingItemThatIsNotAllwedThrowsException()
        {
            this.Inventory.SetItemFilter(x => false);

            Action act = () => this.Inventory.InsertItem(this.Item);

            act.Should().Throw<ItemNotAllowedException>();
        }

        [TestMethod]
        public void InsertingItemWithForceEvenWithDisallowingFilterInsertsIt()
        {
            this.Inventory.SetItemFilter(x => false);

            this.Inventory.InsertItem(this.Item, true);

            this.Inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == this.Item);
        }

        [TestMethod]
        public void InsertingItemWithForceEvenWithNotEnoughCapacityShouldAllowInsertion()
        {
            this.Item.SetSingleWeight(10);
            this.Inventory.SetCapacity(5);

            this.Inventory.InsertItem(this.Item, true);

            this.Inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == this.Item);
        }

        [TestMethod]
        public void InsertingItemWithForceEvenInmovableItemShouldAllowInsertion()
        {
            this.Item.MovingLocked = true;

            this.Inventory.InsertItem(this.Item, true);

            this.Inventory.Items.Should()
                .HaveCount(1)
                .And.ContainSingle(x => x == this.Item);
        }
    }
}
