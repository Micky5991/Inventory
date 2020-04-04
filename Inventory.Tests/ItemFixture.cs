using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Item;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Extensions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Micky5991.Inventory.Tests.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ItemFixture : ItemTest
    {

        private Mock<IInventory> _inventoryMock;
        private Mock<IItemMergeStrategyHandler> _mergeStrategyHandlerMock;
        private Mock<IItemSplitStrategyHandler> _splitStrategyHandlerMock;
        private Mock<IItemFactory> _itemFactoryMock;

        private AggregatedItemServices _itemServices;

        [TestInitialize]
        public void Setup()
        {
            Item.MinimalItemAmount = 0;

            SetupItemTest();

            _inventoryMock = new Mock<IInventory>();

            _mergeStrategyHandlerMock = new Mock<IItemMergeStrategyHandler>();
            _splitStrategyHandlerMock = new Mock<IItemSplitStrategyHandler>();

            _itemFactoryMock = new Mock<IItemFactory>();

            _itemServices = new AggregatedItemServices(_mergeStrategyHandlerMock.Object, _splitStrategyHandlerMock.Object, _itemFactoryMock.Object);

        }

        [TestCleanup]
        public void TearDown()
        {
            TearDownItemTest();
        }

        [TestMethod]
        public void CreatingItemWillBeSuccessful()
        {
            SetupDefaultServiceProvider();

            var item = _item;

            item.Should()
                .NotBeNull()
                .And.BeAssignableTo<IItem>();
        }

        [TestMethod]
        public void CreatingItemWillSetParametersCorrectly()
        {
            SetupDefaultServiceProvider();

            var item = new RealItem(_realMeta, ServiceUtils.CreateItemServices());

            item.Meta.Should().Be(_realMeta);
            item.Handle.Should().Be(_realMeta.Handle);
            item.SingleWeight.Should().Be(_realMeta.DefaultWeight);
            item.DisplayName.Should().Be(_realMeta.DisplayName);
            item.DefaultDisplayName.Should().Be(_realMeta.DisplayName);
            item.Amount.Should().Be(Math.Max(Item.MinimalItemAmount, 1));
            item.Stackable.Should().BeTrue();

            item.RuntimeId.Should().NotBe(Guid.Empty);
        }

        [TestMethod]
        [DataRow(ItemFlags.NotStackable, false)]
        [DataRow(ItemFlags.None, true)]
        public void SettingNonStackableFlagWillBeInterpretedCorrectly(ItemFlags flags, bool stackable)
        {
            SetupServiceProvider(new ItemMeta(_defaultRealMeta.Handle, typeof(RealItem), _defaultRealMeta.DisplayName, _defaultRealMeta.DefaultWeight, flags));

            _item.Stackable.Should().Be(stackable);
        }

        [TestMethod]
        public void SettingAmountOfNonStackableItemWillThrowException()
        {
            SetupServiceProvider(new ItemMeta(_defaultRealMeta.Handle, typeof(RealItem), _defaultRealMeta.DisplayName, _defaultRealMeta.DefaultWeight, ItemFlags.NotStackable));

            Action act = () => _item.SetAmount(2);
            act.Should().Throw<ItemNotStackableException>();
        }

        [TestMethod]
        public void CreatingItemWithNullItemMetaWillThrowException()
        {
            Action act = () => new RealItem(null, _itemServices);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ChangingDisplayNameUpdatesValueCorrecly()
        {
            SetupDefaultServiceProvider();

            _item.SetDisplayName("Cool");

            _item.DisplayName.Should().Be("Cool");
        }

        [TestMethod]
        public void ChangingDisplayNameKeepsValueInDefaultDisplayNameSame()
        {
            SetupDefaultServiceProvider();

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
            SetupDefaultServiceProvider();

            var oldName = _item.DisplayName;
            Action act = () => _item.SetDisplayName(displayName);

            act.Should().Throw<ArgumentNullException>();
            _item.DisplayName.Should().Be(oldName);
        }

        [TestMethod]
        public void CanMergeCheckWithNullSourceItemWillResultInException()
        {
            SetupDefaultServiceProvider();

            Action act = () => _item.CanMergeWith(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        public void SettingAmountWillUpdateWeightAndAmount(int amountDelta)
        {
            SetupDefaultServiceProvider();

            var singleWeight = _realMeta.DefaultWeight;
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
            SetupDefaultServiceProvider();

            var actualAmount = Item.MinimalItemAmount + amountDelta;

            _item.SetAmount(actualAmount);

            (_item.TotalWeight / _item.SingleWeight).Should().Be(actualAmount);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        public void SettingItemAmountBelowMinimalAllowedItemAmountThrowsException(int amountDelta)
        {
            SetupDefaultServiceProvider();

            var oldAmount = _item.Amount;
            Action act = () => _item.SetAmount(Item.MinimalItemAmount + amountDelta);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains($"{Math.Max(Item.MinimalItemAmount, 0)} or higher"));

            _item.Amount.Should().Be(oldAmount);
        }

        [TestMethod]
        public void SettingCurrentInventoryWillUpdateValue()
        {
            SetupDefaultServiceProvider();

            _item.SetCurrentInventory(_inventoryMock.Object);
            _item.CurrentInventory.Should().Be(_inventoryMock.Object);

            _item.SetCurrentInventory(null);
            _item.CurrentInventory.Should().BeNull();
        }

        [TestMethod]
        public void DetectionOfMinimalAmountDetectsRightAmount()
        {
            SetupDefaultServiceProvider();

            Item.MinimalItemAmount = -10;

            Action act = () => _item.SetAmount(-5);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("0"));
        }

        [TestMethod]
        public void ChangingDisplayNameTriggersNotification()
        {
            SetupDefaultServiceProvider();

            using var monitoredSubject = _item.Monitor();

            _item.SetDisplayName(_item.DisplayName + "ADD");

            monitoredSubject.Should().RaisePropertyChangeFor(x => x.DisplayName);
        }

        [TestMethod]
        public void ChangingDisplayNameToCurrentValueDoesNotTriggerNotification()
        {
            SetupDefaultServiceProvider();

            using var monitoredSubject = _item.Monitor();

            _item.SetDisplayName(_item.DisplayName);

            monitoredSubject.Should().NotRaisePropertyChangeFor(x => x.DisplayName);
        }

        [TestMethod]
        public void ChangingAmountWillNotifyAmountAndTotalWeight()
        {
            SetupDefaultServiceProvider();

            using var monitoredItem = _item.Monitor();

            _item.SetAmount(_item.Amount + 1);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.Amount);
            monitoredItem.Should().RaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void ChangingAmountToSameAmountWontTriggerNotification()
        {
            SetupDefaultServiceProvider();

            using var monitoredItem = _item.Monitor();

            _item.SetAmount(_item.Amount);

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.Amount);
            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.TotalWeight);
        }

        [TestMethod]
        public void ChangingCurrentInventoryToDifferentValueWillNotify()
        {
            SetupDefaultServiceProvider();

            using var monitoredItem = _item.Monitor();

            var otherInventory = new Mock<IInventory>();

            _item.SetCurrentInventory(otherInventory.Object);

            monitoredItem.Should().RaisePropertyChangeFor(x => x.CurrentInventory);
        }

        [TestMethod]
        public void ChangingCurrentInventoryToSameValueWontNotify()
        {
            SetupDefaultServiceProvider();

            using var monitoredItem = _item.Monitor();

            _item.SetCurrentInventory(null);

            monitoredItem.Should().NotRaisePropertyChangeFor(x => x.CurrentInventory);
        }

        [TestMethod]
        public void PassingMetaWithWrongTypeThrowsException()
        {
            SetupDefaultServiceProvider();

            var meta = new ItemMeta(ItemHandle, typeof(FakeItem), ItemDisplayName, ItemWeight, ItemFlags);

            Action act = () => new RealItem(meta, _itemServices);

            act.Should().Throw<ArgumentException>()
                .Where(x =>
                    x.Message.Contains("mismatch")
                    && x.Message.Contains("type")
                    && x.Message.Contains("meta"));
        }

        [TestMethod]
        public async Task MergingTwoItemsCanMergeSignalsFalseWillThrowException()
        {
            SetupDefaultServiceProvider();

            var realItem = _itemFactory.CreateItem(_realMeta, 1);
            var fakeItem = _itemFactory.CreateItem(_fakeMeta, 1);

            realItem.CanMergeWith(fakeItem).Should().BeFalse();

            Func<Task> act = () => realItem.MergeItemAsync(fakeItem);

            (await act.Should().ThrowAsync<ArgumentException>())
                .Where(x => x.Message.Contains("merge") && x.Message.Contains("not"));
        }

        [TestMethod]
        public async Task MergingWithNullThrowsException()
        {
            SetupDefaultServiceProvider();

            Func<Task> act = () => _item.MergeItemAsync(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task MergingWithMergableStrategyExecutesCorrectMethod()
        {
            SetupDefaultServiceProvider();

            var item = new RealItem(_realMeta, _itemServices);

            _mergeStrategyHandlerMock.Setup(x => x.CanBeMerged(item, _item)).Returns(true);

            await item.MergeItemAsync(_item);

            _mergeStrategyHandlerMock.Verify(x => x.MergeItemWithAsync(item, _item), Times.Once);
        }

        [TestMethod]
        public async Task SplittingStrategyExecutesCorrectMethod()
        {
            SetupDefaultServiceProvider();

            var item = new RealItem(_defaultRealMeta, _itemServices);
            var factoryResultItem = new RealItem(_defaultRealMeta, _itemServices);

            _itemFactoryMock.Setup(x => x.CreateItem(_realMeta, 2))
                .Returns<ItemMeta, int>((meta, amount) =>
                {
                    factoryResultItem.SetAmount(amount);

                    return factoryResultItem;
                });

            _splitStrategyHandlerMock
                .Setup(x => x.SplitItemAsync(item, It.IsAny<IItem>()))
                .Returns(Task.CompletedTask);

            item.SetAmount(5);

            var resultItem = await item.SplitItemAsync(2);

            resultItem.Should()
                .NotBeNull()
                .And.Be(factoryResultItem);

            _splitStrategyHandlerMock.Verify(x => x.SplitItemAsync(item, It.IsAny<IItem>()), Times.Once);
        }

        [TestMethod]
        public async Task SplittingItemReturnsCorrectItem()
        {
            SetupDefaultServiceProvider();

            _item.SetAmount(5);

            var resultItem = await _item.SplitItemAsync(2);

            _item.Amount.Should().Be(3);

            resultItem.Should().BeOfType(_item.GetType());
            resultItem.Amount.Should().Be(2);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public async Task SplittingWithNegativeAmountThrowsException(int targetAmount)
        {
            SetupDefaultServiceProvider();

            Func<Task> act = () => _item.SplitItemAsync(targetAmount);

            (await act.Should().ThrowAsync<ArgumentOutOfRangeException>())
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public async Task SplittingWithAmountHigherThanItemAmountThrowsException(int amountDelta)
        {
            SetupDefaultServiceProvider();

            Func<Task> act = () => _item.SplitItemAsync(_item.Amount + amountDelta);

            (await act.Should().ThrowAsync<ArgumentOutOfRangeException>())
                .Where(x =>
                    x.Message.Contains((_item.Amount - 1).ToString())
                    && x.Message.Contains("or lower")
                    );
        }

    }
}
