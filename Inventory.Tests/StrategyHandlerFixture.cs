using System;
using FluentAssertions;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class StrategyHandlerFixture
    {

        private RealStrategyHandler strategyHandler;

        [TestInitialize]
        public void Setup()
        {
            this.strategyHandler = new RealStrategyHandler();
        }

        [TestMethod]
        public void AddingNullToHandlerWillThrowException()
        {
            Action act = () => this.strategyHandler.Add(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void InitializingStrategyHandlerWithNullThrowsException()
        {
            Action act = () =>
            {
                new RealStrategyHandler
                {
                    null
                };
            };

            act.Should().Throw<ArgumentNullException>();
        }


    }
}
