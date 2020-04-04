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
            _strategyHandler = new ItemMergeStrategyHandler();
            _mergeStrategies = new List<Mock<IItemMergeStrategy>>
            {
                new Mock<IItemMergeStrategy>(),
                new Mock<IItemMergeStrategy>(),
                new Mock<IItemMergeStrategy>()
            };

            foreach (var mergeStrategy in _mergeStrategies)
            {
                _strategyHandler.Add(mergeStrategy.Object);
            }

            _sourceItem = new Mock<IItem>();
            _targetItem = new Mock<IItem>();
        }

        [TestMethod]
        public void CanBeMergedWithTargetNullThrowsException()
        {
            Action act = () => _strategyHandler.CanBeMerged(null, _sourceItem.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CanBeMergedWithSourceNullThrowsException()
        {
            Action act = () => _strategyHandler.CanBeMerged(_targetItem.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void MergeWithAsyncWithTargetNullThrowsException()
        {
            Func<Task> act = () => _strategyHandler.MergeItemWithAsync(null, _sourceItem.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void MergeWithAsyncWithSourceNullThrowsException()
        {
            Func<Task> act = () => _strategyHandler.MergeItemWithAsync(_targetItem.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void HandlerImplementsStrategyHandlerBase()
        {
            _strategyHandler.Should().BeAssignableTo<StrategyHandler<IItemMergeStrategy>>();
        }

        [TestMethod]
        public void CanBeMergedWillCallAllStrategies()
        {
            foreach (var strategy in _mergeStrategies)
            {
                strategy
                    .Setup(x => x.CanBeMerged(_targetItem.Object, _sourceItem.Object))
                    .Returns(true);
            }

            _strategyHandler.CanBeMerged(_targetItem.Object, _sourceItem.Object);

            foreach (var strategy in _mergeStrategies)
            {
                strategy
                    .Verify(x => x.CanBeMerged(_targetItem.Object, _sourceItem.Object), Times.Once);
            }
        }

        [TestMethod]
        public void FirstUnmergableResultWillNotCallOthers()
        {
            const int firstFalseStrategy = 2;

            for (var i = 0; i < _mergeStrategies.Count; i++)
            {
                _mergeStrategies[i].Setup(x => x.CanBeMerged(_targetItem.Object, _sourceItem.Object))
                    .Returns(i < firstFalseStrategy);
            }

            _strategyHandler.CanBeMerged(_targetItem.Object, _sourceItem.Object);

            for (var i = 0; i < _mergeStrategies.Count; i++)
            {
                var times = Times.Once();

                if (i > firstFalseStrategy)
                {
                    times = Times.Never();
                }

                _mergeStrategies[i]
                    .Verify(x => x.CanBeMerged(_targetItem.Object, _sourceItem.Object), times);
            }
        }

        [TestMethod]
        public async Task ExecutingMergeStrategyExecutesAllStrategies()
        {
            for (var i = 0; i < _mergeStrategies.Count; i++)
            {
                _mergeStrategies[i]
                    .Setup(x => x.MergeItemWithAsync(_targetItem.Object, _sourceItem.Object))
                    .Returns(Task.CompletedTask);
            }

            await _strategyHandler.MergeItemWithAsync(_targetItem.Object, _sourceItem.Object);

            for (var i = 0; i < _mergeStrategies.Count; i++)
            {
                _mergeStrategies[i]
                    .Verify(x => x.MergeItemWithAsync(_targetItem.Object, _sourceItem.Object), Times.Once);
            }
        }
    }
}
