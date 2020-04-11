using System;
using FluentAssertions;
using Micky5991.Inventory.Entities.Item;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.EventArgs;
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

        private Mock<IInventory> inventoryMock;

        [TestInitialize]
        public void Setup()
        {
            Item.MinimalItemAmount = 0;

            this.SetupItemTest();

            this.inventoryMock = new Mock<IInventory>();

        }

        [TestCleanup]
        public void TearDown()
        {
            this.TearDownItemTest();
        }

        [TestMethod]
        public void CreatingItemWillBeSuccessful()
        {
            var item = this.Item;

            item.Should()
                .NotBeNull()
                .And.BeAssignableTo<IItem>();
        }

        [TestMethod]
        public void CreatingItemWillSetParametersCorrectly()
        {
            var item = new RealItem(this.RealMeta, ServiceUtils.CreateItemServices());

            item.Meta.Should().Be(this.RealMeta);
            item.Handle.Should().Be(this.RealMeta.Handle);
            item.SingleWeight.Should().Be(this.RealMeta.DefaultWeight);
            item.DisplayName.Should().Be(this.RealMeta.DisplayName);
            item.DefaultDisplayName.Should().Be(this.RealMeta.DisplayName);
            item.Amount.Should().Be(Math.Max(Item.MinimalItemAmount, 1));
            item.Stackable.Should().BeTrue();
            item.WeightChangable.Should().BeFalse();

            item.RuntimeId.Should().NotBe(Guid.Empty);
        }

        [TestMethod]
        [DataRow(ItemFlags.NotStackable, false)]
        [DataRow(ItemFlags.None, true)]
        public void SettingNonStackableFlagWillBeInterpretedCorrectly(ItemFlags flags, bool stackable)
        {
            this.SetupServiceProvider(new ItemMeta(this.DefaultRealMeta.Handle, typeof(RealItem), this.DefaultRealMeta.DisplayName, this.DefaultRealMeta.DefaultWeight, flags));

            this.Item.Stackable.Should().Be(stackable);
        }

        [TestMethod]
        [DataRow(ItemFlags.WeightChangable, true)]
        [DataRow(ItemFlags.None, false)]
        public void SettingWeightChangableFlagWillBeInterpretedCorrectly(ItemFlags flags, bool weightChangable)
        {
            this.SetupServiceProvider(new ItemMeta(this.DefaultRealMeta.Handle, typeof(RealItem), this.DefaultRealMeta.DisplayName, this.DefaultRealMeta.DefaultWeight, flags));

            this.Item.WeightChangable.Should().Be(weightChangable);
        }

        [TestMethod]
        public void SettingAmountOfNonStackableItemWillThrowException()
        {
            this.SetupServiceProvider(new ItemMeta(this.DefaultRealMeta.Handle, typeof(RealItem), this.DefaultRealMeta.DisplayName, this.DefaultRealMeta.DefaultWeight, ItemFlags.NotStackable));

            Action act = () => this.Item.SetAmount(2);
            act.Should().Throw<ItemNotStackableException>();
        }

        [TestMethod]
        public void IncreasingAmountOfNonStackableItemWillThrowException()
        {
            this.SetupServiceProvider(new ItemMeta(this.DefaultRealMeta.Handle, typeof(RealItem), this.DefaultRealMeta.DisplayName, this.DefaultRealMeta.DefaultWeight, ItemFlags.NotStackable));

            Action act = () => this.Item.IncreaseAmount(2);
            act.Should().Throw<ItemNotStackableException>();
        }

        [TestMethod]
        public void CreatingItemWithNullItemMetaWillThrowException()
        {
            Action act = () => new RealItem(null, this.ItemServices);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ChangingDisplayNameUpdatesValueCorrecly()
        {
            this.Item.SetDisplayName("Cool");

            this.Item.DisplayName.Should().Be("Cool");
        }

        [TestMethod]
        public void ChangingDisplayNameKeepsValueInDefaultDisplayNameSame()
        {
            this.Item.SetDisplayName("Other");

            var oldName = this.Item.DefaultDisplayName;

            this.Item.DefaultDisplayName.Should().Be(oldName);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void SettingDisplayNameToNullThrowsException(string displayName)
        {
            var oldName = this.Item.DisplayName;
            Action act = () => this.Item.SetDisplayName(displayName);

            act.Should().Throw<ArgumentNullException>();
            this.Item.DisplayName.Should().Be(oldName);
        }

        [TestMethod]
        public void CanMergeCheckWithNullSourceItemWillResultInException()
        {
            Action act = () => this.Item.CanMergeWith(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void SettingAmountWillUpdateWeightAndAmount(int amountDelta)
        {
            var singleWeight = this.RealMeta.DefaultWeight;
            var targetAmount = Item.MinimalItemAmount + amountDelta;

            this.Item.SetAmount(targetAmount);

            this.Item.Amount.Should().Be(targetAmount);
            this.Item.TotalWeight.Should().Be(singleWeight * targetAmount);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void IncreasingAmountIncreasesTotalAmount(int amountDelta)
        {
            var oldAmount = this.Item.Amount;

            this.Item.IncreaseAmount(amountDelta);

            this.Item.Amount.Should().Be(oldAmount + amountDelta);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void ReducingAmountReducesTotalAmount(int amountDelta)
        {
            this.Item.SetAmount(amountDelta + 1);

            var oldAmount = this.Item.Amount;

            this.Item.ReduceAmount(amountDelta);

            this.Item.Amount.Should().Be(oldAmount - amountDelta);
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(10)]
        [DataRow(100)]
        public void ReducingAmountByAmountHigherThanTotalAmountThrowsException(int amountDelta)
        {
            this.Item.SetAmount(1);

            Action act = () => this.Item.ReduceAmount(amountDelta);

            act.Should().Throw<ArgumentOutOfRangeException>()
               .Where(x => x.Message.Contains(this.Item.Amount.ToString()));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-10)]
        public void ReducingAmountByZeroOrLowerValueThrowsException(int amountDelta)
        {
            this.Item.SetAmount(1);

            Action act = () => this.Item.ReduceAmount(amountDelta);

            act.Should().Throw<ArgumentOutOfRangeException>()
               .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-10)]
        public void IncreasingAmountByZeroOrLowerValueThrowsException(int amountDelta)
        {
            this.Item.SetAmount(1);

            Action act = () => this.Item.IncreaseAmount(amountDelta);

            act.Should().Throw<ArgumentOutOfRangeException>()
               .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(3)]
        [DataRow(7)]
        [DataRow(11)]
        public void TotalWeightIsMultipleOfSingleWeightAndAmount(int amountDelta)
        {
            var actualAmount = Item.MinimalItemAmount + amountDelta;

            this.Item.SetAmount(actualAmount);

            (this.Item.TotalWeight / this.Item.SingleWeight).Should().Be(actualAmount);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        public void SettingItemAmountBelowMinimalAllowedItemAmountThrowsException(int amountDelta)
        {
            var oldAmount = this.Item.Amount;
            Action act = () => this.Item.SetAmount(Item.MinimalItemAmount + amountDelta);

            act.Should().Throw<ArgumentOutOfRangeException>()
               .Where(x => x.Message.Contains($"{Math.Max(Item.MinimalItemAmount, 0)} or higher"));

            this.Item.Amount.Should().Be(oldAmount);
        }

        [TestMethod]
        public void SettingCurrentInventoryWillUpdateValue()
        {
            this.Item.SetCurrentInventory(this.inventoryMock.Object);
            this.Item.CurrentInventory.Should().Be(this.inventoryMock.Object);

            this.Item.SetCurrentInventory(null);
            this.Item.CurrentInventory.Should().BeNull();
        }

        [TestMethod]
        public void DetectionOfMinimalAmountDetectsRightAmount()
        {
            Item.MinimalItemAmount = -10;

            Action act = () => this.Item.SetAmount(-5);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("0"));
        }

        [TestMethod]
        public void ChangingDisplayNameTriggersNotification()
        {
            using var monitoredSubject = this.Item.Monitor();

            this.Item.SetDisplayName(this.Item.DisplayName + "ADD");

            monitoredSubject.Should().RaisePropertyChangeFor(x => x.DisplayName);
        }

        [TestMethod]
        public void ChangingDisplayNameToCurrentValueDoesNotTriggerNotification()
        {
            using var monitoredSubject = this.Item.Monitor();

            this.Item.SetDisplayName(this.Item.DisplayName);

            monitoredSubject.Should().NotRaisePropertyChangeFor(x => x.DisplayName);
        }

        [TestMethod]
        public void ChangingAmountWillNotifyAmountAndTotalWeight()
        {
            using var monitoredItem = this.Item.Monitor();

            this.Item.SetAmount(this.Item.Amount + 1);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.Amount);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void IncreasingAmountWillNotifyAmountAndTotalWeight()
        {
            using var monitoredItem = this.Item.Monitor();

            this.Item.IncreaseAmount(1);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.Amount);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void ReducingAmountWillNotifyAmountAndTotalWeight()
        {
            using var monitoredItem = this.Item.Monitor();

            this.Item.ReduceAmount(1);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.Amount);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void ChangingAmountToSameAmountWontTriggerNotification()
        {
            using var monitoredItem = this.Item.Monitor();

            this.Item.SetAmount(this.Item.Amount);

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.Amount);
            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void ChangingCurrentInventoryToDifferentValueWillNotify()
        {
            using var monitoredItem = this.Item.Monitor();

            var otherInventory = new Mock<IInventory>();

            this.Item.SetCurrentInventory(otherInventory.Object);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.CurrentInventory);
        }

        [TestMethod]
        public void ChangingCurrentInventoryToSameValueWontNotify()
        {
            using var monitoredItem = this.Item.Monitor();

            this.Item.SetCurrentInventory(null);

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.CurrentInventory);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void ChangingSingleWeightToNegativeAmountThrowsException(int singleWeight)
        {
            Action act = () => this.Item.SetSingleWeight(singleWeight);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void ChangingSingleWeightToPositiveAmountRaisesEvent(int singleWeight)
        {
            using var monitoredItem = this.Item.Monitor();

            this.Item.SetSingleWeight(singleWeight);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.SingleWeight);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void SettingSingleWeightToOldValueDoesNotRaiseNotification()
        {
            using var monitoredItem = this.Item.Monitor();

            this.Item.SetSingleWeight(this.Item.SingleWeight);

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.SingleWeight);
            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void SettingSingleWeightAboveInventoryCapacityThrowsException()
        {
            this.Inventory.InsertItem(this.Item);

            Action act = () => this.Item.SetSingleWeight(this.Inventory.Capacity + 1);

            act.Should().Throw<InventoryCapacityException>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void SettingAmountAboveInventoryCapacityThrowsException(int amountDelta)
        {
            this.Inventory.SetCapacity(10);
            this.Item.SetSingleWeight(1);
            this.Item.SetAmount(1);

            this.Inventory.InsertItem(this.Item);

            Action act = () => this.Item.SetAmount(10 + amountDelta);

            act.Should().Throw<InventoryCapacityException>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void IncreasingAmountAboveInventoryCapacityThrowsException(int amountDelta)
        {
            this.Inventory.SetCapacity(10);
            this.Item.SetSingleWeight(1);
            this.Item.SetAmount(1);

            this.Inventory.InsertItem(this.Item);

            this.Item.SetAmount(10);

            Action act = () => this.Item.IncreaseAmount(amountDelta);

            act.Should().Throw<InventoryCapacityException>();
        }

        [TestMethod]
        public void SettingSingleWeightAboveCapacityWithAmountAboveOneThrowsException()
        {
            this.Inventory.SetCapacity(10);
            this.Item.SetAmount(10);
            this.Item.SetSingleWeight(1);

            this.Inventory.InsertItem(this.Item);

            Action act = () => this.Item.SetSingleWeight(2);

            act.Should().Throw<InventoryCapacityException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void SettingSingleWeightToCapacityDoesNotThrowInventoryCapacityException(int capacityDelta)
        {
            this.Inventory.InsertItem(this.Item);

            Action act = () => this.Item.SetSingleWeight(this.Inventory.Capacity + capacityDelta);

            act.Should().NotThrow<InventoryCapacityException>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void SettingSingleWeightUpdatesValue(int singleWeight)
        {
            this.Item.SetSingleWeight(singleWeight);

            this.Item.SingleWeight.Should().Be(singleWeight);
        }

        [TestMethod]
        public void PassingMetaWithWrongTypeThrowsException()
        {
            var meta = new ItemMeta(ItemHandle, typeof(FakeItem), ItemDisplayName, ItemWeight, ItemFlags);

            Action act = () => new RealItem(meta, this.ItemServices);

            act.Should().Throw<ArgumentException>()
                .Where(x =>
                    x.Message.Contains("mismatch")
                    && x.Message.Contains("type")
                    && x.Message.Contains("meta"));
        }

        [TestMethod]
        public void MergingTwoItemsCanMergeSignalsFalseWillThrowException()
        {
            var realItem = this.ItemFactory.CreateItem(this.RealMeta, 1);
            var fakeItem = this.ItemFactory.CreateItem(this.FakeMeta, 1);

            realItem.CanMergeWith(fakeItem).Should().BeFalse();

            Action act = () => realItem.MergeItem(fakeItem);

            (act.Should().Throw<ArgumentException>())
                .Where(x => x.Message.Contains("merge") && x.Message.Contains("not"));
        }

        [TestMethod]
        public void MergingWithNullThrowsException()
        {
            Action act = () => this.Item.MergeItem(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void MergingWithMergableStrategyExecutesCorrectMethod()
        {
            this.ItemMergeStrategyHandlerMock = new Mock<IItemMergeStrategyHandler>();

            this.Setup();

            var otherItem = this.ItemFactory.CreateItem(this.RealMeta, 1);

            this.ItemMergeStrategyHandlerMock.Setup(x => x.CanBeMerged(otherItem, this.Item)).Returns(true);

            otherItem.MergeItem(this.Item);

            this.ItemMergeStrategyHandlerMock.Verify(x => x.MergeItemWith(otherItem, this.Item), Times.Once);
        }

        [TestMethod]
        public void SplittingStrategyExecutesCorrectMethod()
        {
            this.ItemSplitStrategyHandlerMock = new Mock<IItemSplitStrategyHandler>();
            this.ItemFactoryMock = new Mock<IItemFactory>();

            this.Setup();

            var item = new RealItem(this.DefaultRealMeta, this.ItemServices);
            var factoryResultItem = new RealItem(this.DefaultRealMeta, this.ItemServices);

            this.ItemFactoryMock.Setup(x => x.CreateItem(this.RealMeta, 2))
                .Returns<ItemMeta, int>((meta, amount) =>
                {
                    factoryResultItem.SetAmount(amount);

                    return factoryResultItem;
                });

            this.ItemSplitStrategyHandlerMock
                .Setup(x => x.SplitItem(item, It.IsAny<IItem>()));

            item.SetAmount(5);

            var resultItem = item.SplitItem(2);

            resultItem.Should()
                .NotBeNull()
                .And.Be(factoryResultItem);

            this.ItemSplitStrategyHandlerMock.Verify(x => x.SplitItem(item, It.IsAny<IItem>()), Times.Once);
        }

        [TestMethod]
        public void SplittingItemReturnsCorrectItem()
        {
            this.Item.SetAmount(5);

            var resultItem = this.Item.SplitItem(2);

            this.Item.Amount.Should().Be(3);

            resultItem.Should().BeOfType(this.Item.GetType());
            resultItem.Amount.Should().Be(2);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void SplittingWithNegativeAmountThrowsException(int targetAmount)
        {
            Action act = () => this.Item.SplitItem(targetAmount);

            (act.Should().Throw<ArgumentOutOfRangeException>())
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void SplittingWithAmountHigherThanItemAmountThrowsException(int amountDelta)
        {
            Action act = () => this.Item.SplitItem(this.Item.Amount + amountDelta);

            (act.Should().Throw<ArgumentOutOfRangeException>())
                .Where(x =>
                    x.Message.Contains((this.Item.Amount - 1).ToString())
                    && x.Message.Contains("or lower")
                );
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SettingMovingLockedUpdatesMovingLocked(bool newValue)
        {
            this.Item.MovingLocked = newValue == false; // set inverted value to force change

            using var monitoredItem = this.Item.Monitor();

            this.Item.MovingLocked = newValue;

            this.Item.MovingLocked.Should().Be(newValue);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.MovingLocked);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SettingMovingLockedToSameValueDoesNotChangeValue(bool newValue)
        {
            this.Item.MovingLocked = newValue;

            using var monitoredItem = this.Item.Monitor();

            this.Item.MovingLocked = newValue;

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.MovingLocked);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SettingLockedToSameValueDoesNotChangeValue(bool newValue)
        {
            this.Item.Locked = newValue;

            using var monitoredItem = this.Item.Monitor();

            this.Item.Locked = newValue;

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.Locked);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SettingLockedUpdatesMovingLockedToo(bool newValue)
        {
            this.Item.MovingLocked = false;
            this.Item.Locked = newValue == false; // set inverted value to force change

            using var monitoredItem = this.Item.Monitor();

            this.Item.Locked = newValue;

            this.Item.MovingLocked.Should().Be(newValue);
            this.Item.Locked.Should().Be(newValue);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.MovingLocked);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.Locked);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SetMovingLockedTrueAndChangeLockedKeepsValue(bool newValue)
        {
            this.Item.MovingLocked = true;

            using var monitoredItem = this.Item.Monitor();

            this.Item.Locked = newValue;

            this.Item.MovingLocked.Should().BeTrue();

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.MovingLocked);
        }

        [TestMethod]
        public void InitializingItemExecutesEvent()
        {
            var item = new RealItem(this.DefaultRealMeta, this.ItemServices);

            using var monitoredItem = item.Monitor();

            item.Initialize();

            monitoredItem.Should()
                         .Raise(nameof(IItem.Initialized))
                         .WithSender(item)
                         .WithArgs<ItemInitializedEventArgs>(args => args.Item == item);
        }

    }
}
