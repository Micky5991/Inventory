using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    public partial class InventoryFixture
    {

        [TestMethod]
        public void GettingItemsWithNullHandleThrowsException()
        {
            Action act = () => _inventory.GetItems(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public async Task GettingItemsWithHandleOnlyWillReturnCorrectItems()
        {
            var realMeta = _defaultRealMeta;
            _defaultRealMeta = new ItemMeta(realMeta.Handle, realMeta.Type, realMeta.DisplayName, realMeta.DefaultWeight, ItemFlags.NotStackable);

            SetupDefaultServiceProvider();

            _inventory.SetCapacity(1000);

            var items = new[]
            {
                _itemFactory.CreateItem(_realMeta, 1),
                _itemFactory.CreateItem(_realMeta, 1),
                _itemFactory.CreateItem(_fakeMeta, 1),
            };

            foreach (var item in items)
            {
                await _inventory.InsertItemAsync(item);
            }

            var returnedItem = _inventory.GetItems(ItemHandle);
            var expectedResult = items.Where(x => x.Handle == ItemHandle).ToArray();

            returnedItem.Should().OnlyContain(x => expectedResult.Contains(x));
        }

        [TestMethod]
        public async Task GettingItemsWithItemInterfaceWithNullHandleReturnsAllItems()
        {
            var realMeta = _defaultRealMeta;
            _defaultRealMeta = new ItemMeta(realMeta.Handle, realMeta.Type, realMeta.DisplayName, realMeta.DefaultWeight, ItemFlags.NotStackable);

            SetupDefaultServiceProvider();

            _inventory.SetCapacity(1000);

            var items = new[]
            {
                _itemFactory.CreateItem(_realMeta, 1),
                _itemFactory.CreateItem(_realMeta, 1),
                _itemFactory.CreateItem(_fakeMeta, 1),
            };

            foreach (var item in items)
            {
                await _inventory.InsertItemAsync(item);
            }

            var returnedItem = _inventory.GetItems<IItem>(null);

            returnedItem.Should().OnlyContain(x => items.Contains(x));
        }

        [TestMethod]
        public async Task GettingItemsWithItemInterfaceWithSpecificHandleReturnsSubsetOfItems()
        {
            var realMeta = _defaultRealMeta;
            _defaultRealMeta = new ItemMeta(realMeta.Handle, realMeta.Type, realMeta.DisplayName, realMeta.DefaultWeight, ItemFlags.NotStackable);

            SetupDefaultServiceProvider();

            _inventory.SetCapacity(1000);

            var items = new[]
            {
                _itemFactory.CreateItem(_realMeta, 1),
                _itemFactory.CreateItem(_realMeta, 1),
                _itemFactory.CreateItem(_fakeMeta, 1),
            };

            foreach (var item in items)
            {
                await _inventory.InsertItemAsync(item);
            }

            var returnedItem = _inventory.GetItems<IItem>(ItemHandle);
            var expectedResult = items.Where(x => x.Handle == ItemHandle).ToArray();

            returnedItem.Should().OnlyContain(x => expectedResult.Contains(x));
        }

        [TestMethod]
        public async Task GetItemsWithSpecificTypeParameterReturnsCorrectItems()
        {
            var realMeta = _defaultRealMeta;
            _defaultRealMeta = new ItemMeta(realMeta.Handle, realMeta.Type, realMeta.DisplayName, realMeta.DefaultWeight, ItemFlags.NotStackable);

            SetupDefaultServiceProvider();

            _inventory.SetCapacity(1000);

            var items = new[]
            {
                _itemFactory.CreateItem(_realMeta, 1),
                _itemFactory.CreateItem(_realMeta, 1),
                _itemFactory.CreateItem(_fakeMeta, 1),
            };

            foreach (var item in items)
            {
                await _inventory.InsertItemAsync(item);
            }

            var returnedItem = _inventory.GetItems<RealItem>(ItemHandle);
            var expectedResult = items.Where(x => x.GetType() == typeof(RealItem)).ToArray();

            returnedItem.Should().OnlyContain(x => expectedResult.Contains(x));
        }

    }
}
