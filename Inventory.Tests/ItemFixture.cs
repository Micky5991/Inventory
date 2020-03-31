using System;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Entities.Item;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ItemFixture
    {
        private const string ItemHandle = "testhandle";
        private const string ItemDisplayName = "FakeItem";
        private const int ItemWeight = 50;
        private const ItemFlags ItemFlags = Enums.ItemFlags.None;

        private ItemMeta _meta;
        private Item _item;

        private Mock<IInventory> _inventoryMock;

        [TestInitialize]
        public void Setup()
        {
            Item.MinimalItemAmount = 0;

            _meta = new ItemMeta(ItemHandle, typeof(RealItem), ItemDisplayName, ItemWeight, ItemFlags);
            _item = new RealItem(_meta);

            _inventoryMock = new Mock<IInventory>();
        }

        [TestMethod]
        public void CreatingItemWillBeSuccessful()
        {
            var item = _item;

            item.Should()
                .NotBeNull()
                .And.BeAssignableTo<IItem>();
        }

        [TestMethod]
        public void CreatingItemWillSetParametersCorrectly()
        {
            var item = new RealItem(_meta);

            item.Meta.Should().Be(_meta);
            item.Handle.Should().Be(_meta.Handle);
            item.SingleWeight.Should().Be(_meta.DefaultWeight);
            item.DisplayName.Should().Be(_meta.DisplayName);
            item.DefaultDisplayName.Should().Be(_meta.DisplayName);
            item.Amount.Should().Be(Math.Max(Item.MinimalItemAmount, 1));
            item.Stackable.Should().BeTrue();

            item.RuntimeId.Should().NotBe(Guid.Empty);
        }

        [TestMethod]
        [DataRow(ItemFlags.NotStackable, false)]
        [DataRow(ItemFlags.None, true)]
        public void SettingNonStackableFlagWillBeInterpretedCorrectly(ItemFlags flags, bool stackable)
        {
            _meta = new ItemMeta(_meta.Handle, _meta.Type, _meta.DisplayName, _meta.DefaultWeight, flags);
            _item = new RealItem(_meta);

            _item.Stackable.Should().Be(stackable);
        }

        [TestMethod]
        public void CreatingItemWithNullItemMetaWillThrowException()
        {
            Action act = () => new RealItem(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ChangingDisplayNameUpdatesValueCorrecly()
        {
            _item.SetDisplayName("Cool");

            _item.DisplayName.Should().Be("Cool");
        }

        [TestMethod]
        public void ChangingDisplayNameKeepsValueInDefaultDisplayNameSame()
        {
            _item.SetDisplayName("Other");

            var oldName = _item.DefaultDisplayName;

            _item.DefaultDisplayName.Should().Be(oldName);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void SettingDisplayNameToNullThrowsException(string displayName)
        {
            var oldName = _item.DisplayName;
            Action act = () => _item.SetDisplayName(displayName);

            act.Should().Throw<ArgumentNullException>();
            _item.DisplayName.Should().Be(oldName);
        }

        [TestMethod]
        public void InequalHandleShouldPreventMerging()
        {
            var fakeItem = new FakeItem(_item.SingleWeight, "unmergableitem");

            _item.CanMergeWith(fakeItem).Should().BeFalse();
        }

        [TestMethod]
        public void CanMergeCheckWithNullSourceItemWillResultInException()
        {
            Action act = () => _item.CanMergeWith(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void NonStackableItemsWillNotBeMergable()
        {
            var fakeItem = new FakeItem(_item.SingleWeight, ItemHandle, flags: ItemFlags.NotStackable);

            _item.CanMergeWith(fakeItem).Should().BeFalse();
        }

        [TestMethod]
        public void NonStackableTargetWillNotBeMergable()
        {
            var fakeItem = new FakeItem(_item.SingleWeight, ItemHandle, flags: ItemFlags.NotStackable);

            var newMeta = new ItemMeta(_meta.Handle, _meta.Type, _meta.DisplayName, _meta.DefaultWeight, ItemFlags.NotStackable);

            _item = new RealItem(newMeta);

            _item.CanMergeWith(fakeItem).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void ItemWithZeroAmountWillNotBeMergable(int itemAmount)
        {
            var fakeItem = new FakeItem(_item.SingleWeight, _item.Handle);

            fakeItem.SetAmount(itemAmount);

            _item.CanMergeWith(fakeItem).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void ItemWithPositiveItemAmountWillBeMergable(int itemAmount)
        {
            var fakeItem = new FakeItem(_item.SingleWeight, _item.Handle);

            fakeItem.SetAmount(itemAmount);

            _item.CanMergeWith(fakeItem).Should().BeTrue();
        }

        [TestMethod]
        public void SameItemWillNotBeMergable()
        {
            _item.CanMergeWith(_item).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void SettingAmountWillUpdateWeightAndAmount(int amountDelta)
        {
            var singleWeight = _meta.DefaultWeight;
            var targetAmount = Item.MinimalItemAmount + amountDelta;

            _item.SetAmount(targetAmount);

            _item.Amount.Should().Be(targetAmount);
            _item.TotalWeight.Should().Be(singleWeight * targetAmount);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(3)]
        [DataRow(7)]
        [DataRow(11)]
        public void TotalWeightIsMultipleOfSingleWeightAndAmount(int amountDelta)
        {
            var actualAmount = Item.MinimalItemAmount + amountDelta;

            _item.SetAmount(actualAmount);

            (_item.TotalWeight / _item.SingleWeight).Should().Be(actualAmount);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        public void SettingItemAmountBelowMinimalAllowedItemAmountThrowsException(int amountDelta)
        {
            var oldAmount = _item.Amount;
            Action act = () => _item.SetAmount(Item.MinimalItemAmount + amountDelta);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains($"{Math.Max(Item.MinimalItemAmount, 0)} or higher"));

            _item.Amount.Should().Be(oldAmount);
        }

        [TestMethod]
        public void SettingCurrentInventoryWillUpdateValue()
        {
            _item.SetCurrentInventory(_inventoryMock.Object);
            _item.CurrentInventory.Should().Be(_inventoryMock.Object);

            _item.SetCurrentInventory(null);
            _item.CurrentInventory.Should().BeNull();
        }

        [TestMethod]
        public async Task MergingItemWillSumAmount()
        {
            var otherItem = new FakeItem(_meta);
            otherItem.SetAmount(2);

            _item.SetAmount(2);

            await _item.MergeItemAsync(otherItem);

            _item.Amount.Should().Be(4);
            otherItem.Amount.Should().Be(0);
        }

        [TestMethod]
        public async Task MergingItemWithItselfWillThrowException()
        {
            Func<Task> act = () => _item.MergeItemAsync(_item);

            (await act.Should().ThrowAsync<ArgumentException>())
                .Where(x => x.Message.Contains("itself"));
        }

        [TestMethod]
        public async Task MergingItemWithNullWillThrowException()
        {
            Func<Task> act = () => _item.MergeItemAsync(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task MergingItemWithUnmergableItemWillThrowException()
        {
            var otherItem = new Mock<IItem>();

            otherItem.SetupGet(x => x.Stackable).Returns(false);

            Func<Task> act = () => _item.MergeItemAsync(_item);

            await act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        public void DetectionOfMinimalAmountDetectsRightAmount()
        {
            Item.MinimalItemAmount = -10;

            Action act = () => _item.SetAmount(-5);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("0"));
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(1)]
        public void SingleWeightHasToBeEqualToAllowMerging(int weightDelta)
        {
            var otherItem = new FakeItem(_item.Meta);

            otherItem.SingleWeight = _item.Meta.DefaultWeight + weightDelta;

            _item.CanMergeWith(otherItem).Should().Be(otherItem.SingleWeight == _item.Meta.DefaultWeight);
        }

        [TestMethod]
        public void ChangingDisplayNameTriggersNotification()
        {
            using var monitoredSubject = _item.Monitor();

            _item.SetDisplayName(_item.DisplayName + "ADD");

            monitoredSubject.Should().RaisePropertyChangeFor(x => x.DisplayName);
        }

        [TestMethod]
        public void ChangingDisplayNameToCurrentValueDoesNotTriggerNotification()
        {
            using var monitoredSubject = _item.Monitor();

            _item.SetDisplayName(_item.DisplayName);

            monitoredSubject.Should().NotRaisePropertyChangeFor(x => x.DisplayName);
        }

        [TestMethod]
        public void ChangingAmountWillNotifyAmountAndTotalWeight()
        {
            using var monitoredItem = _item.Monitor();

            _item.SetAmount(_item.Amount + 1);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.Amount);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void ChangingAmountToSameAmountWontTriggerNotification()
        {
            using var monitoredItem = _item.Monitor();

            _item.SetAmount(_item.Amount);

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.Amount);
            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void ChangingCurrentInventoryToDifferentValueWillNotify()
        {
            using var monitoredItem = _item.Monitor();

            var otherInventory = new Mock<IInventory>();

            _item.SetCurrentInventory(otherInventory.Object);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.CurrentInventory);
        }

        [TestMethod]
        public void ChangingCurrentInventoryToSameValueWontNotify()
        {
            using var monitoredItem = _item.Monitor();

            _item.SetCurrentInventory(null);

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.CurrentInventory);
        }

        [TestMethod]
        public void PassingMetaWithWrongTypeThrowsException()
        {
            var meta = new ItemMeta(ItemHandle, typeof(FakeItem), ItemDisplayName, ItemWeight, ItemFlags);

            Action act = () => new RealItem(meta);

            act.Should().Throw<ArgumentException>()
                .Where(x =>
                    x.Message.Contains("mismatch")
                    && x.Message.Contains("type")
                    && x.Message.Contains("meta"));
        }

        [TestMethod]
        public async Task MergingTwoItemsCanMergeSignalsFalseWillThrowException()
        {
            var realMeta = new ItemMeta("item1", typeof(RealItem), ItemDisplayName, ItemWeight, ItemFlags);
            var fakeMeta = new ItemMeta("item2", typeof(FakeItem), ItemDisplayName, ItemWeight, ItemFlags);

            var realItem = new RealItem(realMeta);
            var fakeItem = new FakeItem(fakeMeta);

            realItem.CanMergeWith(fakeItem).Should().BeFalse();

            Func<Task> act = () => realItem.MergeItemAsync(fakeItem);

            (await act.Should().ThrowAsync<ArgumentException>())
                .Where(x => x.Message.Contains("merge") && x.Message.Contains("not"));
        }

    }
}
