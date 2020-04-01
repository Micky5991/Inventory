using System;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class BasicItemMergeStrategyFixture : ItemTest
    {

        private BasicItemMergeStrategy _strategy;

        private Mock<IItem> _targetItem;
        private Mock<IItem> _sourceItem;

        [TestInitialize]
        public void Setup()
        {
            _strategy = new BasicItemMergeStrategy();

            _targetItem = new Mock<IItem>();
            _sourceItem = new Mock<IItem>();

            SetupItemTest();
        }

        [TestCleanup]
        public void Teardown()
        {
            TearDownItemTest();
        }

        [TestMethod]
        public void PassingNullAsTargetInMergableCheckWillThrowArgumentNullException()
        {
            Action act = () => _strategy.CanBeMerged(null, _sourceItem.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void PassingNullAsSourceInMergableCheckWillThrowArgumentNullException()
        {
            Action act = () => _strategy.CanBeMerged(_targetItem.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public async Task PassingNullAsTargetInMergingWillThrowArgumentNullException()
        {
            Func<Task> act = () => _strategy.MergeItemWithAsync(null, _sourceItem.Object);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task PassingNullAsSourceInMergingWillThrowArgumentNullException()
        {
            Func<Task> act = () => _strategy.MergeItemWithAsync(_targetItem.Object, null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void TwoNonStackableItemAreNotMergable(bool targetStackable, bool sourceStackable)
        {
            SetupMock(_targetItem, targetStackable, "item", 1, 1);
            SetupMock(_sourceItem, sourceStackable, "item", 1, 1);

            _strategy.CanBeMerged(_targetItem.Object, _sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void TwoIdenticalStackableItemAreMergable()
        {
            SetupMock(_targetItem, true, "item", 1, 1);
            SetupMock(_sourceItem, true, "item", 1, 1);

            _strategy.CanBeMerged(_targetItem.Object, _sourceItem.Object).Should().BeTrue();
        }

        [TestMethod]
        public void SourceItemWithZeroAmountIsNotMergable()
        {
            SetupMock(_targetItem, true, "item1", 1, 1);
            SetupMock(_sourceItem, true, "item2", 0, 1);

            _strategy.CanBeMerged(_targetItem.Object, _sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void StackableItemsWithDifferentHandlesAreNotMergable()
        {
            SetupMock(_targetItem, true, "item1", 1, 1);
            SetupMock(_sourceItem, true, "item2", 1, 1);

            _strategy.CanBeMerged(_targetItem.Object, _sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void DifferentSingleWeightWillPreventMerging()
        {
            SetupMock(_targetItem, true, "item", 1, 1);
            SetupMock(_sourceItem, true, "item", 1, 2);

            _strategy.CanBeMerged(_targetItem.Object, _sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void SameItemIsNotMergable()
        {
            SetupMock(_targetItem, true, "item", 1, 1);

            _strategy.CanBeMerged(_targetItem.Object, _targetItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public async Task MergingItemExecutesAmountSumAndAmountClearOnSource()
        {
            _targetItem.SetupGet(x => x.Amount).Returns(1);
            _sourceItem.SetupGet(x => x.Amount).Returns(2);

            _targetItem.Setup(x => x.SetAmount(1 + 2));
            _sourceItem.Setup(x => x.SetAmount(0));

            await _strategy.MergeItemWithAsync(_targetItem.Object, _sourceItem.Object);

            _targetItem.VerifyGet(x => x.Amount, Times.Once);
            _sourceItem.VerifyGet(x => x.Amount, Times.Once);

            _targetItem.Verify(x => x.SetAmount(1 + 2), Times.Once);
            _sourceItem.Verify(x => x.SetAmount(0), Times.Once);
        }

        private void SetupMock(Mock<IItem> mock, bool stackable = true, string handle = "item", int amount = 1,
            int singleWeight = 1)
        {
            mock.SetupGet(x => x.Stackable).Returns(stackable);
            mock.SetupGet(x => x.Handle).Returns(handle);
            mock.SetupGet(x => x.Amount).Returns(amount);
            mock.SetupGet(x => x.SingleWeight).Returns(singleWeight);
        }

    }
}
