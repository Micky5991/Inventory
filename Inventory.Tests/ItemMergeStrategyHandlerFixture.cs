using System;
using System.Collections.Generic;
using FluentAssertions;
using Micky5991.Inventory.Entities.Strategies;
using Micky5991.Inventory.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ItemMergeStrategyHandlerFixture
    {
        private ItemMergeStrategyHandler strategyHandler;
        private List<Mock<IItemMergeStrategy>> mergeStrategies;

        private Mock<IItem> sourceItem;
        private Mock<IItem> targetItem;

        [TestInitialize]
        public void Setup()
        {
            this.strategyHandler = new ItemMergeStrategyHandler();
            this.mergeStrategies = new List<Mock<IItemMergeStrategy>>
            {
                new Mock<IItemMergeStrategy>(),
                new Mock<IItemMergeStrategy>(),
                new Mock<IItemMergeStrategy>()
            };

            foreach (var mergeStrategy in this.mergeStrategies)
            {
                this.strategyHandler.Add(mergeStrategy.Object);
            }

            this.sourceItem = new Mock<IItem>();
            this.targetItem = new Mock<IItem>();
        }

        [TestMethod]
        public void CanBeMergedWithTargetNullThrowsException()
        {
            Action act = () => this.strategyHandler.CanBeMerged(null, this.sourceItem.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CanBeMergedWithSourceNullThrowsException()
        {
            Action act = () => this.strategyHandler.CanBeMerged(this.targetItem.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void MergeWithAsyncWithTargetNullThrowsException()
        {
            Action act = () => this.strategyHandler.MergeItemWith(null, this.sourceItem.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void MergeWithAsyncWithSourceNullThrowsException()
        {
            Action act = () => this.strategyHandler.MergeItemWith(this.targetItem.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void HandlerImplementsStrategyHandlerBase()
        {
            this.strategyHandler.Should().BeAssignableTo<StrategyHandler<IItemMergeStrategy>>();
        }

        [TestMethod]
        public void CanBeMergedWillCallAllStrategies()
        {
            foreach (var strategy in this.mergeStrategies)
            {
                strategy
                    .Setup(x => x.CanBeMerged(this.targetItem.Object, this.sourceItem.Object))
                    .Returns(true);
            }

            this.strategyHandler.CanBeMerged(this.targetItem.Object, this.sourceItem.Object);

            foreach (var strategy in this.mergeStrategies)
            {
                strategy
                    .Verify(x => x.CanBeMerged(this.targetItem.Object, this.sourceItem.Object), Times.Once);
            }
        }

        [TestMethod]
        public void FirstUnmergableResultWillNotCallOthers()
        {
            const int firstFalseStrategy = 2;

            for (var i = 0; i < this.mergeStrategies.Count; i++)
            {
                this.mergeStrategies[i].Setup(x => x.CanBeMerged(this.targetItem.Object, this.sourceItem.Object))
                    .Returns(i < firstFalseStrategy);
            }

            this.strategyHandler.CanBeMerged(this.targetItem.Object, this.sourceItem.Object);

            for (var i = 0; i < this.mergeStrategies.Count; i++)
            {
                var times = Times.Once();

                if (i > firstFalseStrategy)
                {
                    times = Times.Never();
                }

                this.mergeStrategies[i]
                    .Verify(x => x.CanBeMerged(this.targetItem.Object, this.sourceItem.Object), times);
            }
        }

        [TestMethod]
        public void ExecutingMergeStrategyExecutesAllStrategies()
        {
            for (var i = 0; i < this.mergeStrategies.Count; i++)
            {
                this.mergeStrategies[i]
                    .Setup(x => x.MergeItemWith(this.targetItem.Object, this.sourceItem.Object));
            }

            this.strategyHandler.MergeItemWith(this.targetItem.Object, this.sourceItem.Object);

            for (var i = 0; i < this.mergeStrategies.Count; i++)
            {
                this.mergeStrategies[i]
                    .Verify(x => x.MergeItemWith(this.targetItem.Object, this.sourceItem.Object), Times.Once);
            }
        }
    }
}
