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
    public class ItemSplitStrategyHandlerFixture
    {

        private ItemSplitStrategyHandler _strategyHandler;
        private List<Mock<IItemSplitStrategy>> _splitStrategies;

        private Mock<IItem> _oldItem;
        private Mock<IItem> _newItem;

        [TestInitialize]
        public void Setup()
        {
            _strategyHandler = new ItemSplitStrategyHandler();
            _splitStrategies = new List<Mock<IItemSplitStrategy>>
            {
                new Mock<IItemSplitStrategy>(),
                new Mock<IItemSplitStrategy>(),
                new Mock<IItemSplitStrategy>()
            };

            foreach (var splitStrategy in _splitStrategies)
            {
                _strategyHandler.Add(splitStrategy.Object);
            }

            _oldItem = new Mock<IItem>();
            _newItem = new Mock<IItem>();
        }

        [TestMethod]
        public async Task ExecutingSplittingStrategyWillCallEveryStrategy()
        {
            await _strategyHandler.SplitItemAsync(_oldItem.Object, _newItem.Object);

            foreach (var splitStrategy in _splitStrategies)
            {
                splitStrategy.Verify(x => x.SplitItemAsync(_oldItem.Object, _newItem.Object), Times.Once);
            }
        }

        [TestMethod]
        public async Task ExecutingStrategyWithNullOldItemWillThrowException()
        {
            Func<Task> act = () => _strategyHandler.SplitItemAsync(null, _newItem.Object);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task ExecutingStrategyWithNullNewItemWillThrowException()
        {
            Func<Task> act = () => _strategyHandler.SplitItemAsync(_oldItem.Object, null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
