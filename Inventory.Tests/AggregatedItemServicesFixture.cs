using FluentAssertions;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class AggregatedItemServicesFixture
    {

        [TestMethod]
        public void CreatingAggregatedInventoryServiceSetsRightValues()
        {
            var itemMergeHandler = new Mock<IItemMergeStrategyHandler>();
            var itemSplitHandler = new Mock<IItemSplitStrategyHandler>();
            var itemFactory = new Mock<IItemFactory>();

            var service = new AggregatedItemServices(itemMergeHandler.Object, itemSplitHandler.Object, itemFactory.Object);

            Assert.AreEqual(itemMergeHandler.Object, service.ItemMergeStrategyHandler);
            Assert.AreEqual(itemSplitHandler.Object, service.ItemSplitStrategyHandler);
            service.ItemFactory.Should().Be(itemFactory.Object);
        }

        [TestMethod]
        public void CreatingAggregatedInventoryServiceSetsRightNullValues()
        {
            var service = new AggregatedItemServices(null, null, null);

            service.ItemMergeStrategyHandler.Should().BeNull();
            service.ItemSplitStrategyHandler.Should().BeNull();

            service.ItemFactory.Should().BeNull();
        }

    }
}
