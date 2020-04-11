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
            this._itemMock.Setup(x => x.SetCurrentInventory(this._inventory));

            await this._inventory.InsertItemAsync(this._itemMock.Object);

            this._itemMock.Verify(x => x.SetCurrentInventory(this._inventory));
        }

        [TestMethod]
        public async Task RemovingItemWillSetCurrentInventoryToNull()
        {
            this._itemMock.Setup(x => x.SetCurrentInventory(It.IsAny<IInventory>()));

            await this._inventory.InsertItemAsync(this._itemMock.Object);
            await this._inventory.RemoveItemAsync(this._itemMock.Object);

            this._itemMock.Verify(x => x.SetCurrentInventory(null), Times.Once);
        }

        [TestMethod]
        public async Task InsertingItemIntoInventoryWillRemoveItemFromOldInventoryFirst()
        {
            var otherInventoryMock = new Mock<IInventory>();

            this._itemMock
                .SetupGet(x => x.CurrentInventory)
                .Returns(otherInventoryMock.Object);

            this._itemMock
                .Setup(x => x.SetCurrentInventory(this._inventory));

            otherInventoryMock
                .Setup(x => x.RemoveItemAsync(this._itemMock.Object))
                .ReturnsAsync(true);

            await this._inventory.InsertItemAsync(this._itemMock.Object);

            this._itemMock.VerifyGet(x => x.CurrentInventory, Times.AtLeastOnce);
            otherInventoryMock.Verify(x => x.RemoveItemAsync(this._itemMock.Object), Times.Once);

            this._itemMock.Verify(x => x.SetCurrentInventory(this._inventory), Times.Once);
        }

        [TestMethod]
        public async Task FailingRemovingOldInventoryWillResultInFailedInsertion()
        {
            var otherInventoryMock = new Mock<IInventory>();

            this._itemMock
                .SetupGet(x => x.CurrentInventory)
                .Returns(otherInventoryMock.Object);

            otherInventoryMock
                .Setup(x => x.RemoveItemAsync(this._itemMock.Object))
                .ReturnsAsync(false);

            var success = await this._inventory.InsertItemAsync(this._itemMock.Object);

            success.Should().BeFalse();

            this._itemMock.Verify(x => x.SetCurrentInventory(It.IsAny<IInventory>()), Times.Never);
        }
    }
}
