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
            this._strategy = new BasicItemMergeStrategy();

            this._targetItem = new Mock<IItem>();
            this._sourceItem = new Mock<IItem>();

            this.SetupItemTest();
        }

        [TestCleanup]
        public void Teardown()
        {
            this.TearDownItemTest();
        }

        [TestMethod]
        public void PassingNullAsTargetInMergableCheckWillThrowArgumentNullException()
        {
            Action act = () => this._strategy.CanBeMerged(null, this._sourceItem.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void PassingNullAsSourceInMergableCheckWillThrowArgumentNullException()
        {
            Action act = () => this._strategy.CanBeMerged(this._targetItem.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public async Task PassingNullAsTargetInMergingWillThrowArgumentNullException()
        {
            Func<Task> act = () => this._strategy.MergeItemWithAsync(null, this._sourceItem.Object);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task PassingNullAsSourceInMergingWillThrowArgumentNullException()
        {
            Func<Task> act = () => this._strategy.MergeItemWithAsync(this._targetItem.Object, null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void TwoNonStackableItemAreNotMergable(bool targetStackable, bool sourceStackable)
        {
            this.SetupMock(this._targetItem, targetStackable, "item", 1, 1);
            this.SetupMock(this._sourceItem, sourceStackable, "item", 1, 1);

            this._strategy.CanBeMerged(this._targetItem.Object, this._sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void TwoIdenticalStackableItemAreMergable()
        {
            this.SetupMock(this._targetItem, true, "item", 1, 1);
            this.SetupMock(this._sourceItem, true, "item", 1, 1);

            this._strategy.CanBeMerged(this._targetItem.Object, this._sourceItem.Object).Should().BeTrue();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void SourceItemWithZeroAmountIsNotMergable(int sourceAmount)
        {
            this.SetupMock(this._targetItem, true, "item", 1, 1);
            this.SetupMock(this._sourceItem, true, "item", sourceAmount, 1);

            this._strategy.CanBeMerged(this._targetItem.Object, this._sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void StackableItemsWithDifferentHandlesAreNotMergable()
        {
            this.SetupMock(this._targetItem, true, "item1", 1, 1);
            this.SetupMock(this._sourceItem, true, "item2", 1, 1);

            this._strategy.CanBeMerged(this._targetItem.Object, this._sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void DifferentSingleWeightWillPreventMerging()
        {
            this.SetupMock(this._targetItem, true, "item", 1, 1);
            this.SetupMock(this._sourceItem, true, "item", 1, 2);

            this._strategy.CanBeMerged(this._targetItem.Object, this._sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void SameItemIsNotMergable()
        {
            this.SetupMock(this._targetItem, true, "item", 1, 1);

            this._strategy.CanBeMerged(this._targetItem.Object, this._targetItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public async Task MergingItemExecutesAmountSumAndAmountClearOnSource()
        {
            this._targetItem.SetupGet(x => x.Amount).Returns(1);
            this._sourceItem.SetupGet(x => x.Amount).Returns(2);

            this._targetItem.Setup(x => x.SetAmount(1 + 2));
            this._sourceItem.Setup(x => x.SetAmount(0));

            await this._strategy.MergeItemWithAsync(this._targetItem.Object, this._sourceItem.Object);

            this._targetItem.VerifyGet(x => x.Amount, Times.Once);
            this._sourceItem.VerifyGet(x => x.Amount, Times.Once);

            this._targetItem.Verify(x => x.SetAmount(1 + 2), Times.Once);
            this._sourceItem.Verify(x => x.SetAmount(0), Times.Once);
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
