using System;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Entities.Item;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;
using Micky5991.Inventory.Tests.Fakes;
using Micky5991.Inventory.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ItemFixture : ItemTest
    {

        private Mock<IInventory> _inventoryMock;

        [TestInitialize]
        public void Setup()
        {
            Item.MinimalItemAmount = 0;

            this.SetupItemTest();

            this._inventoryMock = new Mock<IInventory>();

        }

        [TestCleanup]
        public void TearDown()
        {
            this.TearDownItemTest();
        }

        [TestMethod]
        public void CreatingItemWillBeSuccessful()
        {
            var item = this._item;

            item.Should()
                .NotBeNull()
                .And.BeAssignableTo<IItem>();
        }

        [TestMethod]
        public void CreatingItemWillSetParametersCorrectly()
        {
            var item = new RealItem(this._realMeta, ServiceUtils.CreateItemServices());

            item.Meta.Should().Be(this._realMeta);
            item.Handle.Should().Be(this._realMeta.Handle);
            item.SingleWeight.Should().Be(this._realMeta.DefaultWeight);
            item.DisplayName.Should().Be(this._realMeta.DisplayName);
            item.DefaultDisplayName.Should().Be(this._realMeta.DisplayName);
            item.Amount.Should().Be(Math.Max(Item.MinimalItemAmount, 1));
            item.Stackable.Should().BeTrue();

            item.RuntimeId.Should().NotBe(Guid.Empty);
        }

        [TestMethod]
        [DataRow(ItemFlags.NotStackable, false)]
        [DataRow(ItemFlags.None, true)]
        public void SettingNonStackableFlagWillBeInterpretedCorrectly(ItemFlags flags, bool stackable)
        {
            this.SetupServiceProvider(new ItemMeta(this._defaultRealMeta.Handle, typeof(RealItem), this._defaultRealMeta.DisplayName, this._defaultRealMeta.DefaultWeight, flags));

            this._item.Stackable.Should().Be(stackable);
        }

        [TestMethod]
        public void SettingAmountOfNonStackableItemWillThrowException()
        {
            this.SetupServiceProvider(new ItemMeta(this._defaultRealMeta.Handle, typeof(RealItem), this._defaultRealMeta.DisplayName, this._defaultRealMeta.DefaultWeight, ItemFlags.NotStackable));

            Action act = () => this._item.SetAmount(2);
            act.Should().Throw<ItemNotStackableException>();
        }

        [TestMethod]
        public void CreatingItemWithNullItemMetaWillThrowException()
        {
            Action act = () => new RealItem(null, this._itemServices);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ChangingDisplayNameUpdatesValueCorrecly()
        {
            this._item.SetDisplayName("Cool");

            this._item.DisplayName.Should().Be("Cool");
        }

        [TestMethod]
        public void ChangingDisplayNameKeepsValueInDefaultDisplayNameSame()
        {
            this._item.SetDisplayName("Other");

            var oldName = this._item.DefaultDisplayName;

            this._item.DefaultDisplayName.Should().Be(oldName);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void SettingDisplayNameToNullThrowsException(string displayName)
        {
            var oldName = this._item.DisplayName;
            Action act = () => this._item.SetDisplayName(displayName);

            act.Should().Throw<ArgumentNullException>();
            this._item.DisplayName.Should().Be(oldName);
        }

        [TestMethod]
        public void CanMergeCheckWithNullSourceItemWillResultInException()
        {
            Action act = () => this._item.CanMergeWith(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void SettingAmountWillUpdateWeightAndAmount(int amountDelta)
        {
            var singleWeight = this._realMeta.DefaultWeight;
            var targetAmount = Item.MinimalItemAmount + amountDelta;

            this._item.SetAmount(targetAmount);

            this._item.Amount.Should().Be(targetAmount);
            this._item.TotalWeight.Should().Be(singleWeight * targetAmount);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(3)]
        [DataRow(7)]
        [DataRow(11)]
        public void TotalWeightIsMultipleOfSingleWeightAndAmount(int amountDelta)
        {
            var actualAmount = Item.MinimalItemAmount + amountDelta;

            this._item.SetAmount(actualAmount);

            (this._item.TotalWeight / this._item.SingleWeight).Should().Be(actualAmount);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        public void SettingItemAmountBelowMinimalAllowedItemAmountThrowsException(int amountDelta)
        {
            var oldAmount = this._item.Amount;
            Action act = () => this._item.SetAmount(Item.MinimalItemAmount + amountDelta);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains($"{Math.Max(Item.MinimalItemAmount, 0)} or higher"));

            this._item.Amount.Should().Be(oldAmount);
        }

        [TestMethod]
        public void SettingCurrentInventoryWillUpdateValue()
        {
            this._item.SetCurrentInventory(this._inventoryMock.Object);
            this._item.CurrentInventory.Should().Be(this._inventoryMock.Object);

            this._item.SetCurrentInventory(null);
            this._item.CurrentInventory.Should().BeNull();
        }

        [TestMethod]
        public void DetectionOfMinimalAmountDetectsRightAmount()
        {
            Item.MinimalItemAmount = -10;

            Action act = () => this._item.SetAmount(-5);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("0"));
        }

        [TestMethod]
        public void ChangingDisplayNameTriggersNotification()
        {
            using var monitoredSubject = this._item.Monitor();

            this._item.SetDisplayName(this._item.DisplayName + "ADD");

            monitoredSubject.Should().RaisePropertyChangeFor(x => x.DisplayName);
        }

        [TestMethod]
        public void ChangingDisplayNameToCurrentValueDoesNotTriggerNotification()
        {
            using var monitoredSubject = this._item.Monitor();

            this._item.SetDisplayName(this._item.DisplayName);

            monitoredSubject.Should().NotRaisePropertyChangeFor(x => x.DisplayName);
        }

        [TestMethod]
        public void ChangingAmountWillNotifyAmountAndTotalWeight()
        {
            using var monitoredItem = this._item.Monitor();

            this._item.SetAmount(this._item.Amount + 1);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.Amount);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void ChangingAmountToSameAmountWontTriggerNotification()
        {
            using var monitoredItem = this._item.Monitor();

            this._item.SetAmount(this._item.Amount);

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.Amount);
            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void ChangingCurrentInventoryToDifferentValueWillNotify()
        {
            using var monitoredItem = this._item.Monitor();

            var otherInventory = new Mock<IInventory>();

            this._item.SetCurrentInventory(otherInventory.Object);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.CurrentInventory);
        }

        [TestMethod]
        public void ChangingCurrentInventoryToSameValueWontNotify()
        {
            using var monitoredItem = this._item.Monitor();

            this._item.SetCurrentInventory(null);

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.CurrentInventory);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void ChangingSingleWeightToNegativeAmountThrowsException(int singleWeight)
        {
            Action act = () => this._item.SetSingleWeight(singleWeight);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void ChangingSingleWeightToPositiveAmountRaisesEvent(int singleWeight)
        {
            using var monitoredItem = this._item.Monitor();

            this._item.SetSingleWeight(singleWeight);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.SingleWeight);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void SettingSingleWeightToOldValueDoesNotRaiseNotification()
        {
            using var monitoredItem = this._item.Monitor();

            this._item.SetSingleWeight(this._item.SingleWeight);

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.SingleWeight);
            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void SettingSingleWeightAboveInventoryCapacityThrowsException()
        {
            this._inventory.InsertItemAsync(this._item);

            Action act = () => this._item.SetSingleWeight(this._inventory.Capacity + 1);

            act.Should().Throw<InventoryCapacityException>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void SettingAmountAboveInventoryCapacityThrowsException(int amountDelta)
        {
            this._inventory.SetCapacity(10);
            this._item.SetSingleWeight(1);
            this._item.SetAmount(1);

            this._inventory.InsertItemAsync(this._item);

            Action act = () => this._item.SetAmount(10 + amountDelta);

            act.Should().Throw<InventoryCapacityException>();
        }

        [TestMethod]
        public void SettingSingleWeightAboveCapacityWithAmountAboveOneThrowsException()
        {
            this._inventory.SetCapacity(10);
            this._item.SetAmount(10);
            this._item.SetSingleWeight(1);

            this._inventory.InsertItemAsync(this._item);

            Action act = () => this._item.SetSingleWeight(2);

            act.Should().Throw<InventoryCapacityException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void SettingSingleWeightToCapacityDoesNotThrowInventoryCapacityException(int capacityDelta)
        {
            this._inventory.InsertItemAsync(this._item);

            Action act = () => this._item.SetSingleWeight(this._inventory.Capacity + capacityDelta);

            act.Should().NotThrow<InventoryCapacityException>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void SettingSingleWeightUpdatesValue(int singleWeight)
        {
            this._item.SetSingleWeight(singleWeight);

            this._item.SingleWeight.Should().Be(singleWeight);
        }

        [TestMethod]
        public void PassingMetaWithWrongTypeThrowsException()
        {
            var meta = new ItemMeta(ItemHandle, typeof(FakeItem), ItemDisplayName, ItemWeight, ItemFlags);

            Action act = () => new RealItem(meta, this._itemServices);

            act.Should().Throw<ArgumentException>()
                .Where(x =>
                    x.Message.Contains("mismatch")
                    && x.Message.Contains("type")
                    && x.Message.Contains("meta"));
        }

        [TestMethod]
        public async Task MergingTwoItemsCanMergeSignalsFalseWillThrowException()
        {
            var realItem = this._itemFactory.CreateItem(this._realMeta, 1);
            var fakeItem = this._itemFactory.CreateItem(this._fakeMeta, 1);

            realItem.CanMergeWith(fakeItem).Should().BeFalse();

            Func<Task> act = () => realItem.MergeItemAsync(fakeItem);

            (await act.Should().ThrowAsync<ArgumentException>())
                .Where(x => x.Message.Contains("merge") && x.Message.Contains("not"));
        }

        [TestMethod]
        public async Task MergingWithNullThrowsException()
        {
            Func<Task> act = () => this._item.MergeItemAsync(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task MergingWithMergableStrategyExecutesCorrectMethod()
        {
            this._itemMergeStrategyHandlerMock = new Mock<IItemMergeStrategyHandler>();

            this.Setup();

            var otherItem = this._itemFactory.CreateItem(this._realMeta, 1);

            this._itemMergeStrategyHandlerMock.Setup(x => x.CanBeMerged(otherItem, this._item)).Returns(true);

            await otherItem.MergeItemAsync(this._item);

            this._itemMergeStrategyHandlerMock.Verify(x => x.MergeItemWithAsync(otherItem, this._item), Times.Once);
        }

        [TestMethod]
        public async Task SplittingStrategyExecutesCorrectMethod()
        {
            this._itemSplitStrategyHandlerMock = new Mock<IItemSplitStrategyHandler>();
            this._itemFactoryMock = new Mock<IItemFactory>();

            this.Setup();

            var item = new RealItem(this._defaultRealMeta, this._itemServices);
            var factoryResultItem = new RealItem(this._defaultRealMeta, this._itemServices);

            this._itemFactoryMock.Setup(x => x.CreateItem(this._realMeta, 2))
                .Returns<ItemMeta, int>((meta, amount) =>
                {
                    factoryResultItem.SetAmount(amount);

                    return factoryResultItem;
                });

            this._itemSplitStrategyHandlerMock
                .Setup(x => x.SplitItemAsync(item, It.IsAny<IItem>()))
                .Returns(Task.CompletedTask);

            item.SetAmount(5);

            var resultItem = await item.SplitItemAsync(2);

            resultItem.Should()
                .NotBeNull()
                .And.Be(factoryResultItem);

            this._itemSplitStrategyHandlerMock.Verify(x => x.SplitItemAsync(item, It.IsAny<IItem>()), Times.Once);
        }

        [TestMethod]
        public async Task SplittingItemReturnsCorrectItem()
        {
            this._item.SetAmount(5);

            var resultItem = await this._item.SplitItemAsync(2);

            this._item.Amount.Should().Be(3);

            resultItem.Should().BeOfType(this._item.GetType());
            resultItem.Amount.Should().Be(2);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task SplittingWithNegativeAmountThrowsException(int targetAmount)
        {
            Func<Task> act = () => this._item.SplitItemAsync(targetAmount);

            (await act.Should().ThrowAsync<ArgumentOutOfRangeException>())
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public async Task SplittingWithAmountHigherThanItemAmountThrowsException(int amountDelta)
        {
            Func<Task> act = () => this._item.SplitItemAsync(this._item.Amount + amountDelta);

            (await act.Should().ThrowAsync<ArgumentOutOfRangeException>())
                .Where(x =>
                    x.Message.Contains((this._item.Amount - 1).ToString())
                    && x.Message.Contains("or lower")
                );
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SettingMovingLockedUpdatesMovingLocked(bool newValue)
        {
            this._item.MovingLocked = newValue == false; // set inverted value to force change

            using var monitoredItem = this._item.Monitor();

            this._item.MovingLocked = newValue;

            this._item.MovingLocked.Should().Be(newValue);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.MovingLocked);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SettingMovingLockedToSameValueDoesNotChangeValue(bool newValue)
        {
            this._item.MovingLocked = newValue;

            using var monitoredItem = this._item.Monitor();

            this._item.MovingLocked = newValue;

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.MovingLocked);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SettingLockedToSameValueDoesNotChangeValue(bool newValue)
        {
            this._item.Locked = newValue;

            using var monitoredItem = this._item.Monitor();

            this._item.Locked = newValue;

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.Locked);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SettingLockedUpdatesMovingLockedToo(bool newValue)
        {
            this._item.MovingLocked = false;
            this._item.Locked = newValue == false; // set inverted value to force change

            using var monitoredItem = this._item.Monitor();

            this._item.Locked = newValue;

            this._item.MovingLocked.Should().Be(newValue);
            this._item.Locked.Should().Be(newValue);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.MovingLocked);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.Locked);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SetMovingLockedTrueAndChangeLockedKeepsValue(bool newValue)
        {
            this._item.MovingLocked = true;

            using var monitoredItem = this._item.Monitor();

            this._item.Locked = newValue;

            this._item.MovingLocked.Should().BeTrue();

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.MovingLocked);
        }

    }
}
