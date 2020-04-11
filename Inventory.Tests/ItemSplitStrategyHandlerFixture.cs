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

        private ItemSplitStrategyHandler strategyHandler;
        private List<Mock<IItemSplitStrategy>> splitStrategies;

        private Mock<IItem> oldItem;
        private Mock<IItem> newItem;

        [TestInitialize]
        public void Setup()
        {
            this.strategyHandler = new ItemSplitStrategyHandler();
            this.splitStrategies = new List<Mock<IItemSplitStrategy>>
            {
                new Mock<IItemSplitStrategy>(),
                new Mock<IItemSplitStrategy>(),
                new Mock<IItemSplitStrategy>()
            };

            foreach (var splitStrategy in this.splitStrategies)
            {
                this.strategyHandler.Add(splitStrategy.Object);
            }

            this.oldItem = new Mock<IItem>();
            this.newItem = new Mock<IItem>();
        }

        [TestMethod]
        public async Task ExecutingSplittingStrategyWillCallEveryStrategy()
        {
            await this.strategyHandler.SplitItemAsync(this.oldItem.Object, this.newItem.Object);

            foreach (var splitStrategy in this.splitStrategies)
            {
                splitStrategy.Verify(x => x.SplitItemAsync(this.oldItem.Object, this.newItem.Object), Times.Once);
            }
        }

        [TestMethod]
        public async Task ExecutingStrategyWithNullOldItemWillThrowException()
        {
            Func<Task> act = () => this.strategyHandler.SplitItemAsync(null, this.newItem.Object);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task ExecutingStrategyWithNullNewItemWillThrowException()
        {
            Func<Task> act = () => this.strategyHandler.SplitItemAsync(this.oldItem.Object, null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
