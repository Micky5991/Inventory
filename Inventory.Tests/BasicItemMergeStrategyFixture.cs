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

        private BasicItemMergeStrategy strategy;

        private Mock<IItem> targetItem;
        private Mock<IItem> sourceItem;

        [TestInitialize]
        public void Setup()
        {
            this.strategy = new BasicItemMergeStrategy();

            this.targetItem = new Mock<IItem>();
            this.sourceItem = new Mock<IItem>();

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
            Action act = () => this.strategy.CanBeMerged(null, this.sourceItem.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void PassingNullAsSourceInMergableCheckWillThrowArgumentNullException()
        {
            Action act = () => this.strategy.CanBeMerged(this.targetItem.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public async Task PassingNullAsTargetInMergingWillThrowArgumentNullException()
        {
            Func<Task> act = () => this.strategy.MergeItemWithAsync(null, this.sourceItem.Object);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task PassingNullAsSourceInMergingWillThrowArgumentNullException()
        {
            Func<Task> act = () => this.strategy.MergeItemWithAsync(this.targetItem.Object, null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void TwoNonStackableItemAreNotMergable(bool targetStackable, bool sourceStackable)
        {
            this.SetupMock(this.targetItem, targetStackable, "item", 1, 1);
            this.SetupMock(this.sourceItem, sourceStackable, "item", 1, 1);

            this.strategy.CanBeMerged(this.targetItem.Object, this.sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void TwoIdenticalStackableItemAreMergable()
        {
            this.SetupMock(this.targetItem, true, "item", 1, 1);
            this.SetupMock(this.sourceItem, true, "item", 1, 1);

            this.strategy.CanBeMerged(this.targetItem.Object, this.sourceItem.Object).Should().BeTrue();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void SourceItemWithZeroAmountIsNotMergable(int sourceAmount)
        {
            this.SetupMock(this.targetItem, true, "item", 1, 1);
            this.SetupMock(this.sourceItem, true, "item", sourceAmount, 1);

            this.strategy.CanBeMerged(this.targetItem.Object, this.sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void StackableItemsWithDifferentHandlesAreNotMergable()
        {
            this.SetupMock(this.targetItem, true, "item1", 1, 1);
            this.SetupMock(this.sourceItem, true, "item2", 1, 1);

            this.strategy.CanBeMerged(this.targetItem.Object, this.sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void DifferentSingleWeightWillPreventMerging()
        {
            this.SetupMock(this.targetItem, true, "item", 1, 1);
            this.SetupMock(this.sourceItem, true, "item", 1, 2);

            this.strategy.CanBeMerged(this.targetItem.Object, this.sourceItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public void SameItemIsNotMergable()
        {
            this.SetupMock(this.targetItem, true, "item", 1, 1);

            this.strategy.CanBeMerged(this.targetItem.Object, this.targetItem.Object).Should().BeFalse();
        }

        [TestMethod]
        public async Task MergingItemExecutesAmountSumAndAmountClearOnSource()
        {
            this.targetItem.SetupGet(x => x.Amount).Returns(1);
            this.sourceItem.SetupGet(x => x.Amount).Returns(2);

            this.targetItem.Setup(x => x.SetAmount(1 + 2));
            this.sourceItem.Setup(x => x.SetAmount(0));

            await this.strategy.MergeItemWithAsync(this.targetItem.Object, this.sourceItem.Object);

            this.targetItem.VerifyGet(x => x.Amount, Times.Once);
            this.sourceItem.VerifyGet(x => x.Amount, Times.Once);

            this.targetItem.Verify(x => x.SetAmount(1 + 2), Times.Once);
            this.sourceItem.Verify(x => x.SetAmount(0), Times.Once);
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
