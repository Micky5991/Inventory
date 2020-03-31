using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    public partial class InventoryFixture
    {

        [TestMethod]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity - 1)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity - 2)]
        [DataRow(int.MinValue)]
        public void CreatingInventoryWithInvalidCapacityThrowsException(int capacity)
        {
            Action act = () => new Entities.Inventory.Inventory(capacity);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains($"{Entities.Inventory.Inventory.MinimalInventoryCapacity} or higher"));
        }

        [TestMethod]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity + 1)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity + 2)]
        [DataRow(int.MaxValue)]
        public void CreatingInventoryWithCapacityWillSetCapacityValues(int capacity)
        {
            var inventory = new Entities.Inventory.Inventory(capacity);

            AssertInventoryCapacity(0, capacity, inventory);
        }

        [TestMethod]
        public void ChangingCapacityBelowMinimalCapacityWillThrowException()
        {
            Action act = () => _inventory.SetCapacity(Entities.Inventory.Inventory.MinimalInventoryCapacity - 1);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains($"{Entities.Inventory.Inventory.MinimalInventoryCapacity} or higher"));
        }

        [TestMethod]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity + 1)]
        [DataRow(int.MaxValue)]
        public void ChangingCapacityWillChangeCapacityCorrectly(int capacity)
        {
            _inventory.SetCapacity(capacity);

            AssertInventoryCapacity(0, capacity);
        }

        [TestMethod]
        public async Task SettingCapacityBelowUsedCapacityWillReturnFalse()
        {
            await AddItemToInventoryAsync(50);

            _inventory.SetCapacity(51).Should().BeTrue();
            _inventory.SetCapacity(50).Should().BeTrue();
            _inventory.SetCapacity(49).Should().BeFalse();
        }

        [TestMethod]
        public void ChangingCapacityWillAlterAvailableCapacity()
        {
            var oldCapacity = _inventory.Capacity;
            var newCapacity = _inventory.Capacity - 10;

            _inventory.SetCapacity(newCapacity);

            _inventory.AvailableCapacity
                .Should().Be(newCapacity)
                .And.BeLessThan(oldCapacity);
        }

        [TestMethod]
        public async Task ChangingCapacityBelowUsedCapacityWillKeepValueSame()
        {
            await AddItemToInventoryAsync(50);

            var oldCapacity = _inventory.Capacity;

            _inventory.SetCapacity(30).Should().BeFalse();
            _inventory.Capacity.Should().Be(oldCapacity);
        }

        [TestMethod]
        public async Task ChangingCapacityWillKeepUsedCapacitySame()
        {
            await AddItemToInventoryAsync(50);

            var oldUsedCapacity = _inventory.UsedCapacity;

            _inventory.SetCapacity(60);

            _inventory.UsedCapacity.Should().Be(oldUsedCapacity);
        }

    }
}
