using System;
using FluentAssertions;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests.InventoryFixtures
{
    [TestClass]
    public partial class InventoryFixture : ItemTest
    {
        private const int InventoryCapacity = 100;

        private Mock<IItem> itemMock;

        [TestInitialize]
        public void Setup()
        {
            this.SetupItemTest();

            this.itemMock = new Mock<IItem>();

            this.SetupDefaultServiceProvider();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.TearDownItemTest();
        }

        [TestMethod]
        public void InsertingItemToInventoryWillChangeCapacity()
        {
            this.AddItemToInventory(10);

            this.AssertInventoryCapacity(10);
        }

        [TestMethod]
        public void RemovingItemFromInventoryWillChangeCapacity()
        {
            var item = this.AddItemToInventory(10);

            this.AssertInventoryCapacity(10);

            this.Inventory.RemoveItem(item);

            this.AssertInventoryCapacity(0);
        }

        [TestMethod]
        public void CheckIfItemFitsWillReturnCorrectValuesAfterItemCollectionChange()
        {
            var item = this.AddItemToInventory(InventoryCapacity);

            this.Inventory.DoesItemFit(item).Should().BeFalse();
        }

        [TestMethod]
        public void PassingNullToDoesItemFitWillThrowException()
        {
            Action act = () => this.Inventory.DoesItemFit(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CheckingIfFillingItemWillFitIntoInventoryShouldReturnTrue()
        {
            var item = new FakeItem(InventoryCapacity);

            this.Inventory.DoesItemFit(item).Should().BeTrue();
        }

        [TestMethod]
        public void CheckingIfSecondFillingItemWillFitIntoInventoryShouldReturnTrue()
        {
            this.AddItemToInventory(InventoryCapacity - 10);

            var item = new FakeItem(10);

            this.Inventory.DoesItemFit(item).Should().BeTrue();
        }

        [TestMethod]
        public void CheckingIfSecondCapacityExceedingItemWillFitIntoInventoryShouldReturnFalse()
        {
            this.AddItemToInventory(InventoryCapacity - 10);

            var item = new FakeItem(11);

            this.Inventory.DoesItemFit(item).Should().BeFalse();
        }

        [TestMethod]
        public void CallingNullForHandleOnDoesItemFitThrowsException()
        {
            Action act = () => this.Inventory.DoesItemFit((ItemMeta) null, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void CallingDoesItemFitWithFittingItemReturnsTrue(int weightDelta)
        {
            this.Inventory.SetCapacity(this.DefaultRealMeta.DefaultWeight + weightDelta);

            this.Inventory.DoesItemFit(this.DefaultRealMeta).Should().BeTrue();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        [DataRow(-3)]
        public void CallingdoesItemFitWithExceedingWeightReturnsFalse(int weightDelta)
        {
            this.Inventory.SetCapacity(this.DefaultRealMeta.DefaultWeight + weightDelta);

            this.Inventory.DoesItemFit(this.DefaultRealMeta).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CallingDoesItemFitWithNegativeAmountThrowsException(int amount)
        {
            this.Inventory.SetCapacity(1000);

            Action act = () => this.Inventory.DoesItemFit(ItemHandle, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CallingDoesItemFitWithNegativeAmountAndMetaThrowsException(int amount)
        {
            this.Inventory.SetCapacity(1000);

            Action act = () => this.Inventory.DoesItemFit(this.DefaultRealMeta, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        public void CallingDoesItemFitWithKnownHandleAndAvailableSpaceReturnsTrue()
        {
            this.Inventory.SetCapacity(1000);

            this.Inventory.DoesItemFit(ItemHandle, 1).Should().BeTrue();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CallingDoesItemFitWithInvalidHandleThrowsException(string handle)
        {
            Action act = () => this.Inventory.DoesItemFit(handle, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CallingDoesItemFitWithUnknownHandleThrowsException()
        {
            Action act = () => this.Inventory.DoesItemFit("unknownhandle", 1);

            act.Should().Throw<ItemMetaNotFoundException>()
               .Where(x => x.Message.Contains("unknownhandle") && x.Message.Contains("handle"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CallingDoesItemFitWithMetaAndNegativeAmountThrowsException(int amount)
        {
            Action act = () => this.Inventory.DoesItemFit(this.DefaultRealMeta, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher") && x.Message.Contains("amount"));
        }

        [TestMethod]
        public void ItemFilterWillCallMethod()
        {
            IItem passedItem = null;

            bool Filter(IItem item)
            {
                passedItem = item;

                return false;
            }

            this.Inventory.SetItemFilter(Filter);

            this.Inventory.IsItemAllowed(this.Item).Should().BeFalse();
            passedItem.Should().Be(this.Item);
        }

        [TestMethod]
        public void ItemFilterWithNullAcceptsAllItems()
        {
            this.Inventory.SetItemFilter(null);

            this.Inventory.IsItemAllowed(this.Item).Should().BeTrue();
        }

        [TestMethod]
        public void CallingIsItemAllowedWithNullThrowsException()
        {
            Action act = () => this.Inventory.IsItemAllowed(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CallingCanBeInsertedWithNullThrowsException()
        {
            Action act = () => this.Inventory.CanBeInserted(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CanBeInsertedWithRightWeightAndAllowedFilterAndMovableItemReturnsTrue()
        {
            this.Inventory.SetCapacity(this.Item.TotalWeight + 1);
            this.Inventory.SetItemFilter(null);
            this.Item.MovingLocked = false;

            this.Inventory.CanBeInserted(this.Item).Should().BeTrue();
        }

        [TestMethod]
        public void CanBeInsertedWithRightWeightAndAllowedFilterAndInMovableItemReturnsFalse()
        {
            this.Inventory.SetCapacity(this.Item.TotalWeight + 1);
            this.Inventory.SetItemFilter(null);
            this.Item.MovingLocked = true;

            this.Inventory.CanBeInserted(this.Item).Should().BeFalse();
        }

        [TestMethod]
        public void CanBeInsertedWithRightWeightAndDisallowedFilterAndMovableItemReturnsFalse()
        {
            this.Inventory.SetCapacity(this.Item.TotalWeight + 1);
            this.Inventory.SetItemFilter(x => false);
            this.Item.MovingLocked = false;

            this.Inventory.CanBeInserted(this.Item).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CanBeInsertedWithTooHighWeightAndAllowedFilterMovableItemReturnsFalse(int weightDelta)
        {
            this.Inventory.SetCapacity(this.Item.TotalWeight + weightDelta);
            this.Inventory.SetItemFilter(x => true);
            this.Item.MovingLocked = false;

            this.Inventory.CanBeInserted(this.Item).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CanBeInsertedWithTooHighWeightAndDisallowedFilterReturnsFalse(int weightDelta)
        {
            this.Inventory.SetCapacity(this.Item.TotalWeight + weightDelta);
            this.Inventory.SetItemFilter(x => false);

            this.Inventory.CanBeInserted(this.Item).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(true, false, false)]
        [DataRow(false, true, false)]
        [DataRow(true, true, false)]
        [DataRow(false, false, false)]
        [DataRow(true, false, true)]
        [DataRow(false, true, true)]
        [DataRow(true, true, true)]
        [DataRow(false, false, true)]
        public void IgnoringFalseFilterReturnsAllItems(bool ignoreCapacity, bool ignoreAllowance, bool ignoreMovable)
        {
            this.Inventory.InsertItem(this.Item);
            this.Inventory.InsertItem(this.FakeItem);

            this.Item.MovingLocked = ignoreMovable;
            this.FakeItem.MovingLocked = ignoreMovable;

            var otherInventory = new Mock<IInventory>();

            otherInventory.Setup(x => x.DoesItemFit(It.IsAny<IItem>())).Returns(ignoreCapacity == false);
            otherInventory.Setup(x => x.IsItemAllowed(It.IsAny<IItem>())).Returns(ignoreAllowance == false);

            var items = this.Inventory.GetInsertableItems(otherInventory.Object,
                                                           ignoreCapacity == false,
                                                           ignoreAllowance == false,
                                                           ignoreMovable == false);

            otherInventory.Verify(x => x.DoesItemFit(this.Item), Times.AtMostOnce);
            otherInventory.Verify(x => x.DoesItemFit(this.FakeItem), Times.AtMostOnce);

            otherInventory.Verify(x => x.IsItemAllowed(this.Item), Times.AtMostOnce);
            otherInventory.Verify(x => x.IsItemAllowed(this.FakeItem), Times.AtMostOnce);

            items.Should()
                .HaveCount(2)
                .And.Contain(this.Item)
                .And.Contain(this.FakeItem);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        public void AnyFalseRequirementReturnsEmptyList(bool fitStatus, bool allowanceStatus)
        {
            this.Inventory.InsertItem(this.Item);
            this.Inventory.InsertItem(this.FakeItem);

            var otherInventory = new Mock<IInventory>();

            otherInventory.Setup(x => x.DoesItemFit(It.IsAny<IItem>())).Returns(fitStatus);
            otherInventory.Setup(x => x.IsItemAllowed(It.IsAny<IItem>())).Returns(allowanceStatus);

            var items = this.Inventory.GetInsertableItems(otherInventory.Object, true, true);

            otherInventory.Verify(x => x.DoesItemFit(this.Item), Times.AtMostOnce);
            otherInventory.Verify(x => x.DoesItemFit(this.FakeItem), Times.AtMostOnce);

            otherInventory.Verify(x => x.IsItemAllowed(this.Item), Times.AtMostOnce);
            otherInventory.Verify(x => x.IsItemAllowed(this.FakeItem), Times.AtMostOnce);

            items.Should().BeEmpty();
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        public void CallingGetInsertableItemsWithNullTargetInventoryThrowsException(bool capacityFilter,
            bool acceptanceFilter)
        {
            Action act = () => this.Inventory.GetInsertableItems(null, capacityFilter, acceptanceFilter);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CallingGetItemFitAmountWithNullHandleThrowsException(string handle)
        {
            Action act = () => this.Inventory.GetItemFitAmount(handle);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CallingGetItemFitAmountWithNullMetaThrowsException()
        {
            Action act = () => this.Inventory.GetItemFitAmount((ItemMeta) null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CallingGetItemFitAmountWithNullItemThrowsException()
        {
            Action act = () => this.Inventory.GetItemFitAmount((IItem) null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CallingGetItemFitAmountWithUnknownItemThrowsException()
        {
            Action act = () => this.Inventory.GetItemFitAmount("unknownhandle");

            act.Should().Throw<ItemMetaNotFoundException>()
               .Where(x => x.Message.Contains("unknownhandle") && x.Message.Contains("handle"));;
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CallingGetItemFitAmountWithZeroOrLowerThrowsException(int value)
        {
            Action act = () => this.Inventory.GetItemFitAmount(value);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(1, 100, 100)]
        [DataRow(2, 100, 50)]
        [DataRow(3, 100, 33)]
        [DataRow(4, 100, 25)]
        [DataRow(11, 100, 9)]
        [DataRow(51, 100, 1)]
        [DataRow(2, 3, 1)]
        public void GetItemFitAmountSetsValueCorrectly(int singleWeight, int capacity, int result)
        {
            this.Item.SetSingleWeight(singleWeight);
            this.Inventory.SetCapacity(capacity);

            this.Item.SingleWeight.Should().Be(singleWeight);
            this.Inventory.Capacity.Should().Be(capacity);

            var amount = this.Inventory.GetItemFitAmount(this.Item);

            amount.Should().Be(result);
        }

        [TestMethod]
        [DataRow(1, 100, 100)]
        [DataRow(2, 100, 50)]
        [DataRow(3, 100, 33)]
        [DataRow(4, 100, 25)]
        [DataRow(11, 100, 9)]
        [DataRow(51, 100, 1)]
        [DataRow(2, 3, 1)]
        public void GetItemFitAmountWithMetaReturnsCorrectValue(int singleWeight, int capacity, int result)
        {
            var meta = new ItemMeta(this.DefaultRealMeta.Handle, this.DefaultRealMeta.Type, this.DefaultRealMeta.DisplayName, singleWeight);

            this.Inventory.SetCapacity(capacity);
            this.Inventory.Capacity.Should().Be(capacity);

            var amount = this.Inventory.GetItemFitAmount(meta);

            amount.Should().Be(result);
        }

        [TestMethod]
        [DataRow(1, 100, 100)]
        [DataRow(2, 100, 50)]
        [DataRow(3, 100, 33)]
        [DataRow(4, 100, 25)]
        [DataRow(11, 100, 9)]
        [DataRow(51, 100, 1)]
        [DataRow(2, 3, 1)]
        public void GetItemFitAmountWithHandleReturnsCorrectValue(int singleWeight, int capacity, int result)
        {
            this.DefaultRealMeta = new ItemMeta(this.DefaultRealMeta.Handle, this.DefaultRealMeta.Type, this.DefaultRealMeta.DisplayName, singleWeight);

            this.SetupDefaultServiceProvider();

            var success = this.ItemRegistry.TryGetItemMeta(this.DefaultRealMeta.Handle, out var meta);
            success.Should().BeTrue();

            meta.DefaultWeight.Should().Be(singleWeight);

            this.Inventory.SetCapacity(capacity);
            this.Inventory.Capacity.Should().Be(capacity);

            var amount = this.Inventory.GetItemFitAmount(this.DefaultRealMeta.Handle);

            amount.Should().Be(result);
        }

        private FakeItem AddItemToInventory(int weight = 10)
        {
            var item = new FakeItem(weight);

            this.Inventory.InsertItem(item);

            return item;
        }

        private void AssertInventoryCapacity(int usedCapacity, int capacity = InventoryCapacity, IInventory inventory = null)
        {
            if (inventory == null)
            {
                inventory = this.Inventory;
            }

            inventory.Capacity.Should().Be(capacity);
            inventory.UsedCapacity.Should().Be(usedCapacity);
            inventory.AvailableCapacity.Should().Be(capacity - usedCapacity);
        }

    }
}
