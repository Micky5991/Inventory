using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests.InventoryFixtures
{
    public partial class InventoryFixture
    {
        [TestMethod]
        public void AddingItemWillNotifyUsedCapacityAndAvailableCapacity()
        {
            using var monitoredInventory = this.Inventory.Monitor();

            this.AddItemToInventory(10);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.UsedCapacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public void RemovingItemWillNotifyUsedCapacityAndAvailableCapacity()
        {
            var item = this.AddItemToInventory(10);

            using var monitoredInventory = this.Inventory.Monitor();

            this.Inventory.RemoveItem(item);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.UsedCapacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public void ChangingCapacityOfInventoryWillNotify()
        {
            var item = this.AddItemToInventory(10);

            using var monitoredInventory = this.Inventory.Monitor();

            this.Inventory.SetCapacity(20);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.Capacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public void SettingCapacityOfInventoryWontNotify()
        {
            this.AddItemToInventory(10);

            using var monitoredInventory = this.Inventory.Monitor();

            this.Inventory.SetCapacity(this.Inventory.Capacity);

            monitoredInventory.Should().NotRaisePropertyChangeFor(x => x.Capacity);
            monitoredInventory.Should().NotRaisePropertyChangeFor(x => x.AvailableCapacity);
        }
    }
}
