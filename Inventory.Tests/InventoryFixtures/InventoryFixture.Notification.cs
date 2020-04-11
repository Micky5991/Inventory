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
            using var monitoredInventory = this._inventory.Monitor();

            await this.AddItemToInventoryAsync(10);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.UsedCapacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task RemovingItemWillNotifyUsedCapacityAndAvailableCapacity()
        {
            var item = await this.AddItemToInventoryAsync(10);

            using var monitoredInventory = this._inventory.Monitor();

            await this._inventory.RemoveItemAsync(item);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.UsedCapacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task ChangingCapacityOfInventoryWillNotify()
        {
            var item = await this.AddItemToInventoryAsync(10);

            using var monitoredInventory = this._inventory.Monitor();

            this._inventory.SetCapacity(20);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.Capacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task SettingCapacityOfInventoryWontNotify()
        {
            await this.AddItemToInventoryAsync(10);

            using var monitoredInventory = this._inventory.Monitor();

            this._inventory.SetCapacity(this._inventory.Capacity);

            monitoredInventory.Should().NotRaisePropertyChangeFor(x => x.Capacity);
            monitoredInventory.Should().NotRaisePropertyChangeFor(x => x.AvailableCapacity);
        }
    }
}
