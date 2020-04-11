using System;
using FluentAssertions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Micky5991.Inventory.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests.InventoryFixtures
{
    public partial class InventoryFixture
    {

        [TestMethod]
        public void InsertingMergableItemChecksAllItemsForMergability()
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

            this.Inventory.InsertItem(insertedItemA);
            this.Inventory.InsertItem(insertedItemB);

            mergableCheckAmount[0] = 0;
            mergableCheckAmount[1] = 0;

            this.Inventory.InsertItem(new FakeItem(5));

            mergableCheckAmount[0].Should().Be(1);
            mergableCheckAmount[1].Should().Be(1);
        }

        [TestMethod]
        public void InsertingMergableItemMergesOneItemWithMergability()
        {
            var insertedItemA = this.CreateMockItem();
            var insertedItemB = this.CreateMockItem();

            var additionalItem = this.CreateMockItem();

            insertedItemA.Setup(x => x.CanMergeWith(additionalItem.Object)).Returns(true);
            insertedItemB.Setup(x => x.CanMergeWith(additionalItem.Object)).Returns(false);

            this.Inventory.InsertItem(insertedItemA.Object);
            this.Inventory.InsertItem(insertedItemB.Object);

            var result = this.Inventory.InsertItem(additionalItem.Object);

            result.Should().BeTrue();
            insertedItemA.Verify(x => x.MergeItem(additionalItem.Object), Times.Once);
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
