using System;
using System.Collections.Generic;
using FluentAssertions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;
using Micky5991.Inventory.Strategies;
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
        public void ExecutingSplittingStrategyWillCallEveryStrategy()
        {
            this.strategyHandler.SplitItem(this.oldItem.Object, this.newItem.Object);

            foreach (var splitStrategy in this.splitStrategies)
            {
                splitStrategy.Verify(x => x.SplitItem(this.oldItem.Object, this.newItem.Object), Times.Once);
            }
        }

        [TestMethod]
        public void ExecutingStrategyWithNullOldItemWillThrowException()
        {
            Action act = () => this.strategyHandler.SplitItem(null, this.newItem.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ExecutingStrategyWithNullNewItemWillThrowException()
        {
            Action act = () => this.strategyHandler.SplitItem(this.oldItem.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
