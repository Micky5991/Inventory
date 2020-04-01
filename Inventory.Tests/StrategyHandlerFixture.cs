using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Strategies;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class StrategyHandlerFixture
    {

        private RealStrategyHandler _strategyHandler;

        [TestInitialize]
        public void Setup()
        {
            _strategyHandler = new RealStrategyHandler();
        }

        [TestMethod]
        public void AddingNullToStrategyHandlerThrowsException()
        {
            Action act = () => _strategyHandler.AddStrategy(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RemovingNullFromStrategyHandlerThrowsException()
        {
            Action act = () => _strategyHandler.RemoveStrategy(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void AddingStrategyAddsToCollection()
        {
            var strategy = new BasicItemMergeStrategy();
            _strategyHandler.AddStrategy(strategy);

            _strategyHandler.GetAllStrategies().Should().ContainSingle(x => x == strategy);
        }

        [TestMethod]
        public void RemovingStrategyRemovesFromCollection()
        {
            var strategy = new BasicItemMergeStrategy();
            _strategyHandler.AddStrategy(strategy);
            _strategyHandler.GetAllStrategies().Should().ContainSingle(x => x == strategy);

            _strategyHandler.RemoveStrategy(strategy);

            _strategyHandler.GetAllStrategies().Should().BeEmpty();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void AddingDifferentAmountsOfStrategiesAreRepresentedInGetter(int strategyAmount)
        {
            var strategies = new List<IItemMergeStrategy>();

            for (int i = 0; i < strategyAmount; i++)
            {
                var strategy = new BasicItemMergeStrategy();

                strategies.Add(strategy);

                _strategyHandler.AddStrategy(strategy);
            }

            _strategyHandler.GetAllStrategies().Should()
                .HaveCount(strategyAmount);

            if (strategyAmount > 0)
            {
                _strategyHandler.GetAllStrategies().Should()
                    .OnlyContain(x => strategies.Contains(x));
            }
        }

        [TestMethod]
        public void StrategiesAreOrderedByAddingOrder()
        {
            var strategies = new List<IItemMergeStrategy>
            {
                new BasicItemMergeStrategy(),
                new BasicItemMergeStrategy(),
                new BasicItemMergeStrategy(),
            };

            foreach (var strategy in strategies)
            {
                _strategyHandler.AddStrategy(strategy);
            }

            _strategyHandler.GetAllStrategies().Should().ContainInOrder(strategies);
        }

        [TestMethod]
        public void ClearingStrategyHandlerRemovesAllElements()
        {
            var strategies = new List<IItemMergeStrategy>
            {
                new BasicItemMergeStrategy(),
                new BasicItemMergeStrategy(),
                new BasicItemMergeStrategy(),
            };

            foreach (var strategy in strategies)
            {
                _strategyHandler.AddStrategy(strategy);
            }

            _strategyHandler.GetAllStrategies().Should().HaveCount(strategies.Count);

            _strategyHandler.ClearStrategies();

            _strategyHandler.GetAllStrategies().Should().BeEmpty();
        }
    }
}
