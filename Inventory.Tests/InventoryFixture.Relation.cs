using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    public partial class InventoryFixture
    {
        [TestMethod]
        public async Task AddingItemWillSetCurrentInventoryCorrectly()
        {
            _itemMock.Setup(x => x.SetCurrentInventory(_inventory));

            await _inventory.InsertItemAsync(_itemMock.Object);

            _itemMock.Verify(x => x.SetCurrentInventory(_inventory));
        }

        [TestMethod]
        public async Task RemovingItemWillSetCurrentInventoryToNull()
        {
            _itemMock.Setup(x => x.SetCurrentInventory(It.IsAny<IInventory>()));

            await _inventory.InsertItemAsync(_itemMock.Object);
            await _inventory.RemoveItemAsync(_itemMock.Object);

            _itemMock.Verify(x => x.SetCurrentInventory(null), Times.Once);
        }

        [TestMethod]
        public async Task InsertingItemIntoInventoryWillRemoveItemFromOldInventoryFirst()
        {
            var otherInventoryMock = new Mock<IInventory>();

            _itemMock
                .SetupGet(x => x.CurrentInventory)
                .Returns(otherInventoryMock.Object);

            _itemMock
                .Setup(x => x.SetCurrentInventory(_inventory));

            otherInventoryMock
                .Setup(x => x.RemoveItemAsync(_itemMock.Object))
                .ReturnsAsync(true);

            await _inventory.InsertItemAsync(_itemMock.Object);

            _itemMock.VerifyGet(x => x.CurrentInventory, Times.AtLeastOnce);
            otherInventoryMock.Verify(x => x.RemoveItemAsync(_itemMock.Object), Times.Once);

            _itemMock.Verify(x => x.SetCurrentInventory(_inventory), Times.Once);
        }

        [TestMethod]
        public async Task FailingRemovingOldInventoryWillResultInFailedInsertion()
        {
            var otherInventoryMock = new Mock<IInventory>();

            _itemMock
                .SetupGet(x => x.CurrentInventory)
                .Returns(otherInventoryMock.Object);

            otherInventoryMock
                .Setup(x => x.RemoveItemAsync(_itemMock.Object))
                .ReturnsAsync(false);

            var success = await _inventory.InsertItemAsync(_itemMock.Object);

            success.Should().BeFalse();

            _itemMock.Verify(x => x.SetCurrentInventory(It.IsAny<IInventory>()), Times.Never);
        }
    }
}
