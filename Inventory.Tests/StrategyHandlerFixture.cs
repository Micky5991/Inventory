using System;
using FluentAssertions;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class StrategyHandlerFixture
    {

        private RealStrategyHandler _strategyHandler;

        [TestInitialize]
        public void Setup()
        {
            this._strategyHandler = new RealStrategyHandler();
        }

        [TestMethod]
        public void AddingNullToHandlerWillThrowException()
        {
            Action act = () => this._strategyHandler.Add(null);

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
