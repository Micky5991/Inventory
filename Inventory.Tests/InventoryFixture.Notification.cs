using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    public partial class InventoryFixture
    {
        [TestMethod]
        public async Task AddingItemWillNotifyUsedCapacityAndAvailableCapacity()
        {
            using var monitoredInventory = _inventory.Monitor();

            await AddItemToInventoryAsync(10);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.UsedCapacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task RemovingItemWillNotifyUsedCapacityAndAvailableCapacity()
        {
            var item = await AddItemToInventoryAsync(10);

            using var monitoredInventory = _inventory.Monitor();

            await _inventory.RemoveItemAsync(item);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.UsedCapacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task ChangingCapacityOfInventoryWillNotify()
        {
            var item = await AddItemToInventoryAsync(10);

            using var monitoredInventory = _inventory.Monitor();

            _inventory.SetCapacity(20);

            monitoredInventory.Should().RaisePropertyChangeFor(x => x.Capacity);
            monitoredInventory.Should().RaisePropertyChangeFor(x => x.AvailableCapacity);
        }

        [TestMethod]
        public async Task SettingCapacityOfInventoryWontNotify()
        {
            await AddItemToInventoryAsync(10);

            using var monitoredInventory = _inventory.Monitor();

            _inventory.SetCapacity(_inventory.Capacity);

            monitoredInventory.Should().NotRaisePropertyChangeFor(x => x.Capacity);
            monitoredInventory.Should().NotRaisePropertyChangeFor(x => x.AvailableCapacity);
        }
    }
}
