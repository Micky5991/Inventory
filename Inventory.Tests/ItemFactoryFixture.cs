using System;
using System.Linq;
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

        private ItemFactory itemFactory;
        private ItemRegistry itemRegistry;

        private ItemMeta defaultMeta;
        private ItemMeta nonStackableDefaultMeta;

        private Mock<IServiceProvider> serviceProviderMock;

        [TestInitialize]
        public void Setup()
        {
            this.serviceProviderMock = new Mock<IServiceProvider>();

            this.itemRegistry = new ItemRegistry();

            this.defaultMeta = InventoryUtils.CreateItemMeta(DefaultItemHandle, typeof(FakeItem), "Fake Item");
            this.nonStackableDefaultMeta = InventoryUtils.CreateItemMeta(DefaultNonStackableItemHandle, typeof(FakeItem), "Fake Item", flags: ItemFlags.NotStackable);

            this.itemRegistry.AddItemMeta(this.defaultMeta);
            this.itemRegistry.AddItemMeta(this.nonStackableDefaultMeta);

            this.itemRegistry.ValidateAndCacheItemMeta();

            this.AddItemResolveToServiceProvider<FakeItem>();

            this.itemFactory = new ItemFactory(this.itemRegistry, this.serviceProviderMock.Object);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void CreatingItemFromHandleWillCreateRightItem(int itemAmount)
        {
            var item = this.itemFactory.CreateItem(DefaultItemHandle, itemAmount);

            item.Should().NotBeNull();
            item.Should().BeOfType<FakeItem>();

            item.Meta.Should().Be(this.defaultMeta);
            item.Amount.Should().Be(itemAmount);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(2)]
        public void CreatingItemFromMetaWillCreateRightItem(int itemAmount)
        {
            var item = this.itemFactory.CreateItem(this.defaultMeta, itemAmount);

            item.Should().NotBeNull();
            item.Should().BeOfType<FakeItem>();

            item.Meta.Should().Be(this.defaultMeta);
            item.Amount.Should().Be(itemAmount);
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void CreatingNonStackableItemWithAmountOverOneWillThrowException(int itemAmount)
        {
            var meta = InventoryUtils.CreateItemMeta(flags: ItemFlags.NotStackable);

            Action act = () => this.itemFactory.CreateItem(meta, itemAmount);

            act.Should().Throw<ItemNotStackableException>();
        }

        [TestMethod]
        [DataRow(2)]
        [DataRow(3)]
        public void CreatingNonStackableItemFromHandleWithAmountOverOneWillThrowException(int itemAmount)
        {
            Action act = () => this.itemFactory.CreateItem(DefaultNonStackableItemHandle, itemAmount);

            act.Should().Throw<ItemNotStackableException>();
        }

        [TestMethod]
        public void CreatingNonStackableItemFromHandleWithAmountEqualToOneWillCreateItem()
        {
            var item = this.itemFactory.CreateItem(DefaultNonStackableItemHandle, 1);

            item.Should().NotBeNull();
            item.Should().BeOfType<FakeItem>();
        }

        [TestMethod]
        public void CreatingItemWithNullMetaThrowsException()
        {
            Action act = () => this.itemFactory.CreateItem((ItemMeta) null, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CreatingItemWithInvalidAmountThrowsException(int amount)
        {
            Action act = () => this.itemFactory.CreateItem(this.defaultMeta, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CreatingItemWithAmountBelowOneWillThrowException(int amount)
        {
            Action act = () => this.itemFactory.CreateItem(DefaultItemHandle, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CreatingItemWithInvalidHandleWillThrowException(string handle = null)
        {
            Action act = () => this.itemFactory.CreateItem(handle, 10);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CreatingItemWithUnknownHandleWillReturnNull()
        {
            var item = this.itemFactory.CreateItem("unknown", 10);

            item.Should().BeNull();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CreatingItemsWithNegativeAmountThrowsException(int amount)
        {
            Action act = () => this.itemFactory.CreateItems(DefaultItemHandle, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        public void CreatingItemsWithNegativeAmountInMetaOverloadThrowsException(int amount)
        {
            Action act = () => this.itemFactory.CreateItems(this.defaultMeta, amount);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CreatingItemsWithInvalidHandleThrowsException(string handle)
        {
            Action act = () => this.itemFactory.CreateItems(handle, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CreatingItemsWithNullMetaThrowsException()
        {
            Action act = () => this.itemFactory.CreateItems((ItemMeta) null, 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void CreatingItemWithKnownHandleReturnsCorrentItem()
        {
            var resultItems = this.itemFactory.CreateItems(DefaultItemHandle, 1);

            resultItems.Should().NotBeNull();
            resultItems.Should().HaveCount(1);

            var resultItem = resultItems.First();

            resultItem.Should().NotBeNull().And.BeOfType(this.defaultMeta.Type);
            resultItem.Amount.Should().Be(1);
        }

        [TestMethod]
        public void CreatingItemWithKnownMetaReturnsCorrentItem()
        {
            var resultItems = this.itemFactory.CreateItems(this.defaultMeta, 1);

            resultItems.Should().NotBeNull();
            resultItems.Should().HaveCount(1);

            var resultItem = resultItems.First();

            resultItem.Should().NotBeNull().And.BeOfType(this.defaultMeta.Type);
            resultItem.Amount.Should().Be(1);
        }

        [TestMethod]
        public void CreatingItemsWithUnknownHandleReturnsNull()
        {
            var resultItems = this.itemFactory.CreateItems("unknownhandle", 1);

            resultItems.Should().BeNull();
        }

        [TestMethod]
        public void CreatingItemsWithNonStackableHandleReturnsMultipleItems()
        {
            var resultItems = this.itemFactory.CreateItems(DefaultNonStackableItemHandle, 5);

            resultItems.Should()
                .HaveCount(5)
                .And.OnlyHaveUniqueItems()
                .And.AllBeOfType(this.nonStackableDefaultMeta.Type)
                .And.NotContainNulls()
                .And.OnlyContain(x => x.Amount == 1);
        }

        [TestMethod]
        public void CreatingItemsWithNonStackableMetaReturnsMultipleItems()
        {
            var resultItems = this.itemFactory.CreateItems(this.nonStackableDefaultMeta, 5);

            resultItems.Should()
                .HaveCount(5)
                .And.OnlyHaveUniqueItems()
                .And.AllBeOfType(this.nonStackableDefaultMeta.Type)
                .And.NotContainNulls()
                .And.OnlyContain(x => x.Amount == 1);
        }

        private void AddItemResolveToServiceProvider<T>() where T : IItem
        {
            this.serviceProviderMock
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
