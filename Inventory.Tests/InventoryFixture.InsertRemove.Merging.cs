using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Micky5991.Inventory.Tests.SimpleInstances;
using Micky5991.Inventory.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    public partial class InventoryFixture
    {

        [TestMethod]
        public async Task InsertingMergableItemChecksAllItemsForMergability()
        {
            var insertedItemA = new FakeItem(10, "item");
            var insertedItemB = new FakeItem(10, "item");

            var mergableCheckAmount = new [] {0, 0};

            insertedItemA.IsMergableCheck = x =>
            {
                mergableCheckAmount[0]++;

                return false;
            };

            insertedItemB.IsMergableCheck = x =>
            {
                mergableCheckAmount[1]++;

                return false;
            };

            await _inventory.InsertItemAsync(insertedItemA);
            await _inventory.InsertItemAsync(insertedItemB);

            mergableCheckAmount[0] = 0;
            mergableCheckAmount[1] = 0;

            await _inventory.InsertItemAsync(new FakeItem(5));

            mergableCheckAmount[0].Should().Be(1);
            mergableCheckAmount[1].Should().Be(1);
        }

        [TestMethod]
        public async Task InsertingMergableItemMergesOneItemWithMergability()
        {
            var insertedItemA = CreateMockItem();
            var insertedItemB = CreateMockItem();

            var additionalItem = CreateMockItem();

            insertedItemA.Setup(x => x.CanMergeWith(additionalItem.Object)).Returns(true);
            insertedItemB.Setup(x => x.CanMergeWith(additionalItem.Object)).Returns(false);

            await _inventory.InsertItemAsync(insertedItemA.Object);
            await _inventory.InsertItemAsync(insertedItemB.Object);

            await _inventory.InsertItemAsync(additionalItem.Object);

            insertedItemA.Verify(x => x.MergeItemAsync(additionalItem.Object), Times.Once);
        }

        [TestMethod]
        public async Task AddingMergableItemToInventoryMergesWithOtherMergableItems()
        {
            const string appleHandle = "apple";
            const string waterHandle = "water";

            var registry = new SimpleItemRegistry();

            registry.TryGetItemMeta(appleHandle, out var appleMeta);
            registry.TryGetItemMeta(waterHandle, out var waterMeta);

            var apple = new AppleItem(appleMeta);
            var water = new WaterItem(waterMeta);

            apple.SetAmount(3);
            water.SetAmount(2);

            await _inventory.InsertItemAsync(apple);
            await _inventory.InsertItemAsync(water);

            var additionalApple = new AppleItem(appleMeta);
            additionalApple.SetAmount(2);

            var result = await _inventory.InsertItemAsync(additionalApple);

            result.Should().BeTrue();
            _inventory.Items.Should().HaveCount(2);

            var foundApple = _inventory.Items.First(x => x.Handle == appleHandle);
            var foundWater = _inventory.Items.First(x => x.Handle == waterHandle);

            foundApple.Should().Be(apple);
            foundApple.Amount.Should().Be(5);

            foundWater.Should().Be(water);
            foundWater.Amount.Should().Be(2);
        }

        private Mock<IItem> CreateMockItem(string handle = "item", int weight = 1)
        {
            var mock = new Mock<IItem>();

            mock.SetupGet(x => x.RuntimeId).Returns(Guid.NewGuid());

            mock.SetupGet(x => x.Meta)
                .Returns(InventoryUtils.CreateItemMeta(handle, typeof(FakeItem), "Item", weight));

            return mock;
        }
    }
}
