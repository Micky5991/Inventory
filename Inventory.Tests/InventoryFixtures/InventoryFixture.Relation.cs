using FluentAssertions;
using Micky5991.Inventory.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests.InventoryFixtures
{
    public partial class InventoryFixture
    {
        [TestMethod]
        public void AddingItemWillSetCurrentInventoryCorrectly()
        {
            this.itemMock.Setup(x => x.SetCurrentInventory(this.Inventory));

            this.Inventory.InsertItem(this.itemMock.Object);

            this.itemMock.Verify(x => x.SetCurrentInventory(this.Inventory));
        }

        [TestMethod]
        public void RemovingItemWillSetCurrentInventoryToNull()
        {
            this.itemMock.Setup(x => x.SetCurrentInventory(It.IsAny<IInventory>()));

            this.Inventory.InsertItem(this.itemMock.Object);
            this.Inventory.RemoveItem(this.itemMock.Object);

            this.itemMock.Verify(x => x.SetCurrentInventory(null), Times.Once);
        }

        [TestMethod]
        public void InsertingItemIntoInventoryWillRemoveItemFromOldInventoryFirst()
        {
            var otherInventoryMock = new Mock<IInventory>();

            this.itemMock
                .SetupGet(x => x.CurrentInventory)
                .Returns(otherInventoryMock.Object);

            this.itemMock
                .Setup(x => x.SetCurrentInventory(this.Inventory));

            otherInventoryMock
                .Setup(x => x.RemoveItem(this.itemMock.Object))
                .Returns(true);

            this.Inventory.InsertItem(this.itemMock.Object);

            this.itemMock.VerifyGet(x => x.CurrentInventory, Times.AtLeastOnce);
            otherInventoryMock.Verify(x => x.RemoveItem(this.itemMock.Object), Times.Once);

            this.itemMock.Verify(x => x.SetCurrentInventory(this.Inventory), Times.Once);
        }

        [TestMethod]
        public void FailingRemovingOldInventoryWillResultInFailedInsertion()
        {
            var otherInventoryMock = new Mock<IInventory>();

            this.itemMock
                .SetupGet(x => x.CurrentInventory)
                .Returns(otherInventoryMock.Object);

            otherInventoryMock
                .Setup(x => x.RemoveItem(this.itemMock.Object))
                .Returns(false);

            var success = this.Inventory.InsertItem(this.itemMock.Object);

            success.Should().BeFalse();

            this.itemMock.Verify(x => x.SetCurrentInventory(It.IsAny<IInventory>()), Times.Never);
        }

        [TestMethod]
        public void SettingItemAmountToZeroRemovesItem()
        {
            this.Inventory.InsertItem(this.Item);

            this.Item.SetAmount(0);

            this.Inventory.Items.Should().BeEmpty();
        }

        [TestMethod]
        public void ReducingAmountToZeroRemovesItem()
        {
            this.Inventory.InsertItem(this.Item);

            this.Item.ReduceAmount(this.Item.Amount);

            this.Inventory.Items.Should().BeEmpty();
        }
    }
}
