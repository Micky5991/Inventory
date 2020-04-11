using System;
using System.Threading.Tasks;
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

        private Mock<IItem> _itemMock;

        [TestInitialize]
        public void Setup()
        {
            this.SetupItemTest();

            this._itemMock = new Mock<IItem>();

            this.SetupDefaultServiceProvider();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.TearDownItemTest();
        }

        [TestMethod]
        public async Task InsertingItemToInventoryWillChangeCapacity()
        {
            await this.AddItemToInventoryAsync(10);

            this.AssertInventoryCapacity(10);
        }

        [TestMethod]
        public async Task RemovingItemFromInventoryWillChangeCapacity()
        {
            var item = await this.AddItemToInventoryAsync(10);

            this.AssertInventoryCapacity(10);

            await this._inventory.RemoveItemAsync(item);

            this.AssertInventoryCapacity(0);
        }

        [TestMethod]
        public async Task CheckIfItemFitsWillReturnCorrectValuesAfterItemCollectionChange()
        {
            var item = await this.AddItemToInventoryAsync(InventoryCapacity);

            this._inventory.DoesItemFit(item).Should().BeFalse();
        }

        [TestMethod]
        public void PassingNullToDoesItemFitWillThrowException()
        {
            Action act = () => this._inventory.DoesItemFit(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CheckingIfFillingItemWillFitIntoInventoryShouldReturnTrue()
        {
            var item = new FakeItem(InventoryCapacity);

            this._inventory.DoesItemFit(item).Should().BeTrue();
        }

        [TestMethod]
        public async Task CheckingIfSecondFillingItemWillFitIntoInventoryShouldReturnTrue()
        {
            await this.AddItemToInventoryAsync(InventoryCapacity - 10);

            var item = new FakeItem(10);

            this._inventory.DoesItemFit(item).Should().BeTrue();
        }

        [TestMethod]
        public async Task CheckingIfSecondCapacityExceedingItemWillFitIntoInventoryShouldReturnFalse()
        {
            await this.AddItemToInventoryAsync(InventoryCapacity - 10);

            var item = new FakeItem(11);

            this._inventory.DoesItemFit(item).Should().BeFalse();
        }

        [TestMethod]
        public void CallingNullForHandleOnDoesItemFitThrowsException()
        {
            Action act = () => this._inventory.DoesItemFit((ItemMeta) null, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void CallingDoesItemFitWithFittingItemReturnsTrue(int weightDelta)
        {
            this._inventory.SetCapacity(this._defaultRealMeta.DefaultWeight + weightDelta);

            this._inventory.DoesItemFit(this._defaultRealMeta).Should().BeTrue();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        [DataRow(-3)]
        public void CallingdoesItemFitWithExceedingWeightReturnsFalse(int weightDelta)
        {
            this._inventory.SetCapacity(this._defaultRealMeta.DefaultWeight + weightDelta);

            this._inventory.DoesItemFit(this._defaultRealMeta).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CallingDoesItemFitWithNegativeAmountThrowsException(int amount)
        {
            this._inventory.SetCapacity(1000);

            Action act = () => this._inventory.DoesItemFit(ItemHandle, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CallingDoesItemFitWithNegativeAmountAndMetaThrowsException(int amount)
        {
            this._inventory.SetCapacity(1000);

            Action act = () => this._inventory.DoesItemFit(this._defaultRealMeta, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        public void CallingDoesItemFitWithKnownHandleAndAvailableSpaceReturnsTrue()
        {
            this._inventory.SetCapacity(1000);

            this._inventory.DoesItemFit(ItemHandle, 1).Should().BeTrue();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CallingDoesItemFitWithInvalidHandleThrowsException(string handle)
        {
            Action act = () => this._inventory.DoesItemFit(handle, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CallingDoesItemFitWithUnknownHandleThrowsException()
        {
            Action act = () => this._inventory.DoesItemFit("unknownhandle", 1);

            act.Should().Throw<ItemMetaNotFoundException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CallingDoesItemFitWithMetaAndNegativeAmountThrowsException(int amount)
        {
            Action act = () => this._inventory.DoesItemFit(this._defaultRealMeta, amount);

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

            this._inventory.SetItemFilter(Filter);

            this._inventory.IsItemAllowed(this._item).Should().BeFalse();
            passedItem.Should().Be(this._item);
        }

        [TestMethod]
        public void ItemFilterWithNullAcceptsAllItems()
        {
            this._inventory.SetItemFilter(null);

            this._inventory.IsItemAllowed(this._item).Should().BeTrue();
        }

        [TestMethod]
        public void CallingIsItemAllowedWithNullThrowsException()
        {
            Action act = () => this._inventory.IsItemAllowed(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CallingCanBeInsertedWithNullThrowsException()
        {
            Action act = () => this._inventory.CanBeInserted(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CanBeInsertedWithRightWeightAndAllowedFilterAndMovableItemReturnsTrue()
        {
            this._inventory.SetCapacity(this._item.TotalWeight + 1);
            this._inventory.SetItemFilter(null);
            this._item.MovingLocked = false;

            this._inventory.CanBeInserted(this._item).Should().BeTrue();
        }

        [TestMethod]
        public void CanBeInsertedWithRightWeightAndAllowedFilterAndInMovableItemReturnsFalse()
        {
            this._inventory.SetCapacity(this._item.TotalWeight + 1);
            this._inventory.SetItemFilter(null);
            this._item.MovingLocked = true;

            this._inventory.CanBeInserted(this._item).Should().BeFalse();
        }

        [TestMethod]
        public void CanBeInsertedWithRightWeightAndDisallowedFilterAndMovableItemReturnsFalse()
        {
            this._inventory.SetCapacity(this._item.TotalWeight + 1);
            this._inventory.SetItemFilter(x => false);
            this._item.MovingLocked = false;

            this._inventory.CanBeInserted(this._item).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CanBeInsertedWithTooHighWeightAndAllowedFilterMovableItemReturnsFalse(int weightDelta)
        {
            this._inventory.SetCapacity(this._item.TotalWeight + weightDelta);
            this._inventory.SetItemFilter(x => true);
            this._item.MovingLocked = false;

            this._inventory.CanBeInserted(this._item).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CanBeInsertedWithTooHighWeightAndDisallowedFilterReturnsFalse(int weightDelta)
        {
            this._inventory.SetCapacity(this._item.TotalWeight + weightDelta);
            this._inventory.SetItemFilter(x => false);

            this._inventory.CanBeInserted(this._item).Should().BeFalse();
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
        public async Task IgnoringFalseFilterReturnsAllItems(bool ignoreCapacity, bool ignoreAllowance, bool ignoreMovable)
        {
            await this._inventory.InsertItemAsync(this._item);
            await this._inventory.InsertItemAsync(this._fakeItem);

            this._item.MovingLocked = ignoreMovable;
            this._fakeItem.MovingLocked = ignoreMovable;

            var otherInventory = new Mock<IInventory>();

            otherInventory.Setup(x => x.DoesItemFit(It.IsAny<IItem>())).Returns(ignoreCapacity == false);
            otherInventory.Setup(x => x.IsItemAllowed(It.IsAny<IItem>())).Returns(ignoreAllowance == false);

            var items = this._inventory.GetInsertableItems(otherInventory.Object,
                                                           ignoreCapacity == false,
                                                           ignoreAllowance == false,
                                                           ignoreMovable == false);

            otherInventory.Verify(x => x.DoesItemFit(this._item), Times.AtMostOnce);
            otherInventory.Verify(x => x.DoesItemFit(this._fakeItem), Times.AtMostOnce);

            otherInventory.Verify(x => x.IsItemAllowed(this._item), Times.AtMostOnce);
            otherInventory.Verify(x => x.IsItemAllowed(this._fakeItem), Times.AtMostOnce);

            items.Should()
                .HaveCount(2)
                .And.Contain(this._item)
                .And.Contain(this._fakeItem);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        public async Task AnyFalseRequirementReturnsEmptyList(bool fitStatus, bool allowanceStatus)
        {
            await this._inventory.InsertItemAsync(this._item);
            await this._inventory.InsertItemAsync(this._fakeItem);

            var otherInventory = new Mock<IInventory>();

            otherInventory.Setup(x => x.DoesItemFit(It.IsAny<IItem>())).Returns(fitStatus);
            otherInventory.Setup(x => x.IsItemAllowed(It.IsAny<IItem>())).Returns(allowanceStatus);

            var items = this._inventory.GetInsertableItems(otherInventory.Object, true, true);

            otherInventory.Verify(x => x.DoesItemFit(this._item), Times.AtMostOnce);
            otherInventory.Verify(x => x.DoesItemFit(this._fakeItem), Times.AtMostOnce);

            otherInventory.Verify(x => x.IsItemAllowed(this._item), Times.AtMostOnce);
            otherInventory.Verify(x => x.IsItemAllowed(this._fakeItem), Times.AtMostOnce);

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
            Action act = () => this._inventory.GetInsertableItems(null, capacityFilter, acceptanceFilter);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CallingGetItemFitAmountWithNullHandleThrowsException(string handle)
        {
            Action act = () => this._inventory.GetItemFitAmount(handle);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CallingGetItemFitAmountWithNullMetaThrowsException()
        {
            Action act = () => this._inventory.GetItemFitAmount((ItemMeta) null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CallingGetItemFitAmountWithNullItemThrowsException()
        {
            Action act = () => this._inventory.GetItemFitAmount((IItem) null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CallingGetItemFitAmountWithUnknownItemThrowsException()
        {
            Action act = () => this._inventory.GetItemFitAmount("unknownhandle");

            act.Should().Throw<ItemMetaNotFoundException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CallingGetItemFitAmountWithZeroOrLowerThrowsException(int value)
        {
            Action act = () => this._inventory.GetItemFitAmount(value);

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
            this._item.SetSingleWeight(singleWeight);
            this._inventory.SetCapacity(capacity);

            this._item.SingleWeight.Should().Be(singleWeight);
            this._inventory.Capacity.Should().Be(capacity);

            var amount = this._inventory.GetItemFitAmount(this._item);

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
            var meta = new ItemMeta(this._defaultRealMeta.Handle, this._defaultRealMeta.Type, this._defaultRealMeta.DisplayName, singleWeight);

            this._inventory.SetCapacity(capacity);
            this._inventory.Capacity.Should().Be(capacity);

            var amount = this._inventory.GetItemFitAmount(meta);

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
            this._defaultRealMeta = new ItemMeta(this._defaultRealMeta.Handle, this._defaultRealMeta.Type, this._defaultRealMeta.DisplayName, singleWeight);

            this.SetupDefaultServiceProvider();

            var success = this._itemRegistry.TryGetItemMeta(this._defaultRealMeta.Handle, out var meta);
            success.Should().BeTrue();

            meta.DefaultWeight.Should().Be(singleWeight);

            this._inventory.SetCapacity(capacity);
            this._inventory.Capacity.Should().Be(capacity);

            var amount = this._inventory.GetItemFitAmount(this._defaultRealMeta.Handle);

            amount.Should().Be(result);
        }

        private async Task<FakeItem> AddItemToInventoryAsync(int weight = 10)
        {
            var item = new FakeItem(weight);

            await this._inventory.InsertItemAsync(item);

            return item;
        }

        private void AssertInventoryCapacity(int usedCapacity, int capacity = InventoryCapacity, IInventory inventory = null)
        {
            if (inventory == null)
            {
                inventory = this._inventory;
            }

            inventory.Capacity.Should().Be(capacity);
            inventory.UsedCapacity.Should().Be(usedCapacity);
            inventory.AvailableCapacity.Should().Be(capacity - usedCapacity);
        }

    }
}
