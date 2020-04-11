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
            this._strategyHandler = new ItemSplitStrategyHandler();
            this._splitStrategies = new List<Mock<IItemSplitStrategy>>
            {
                new Mock<IItemSplitStrategy>(),
                new Mock<IItemSplitStrategy>(),
                new Mock<IItemSplitStrategy>()
            };

            foreach (var splitStrategy in this._splitStrategies)
            {
                this._strategyHandler.Add(splitStrategy.Object);
            }

            this._oldItem = new Mock<IItem>();
            this._newItem = new Mock<IItem>();
        }

        [TestMethod]
        public async Task ExecutingSplittingStrategyWillCallEveryStrategy()
        {
            await this._strategyHandler.SplitItemAsync(this._oldItem.Object, this._newItem.Object);

            foreach (var splitStrategy in this._splitStrategies)
            {
                splitStrategy.Verify(x => x.SplitItemAsync(this._oldItem.Object, this._newItem.Object), Times.Once);
            }
        }

        [TestMethod]
        public async Task ExecutingStrategyWithNullOldItemWillThrowException()
        {
            Func<Task> act = () => this._strategyHandler.SplitItemAsync(null, this._newItem.Object);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task ExecutingStrategyWithNullNewItemWillThrowException()
        {
            Func<Task> act = () => this._strategyHandler.SplitItemAsync(this._oldItem.Object, null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
