using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests.InventoryFixtures
{
    public partial class InventoryFixture
    {
        [TestMethod]
        public void ChangingCapacityBelowMinimalCapacityWillThrowException()
        {
            Action act = () => this._inventory.SetCapacity(Entities.Inventory.Inventory.MinimalInventoryCapacity - 1);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains($"{Entities.Inventory.Inventory.MinimalInventoryCapacity} or higher"));
        }

        [TestMethod]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity + 1)]
        [DataRow(int.MaxValue)]
        public void ChangingCapacityWillChangeCapacityCorrectly(int capacity)
        {
            this._inventory.SetCapacity(capacity);

            this.AssertInventoryCapacity(0, capacity);
        }

        [TestMethod]
        public async Task SettingCapacityBelowUsedCapacityWillReturnFalse()
        {
            await this.AddItemToInventoryAsync(50);

            this._inventory.SetCapacity(51).Should().BeTrue();
            this._inventory.SetCapacity(50).Should().BeTrue();
            this._inventory.SetCapacity(49).Should().BeFalse();
        }

        [TestMethod]
        public void ChangingCapacityWillAlterAvailableCapacity()
        {
            var oldCapacity = this._inventory.Capacity;
            var newCapacity = this._inventory.Capacity - 10;

            this._inventory.SetCapacity(newCapacity);

            this._inventory.AvailableCapacity
                .Should().Be(newCapacity)
                .And.BeLessThan(oldCapacity);
        }

        [TestMethod]
        public async Task ChangingCapacityBelowUsedCapacityWillKeepValueSame()
        {
            await this.AddItemToInventoryAsync(50);

            var oldCapacity = this._inventory.Capacity;

            this._inventory.SetCapacity(30).Should().BeFalse();
            this._inventory.Capacity.Should().Be(oldCapacity);
        }

        [TestMethod]
        public async Task ChangingCapacityWillKeepUsedCapacitySame()
        {
            await this.AddItemToInventoryAsync(50);

            var oldUsedCapacity = this._inventory.UsedCapacity;

            this._inventory.SetCapacity(60);

            this._inventory.UsedCapacity.Should().Be(oldUsedCapacity);
        }

        [TestMethod]
        public async Task ChangingAmountOfItemChangesUsedCapacityOfInventory()
        {
            var item = this._itemFactory.CreateItem(this._realMeta, 1);

            await this._inventory.InsertItemAsync(item);

            this._inventory.UsedCapacity.Should().Be(this._realMeta.DefaultWeight);

            item.SetAmount(2);

            this._inventory.UsedCapacity.Should().Be(2 * this._realMeta.DefaultWeight);
        }
    }
}
