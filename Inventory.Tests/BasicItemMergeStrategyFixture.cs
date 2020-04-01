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
    public class BasicItemMergeStrategyFixture
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

    }
}
