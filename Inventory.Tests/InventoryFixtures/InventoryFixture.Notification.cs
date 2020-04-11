using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests.InventoryFixtures
{
    public partial class InventoryFixture
    {
        [TestMethod]
        public async Task AddingItemWillNotifyUsedCapacityAndAvailableCapacity()
        {
            using var monitoredInventory = this.Inventory.Monitor();

            await this.AddItemToInventoryAsync(10);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.UsedCapacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task RemovingItemWillNotifyUsedCapacityAndAvailableCapacity()
        {
            var item = await this.AddItemToInventoryAsync(10);

            using var monitoredInventory = this.Inventory.Monitor();

            await this.Inventory.RemoveItemAsync(item);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.UsedCapacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task ChangingCapacityOfInventoryWillNotify()
        {
            var item = await this.AddItemToInventoryAsync(10);

            using var monitoredInventory = this.Inventory.Monitor();

            this.Inventory.SetCapacity(20);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.Capacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task SettingCapacityOfInventoryWontNotify()
        {
            await this.AddItemToInventoryAsync(10);

            using var monitoredInventory = this.Inventory.Monitor();

            this.Inventory.SetCapacity(this.Inventory.Capacity);

            monitoredInventory.Should().NotRaisePropertyChangeFor(x => x.Capacity);
            monitoredInventory.Should().NotRaisePropertyChangeFor(x => x.AvailableCapacity);
        }
    }
}
