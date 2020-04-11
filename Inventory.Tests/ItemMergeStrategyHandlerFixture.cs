using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;
using Micky5991.Inventory.Strategies.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ItemMergeStrategyHandlerFixture
    {
        private ItemMergeStrategyHandler _strategyHandler;
        private List<Mock<IItemMergeStrategy>> _mergeStrategies;

        private Mock<IItem> _sourceItem;
        private Mock<IItem> _targetItem;

        [TestInitialize]
        public void Setup()
        {
            this._strategyHandler = new ItemMergeStrategyHandler();
            this._mergeStrategies = new List<Mock<IItemMergeStrategy>>
            {
                new Mock<IItemMergeStrategy>(),
                new Mock<IItemMergeStrategy>(),
                new Mock<IItemMergeStrategy>()
            };

            foreach (var mergeStrategy in this._mergeStrategies)
            {
                this._strategyHandler.Add(mergeStrategy.Object);
            }

            this._sourceItem = new Mock<IItem>();
            this._targetItem = new Mock<IItem>();
        }

        [TestMethod]
        public void CanBeMergedWithTargetNullThrowsException()
        {
            Action act = () => this._strategyHandler.CanBeMerged(null, this._sourceItem.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CanBeMergedWithSourceNullThrowsException()
        {
            Action act = () => this._strategyHandler.CanBeMerged(this._targetItem.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void MergeWithAsyncWithTargetNullThrowsException()
        {
            Func<Task> act = () => this._strategyHandler.MergeItemWithAsync(null, this._sourceItem.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void MergeWithAsyncWithSourceNullThrowsException()
        {
            Func<Task> act = () => this._strategyHandler.MergeItemWithAsync(this._targetItem.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void HandlerImplementsStrategyHandlerBase()
        {
            this._strategyHandler.Should().BeAssignableTo<StrategyHandler<IItemMergeStrategy>>();
        }

        [TestMethod]
        public void CanBeMergedWillCallAllStrategies()
        {
            foreach (var strategy in this._mergeStrategies)
            {
                strategy
                    .Setup(x => x.CanBeMerged(this._targetItem.Object, this._sourceItem.Object))
                    .Returns(true);
            }

            this._strategyHandler.CanBeMerged(this._targetItem.Object, this._sourceItem.Object);

            foreach (var strategy in this._mergeStrategies)
            {
                strategy
                    .Verify(x => x.CanBeMerged(this._targetItem.Object, this._sourceItem.Object), Times.Once);
            }
        }

        [TestMethod]
        public void FirstUnmergableResultWillNotCallOthers()
        {
            const int firstFalseStrategy = 2;

            for (var i = 0; i < this._mergeStrategies.Count; i++)
            {
                this._mergeStrategies[i].Setup(x => x.CanBeMerged(this._targetItem.Object, this._sourceItem.Object))
                    .Returns(i < firstFalseStrategy);
            }

            this._strategyHandler.CanBeMerged(this._targetItem.Object, this._sourceItem.Object);

            for (var i = 0; i < this._mergeStrategies.Count; i++)
            {
                var times = Times.Once();

                if (i > firstFalseStrategy)
                {
                    times = Times.Never();
                }

                this._mergeStrategies[i]
                    .Verify(x => x.CanBeMerged(this._targetItem.Object, this._sourceItem.Object), times);
            }
        }

        [TestMethod]
        public async Task ExecutingMergeStrategyExecutesAllStrategies()
        {
            for (var i = 0; i < this._mergeStrategies.Count; i++)
            {
                this._mergeStrategies[i]
                    .Setup(x => x.MergeItemWithAsync(this._targetItem.Object, this._sourceItem.Object))
                    .Returns(Task.CompletedTask);
            }

            await this._strategyHandler.MergeItemWithAsync(this._targetItem.Object, this._sourceItem.Object);

            for (var i = 0; i < this._mergeStrategies.Count; i++)
            {
                this._mergeStrategies[i]
                    .Verify(x => x.MergeItemWithAsync(this._targetItem.Object, this._sourceItem.Object), Times.Once);
            }
        }
    }
}
