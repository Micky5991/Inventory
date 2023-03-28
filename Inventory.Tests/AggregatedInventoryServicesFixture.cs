using FluentAssertions;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class AggregatedInventoryServicesFixture
    {

        [TestMethod]
        public void CreatingAggregatedInventoryServiceSetsRightValues()
        {
            var itemRegistry = new Mock<IItemRegistry>();

            var service = new AggregatedInventoryServices(itemRegistry.Object);

            service.ItemRegistry.Should().Be(itemRegistry.Object);
        }

        [TestMethod]
        public void CreatingAggregatedInventoryServiceSetsRightNullValues()
        {
            var service = new AggregatedInventoryServices(null!);

            service.ItemRegistry.Should().BeNull();
        }

    }
}
