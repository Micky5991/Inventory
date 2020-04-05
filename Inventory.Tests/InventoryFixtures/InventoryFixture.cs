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
            SetupItemTest();

            _itemMock = new Mock<IItem>();

            SetupDefaultServiceProvider();
        }

        [TestCleanup]
        public void Cleanup()
        {
            TearDownItemTest();
        }

        [TestMethod]
        public async Task InsertingItemToInventoryWillChangeCapacity()
        {
            await AddItemToInventoryAsync(10);

            AssertInventoryCapacity(10);
        }

        [TestMethod]
        public async Task RemovingItemFromInventoryWillChangeCapacity()
        {
            var item = await AddItemToInventoryAsync(10);

            AssertInventoryCapacity(10);

            await _inventory.RemoveItemAsync(item);

            AssertInventoryCapacity(0);
        }

        [TestMethod]
        public async Task CheckIfItemFitsWillReturnCorrectValuesAfterItemCollectionChange()
        {
            var item = await AddItemToInventoryAsync(InventoryCapacity);

            _inventory.DoesItemFit(item).Should().BeFalse();
        }

        [TestMethod]
        public void PassingNullToDoesItemFitWillThrowException()
        {
            Action act = () => _inventory.DoesItemFit(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CheckingIfFillingItemWillFitIntoInventoryShouldReturnTrue()
        {
            var item = new FakeItem(InventoryCapacity);

            _inventory.DoesItemFit(item).Should().BeTrue();
        }

        [TestMethod]
        public async Task CheckingIfSecondFillingItemWillFitIntoInventoryShouldReturnTrue()
        {
            await AddItemToInventoryAsync(InventoryCapacity - 10);

            var item = new FakeItem(10);

            _inventory.DoesItemFit(item).Should().BeTrue();
        }

        [TestMethod]
        public async Task CheckingIfSecondCapacityExceedingItemWillFitIntoInventoryShouldReturnFalse()
        {
            await AddItemToInventoryAsync(InventoryCapacity - 10);

            var item = new FakeItem(11);

            _inventory.DoesItemFit(item).Should().BeFalse();
        }

        [TestMethod]
        public void CallingNullForHandleOnDoesItemFitThrowsException()
        {
            Action act = () => _inventory.DoesItemFit((ItemMeta) null, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void CallingDoesItemFitWithFittingItemReturnsTrue(int weightDelta)
        {
            _inventory.SetCapacity(_defaultRealMeta.DefaultWeight + weightDelta);

            _inventory.DoesItemFit(_defaultRealMeta).Should().BeTrue();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-2)]
        [DataRow(-3)]
        public void CallingdoesItemFitWithExceedingWeightReturnsFalse(int weightDelta)
        {
            _inventory.SetCapacity(_defaultRealMeta.DefaultWeight + weightDelta);

            _inventory.DoesItemFit(_defaultRealMeta).Should().BeFalse();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CallingDoesItemFitWithNegativeAmountThrowsException(int amount)
        {
            _inventory.SetCapacity(1000);

            Action act = () => _inventory.DoesItemFit(ItemHandle, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CallingDoesItemFitWithNegativeAmountAndMetaThrowsException(int amount)
        {
            _inventory.SetCapacity(1000);

            Action act = () => _inventory.DoesItemFit(_defaultRealMeta, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        public void CallingDoesItemFitWithKnownHandleAndAvailableSpaceReturnsTrue()
        {
            _inventory.SetCapacity(1000);

            _inventory.DoesItemFit(ItemHandle, 1).Should().BeTrue();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CallingDoesItemFitWithInvalidHandleThrowsException(string handle)
        {
            Action act = () => _inventory.DoesItemFit(handle, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CallingDoesItemFitWithUnknownHandleThrowsException()
        {
            Action act = () => _inventory.DoesItemFit("unknownhandle", 1);

            act.Should().Throw<ItemMetaNotFoundException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CallingDoesItemFitWithMetaAndNegativeAmountThrowsException(int amount)
        {
            Action act = () => _inventory.DoesItemFit(_defaultRealMeta, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher") && x.Message.Contains("amount"));
        }

        private async Task<FakeItem> AddItemToInventoryAsync(int weight = 10)
        {
            var item = new FakeItem(weight);

            await _inventory.InsertItemAsync(item);

            return item;
        }

        private void AssertInventoryCapacity(int usedCapacity, int capacity = InventoryCapacity, IInventory inventory = null)
        {
            if (inventory == null)
            {
                inventory = _inventory;
            }

            inventory.Capacity.Should().Be(capacity);
            inventory.UsedCapacity.Should().Be(usedCapacity);
            inventory.AvailableCapacity.Should().Be(capacity - usedCapacity);
        }

    }
}