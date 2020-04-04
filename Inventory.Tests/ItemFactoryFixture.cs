using System;
using FluentAssertions;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Micky5991.Inventory.Tests.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ItemFactoryFixture
    {
        private const string DefaultItemHandle = "item";
        private const string DefaultNonStackableItemHandle = "item_nonstackable";

        private ItemFactory _itemFactory;
        private ItemRegistry _itemRegistry;

        private ItemMeta _defaultMeta;
        private ItemMeta _nonStackableDefaultMeta;

        private Mock<IServiceProvider> _serviceProviderMock;

        [TestInitialize]
        public void Setup()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();

            _itemRegistry = new ItemRegistry();

            _defaultMeta = InventoryUtils.CreateItemMeta(DefaultItemHandle, typeof(FakeItem), "Fake Item");
            _nonStackableDefaultMeta = InventoryUtils.CreateItemMeta(DefaultNonStackableItemHandle, typeof(FakeItem), "Fake Item", flags: ItemFlags.NotStackable);

            _itemRegistry.AddItemMeta(_defaultMeta);
            _itemRegistry.AddItemMeta(_nonStackableDefaultMeta);

            _itemRegistry.ValidateAndCacheItemMeta();

            AddItemResolveToServiceProvider<FakeItem>();

            _itemFactory = new ItemFactory(_itemRegistry, _serviceProviderMock.Object);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void CreatingItemFromHandleWillCreateRightItem(int itemAmount)
        {
            var item = _itemFactory.CreateItem(DefaultItemHandle, itemAmount);

            item.Should().NotBeNull();
            item.Should().BeOfType<FakeItem>();

            item.Meta.Should().Be(_defaultMeta);
            item.Amount.Should().Be(itemAmount);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void CreatingItemFromMetaWillCreateRightItem(int itemAmount)
        {
            var item = _itemFactory.CreateItem(_defaultMeta, itemAmount);

            item.Should().NotBeNull();
            item.Should().BeOfType<FakeItem>();

            item.Meta.Should().Be(_defaultMeta);
            item.Amount.Should().Be(itemAmount);
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void CreatingNonStackableItemWithAmountOverOneWillThrowException(int itemAmount)
        {
            var meta = InventoryUtils.CreateItemMeta(flags: ItemFlags.NotStackable);

            Action act = () => _itemFactory.CreateItem(meta, itemAmount);

            act.Should().Throw<ItemNotStackableException>();
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void CreatingNonStackableItemFromHandleWithAmountOverOneWillThrowException(int itemAmount)
        {
            Action act = () => _itemFactory.CreateItem(DefaultNonStackableItemHandle, itemAmount);

            act.Should().Throw<ItemNotStackableException>();
        }

        [TestMethod]
        public void CreatingItemWithNullMetaThrowsException()
        {
            Action act = () => _itemFactory.CreateItem((ItemMeta) null, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CreatingItemWithInvalidAmountThrowsException(int amount)
        {
            Action act = () => _itemFactory.CreateItem(_defaultMeta, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CreatingItemWithAmountBelowOneWillThrowException(int amount)
        {
            Action act = () => _itemFactory.CreateItem(DefaultItemHandle, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CreatingItemWithInvalidHandleWillThrowException(string handle = null)
        {
            Action act = () => _itemFactory.CreateItem(handle, 10);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CreatingItemWithUnknownHandleWillReturnNull()
        {
            var item = _itemFactory.CreateItem("unknown", 10);

            item.Should().BeNull();
        }

        private void AddItemResolveToServiceProvider<T>() where T : IItem
        {
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(T)))
                .Returns<Type>(x =>
                {
                    IItem CreateItem(IServiceProvider serviceProvider, object[] arguments)
                    {
                        return (T) Activator.CreateInstance(typeof(T), arguments);
                    }

                    return new ObjectFactory(CreateItem);
                });
        }

    }
}
