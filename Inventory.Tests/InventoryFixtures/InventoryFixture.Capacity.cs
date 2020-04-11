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
            Action act = () => this.Inventory.SetCapacity(Entities.Inventory.Inventory.MinimalInventoryCapacity - 1);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains($"{Entities.Inventory.Inventory.MinimalInventoryCapacity} or higher"));
        }

        [TestMethod]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity)]
        [DataRow(Entities.Inventory.Inventory.MinimalInventoryCapacity + 1)]
        [DataRow(int.MaxValue)]
        public void ChangingCapacityWillChangeCapacityCorrectly(int capacity)
        {
            this.Inventory.SetCapacity(capacity);

            this.AssertInventoryCapacity(0, capacity);
        }

        [TestMethod]
        public async Task SettingCapacityBelowUsedCapacityWillReturnFalse()
        {
            await this.AddItemToInventoryAsync(50);

            this.Inventory.SetCapacity(51).Should().BeTrue();
            this.Inventory.SetCapacity(50).Should().BeTrue();
            this.Inventory.SetCapacity(49).Should().BeFalse();
        }

        [TestMethod]
        public void ChangingCapacityWillAlterAvailableCapacity()
        {
            var oldCapacity = this.Inventory.Capacity;
            var newCapacity = this.Inventory.Capacity - 10;

            this.Inventory.SetCapacity(newCapacity);

            this.Inventory.AvailableCapacity
                .Should().Be(newCapacity)
                .And.BeLessThan(oldCapacity);
        }

        [TestMethod]
        public async Task ChangingCapacityBelowUsedCapacityWillKeepValueSame()
        {
            await this.AddItemToInventoryAsync(50);

            var oldCapacity = this.Inventory.Capacity;

            this.Inventory.SetCapacity(30).Should().BeFalse();
            this.Inventory.Capacity.Should().Be(oldCapacity);
        }

        [TestMethod]
        public async Task ChangingCapacityWillKeepUsedCapacitySame()
        {
            await this.AddItemToInventoryAsync(50);

            var oldUsedCapacity = this.Inventory.UsedCapacity;

            this.Inventory.SetCapacity(60);

            this.Inventory.UsedCapacity.Should().Be(oldUsedCapacity);
        }

        [TestMethod]
        public async Task ChangingAmountOfItemChangesUsedCapacityOfInventory()
        {
            var item = this.ItemFactory.CreateItem(this.RealMeta, 1);

            await this.Inventory.InsertItemAsync(item);

            this.Inventory.UsedCapacity.Should().Be(this.RealMeta.DefaultWeight);

            item.SetAmount(2);

            this.Inventory.UsedCapacity.Should().Be(2 * this.RealMeta.DefaultWeight);
        }
    }
}
