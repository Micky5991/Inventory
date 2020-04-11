using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests.InventoryFixtures
{
    public partial class InventoryFixture
    {
        [TestMethod]
        public async Task AddingItemWillSetCurrentInventoryCorrectly()
        {
            this.itemMock.Setup(x => x.SetCurrentInventory(this.Inventory));

            await this.Inventory.InsertItemAsync(this.itemMock.Object);

            this.itemMock.Verify(x => x.SetCurrentInventory(this.Inventory));
        }

        [TestMethod]
        public async Task RemovingItemWillSetCurrentInventoryToNull()
        {
            this.itemMock.Setup(x => x.SetCurrentInventory(It.IsAny<IInventory>()));

            await this.Inventory.InsertItemAsync(this.itemMock.Object);
            await this.Inventory.RemoveItemAsync(this.itemMock.Object);

            this.itemMock.Verify(x => x.SetCurrentInventory(null), Times.Once);
        }

        [TestMethod]
        public async Task InsertingItemIntoInventoryWillRemoveItemFromOldInventoryFirst()
        {
            var otherInventoryMock = new Mock<IInventory>();

            this.itemMock
                .SetupGet(x => x.CurrentInventory)
                .Returns(otherInventoryMock.Object);

            this.itemMock
                .Setup(x => x.SetCurrentInventory(this.Inventory));

            otherInventoryMock
                .Setup(x => x.RemoveItemAsync(this.itemMock.Object))
                .ReturnsAsync(true);

            await this.Inventory.InsertItemAsync(this.itemMock.Object);

            this.itemMock.VerifyGet(x => x.CurrentInventory, Times.AtLeastOnce);
            otherInventoryMock.Verify(x => x.RemoveItemAsync(this.itemMock.Object), Times.Once);

            this.itemMock.Verify(x => x.SetCurrentInventory(this.Inventory), Times.Once);
        }

        [TestMethod]
        public async Task FailingRemovingOldInventoryWillResultInFailedInsertion()
        {
            var otherInventoryMock = new Mock<IInventory>();

            this.itemMock
                .SetupGet(x => x.CurrentInventory)
                .Returns(otherInventoryMock.Object);

            otherInventoryMock
                .Setup(x => x.RemoveItemAsync(this.itemMock.Object))
                .ReturnsAsync(false);

            var success = await this.Inventory.InsertItemAsync(this.itemMock.Object);

            success.Should().BeFalse();

            this.itemMock.Verify(x => x.SetCurrentInventory(It.IsAny<IInventory>()), Times.Never);
        }
    }
}
