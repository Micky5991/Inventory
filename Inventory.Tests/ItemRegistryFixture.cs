using System;
using FluentAssertions;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ItemRegistryFixture
    {

        private ItemRegistry _itemRegistry;

        [TestInitialize]
        public void Setup()
        {
            _itemRegistry = new ItemRegistry();
        }

        [TestMethod]
        public void ReturningNullAtItemRegistryThrowsException()
        {
            _itemRegistry.SetItemMetasNull();

            Action act = () => _itemRegistry.GetItemMeta();

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x => x.Message.Contains("null") && x.Message.Contains("return"));
        }

        [TestMethod]
        public void NullWillBeIgnoredOnSetup()
        {
            _itemRegistry.AddItemMeta(CreateItemMeta("item1"));
            _itemRegistry.AddItemMeta(null);
            _itemRegistry.AddItemMeta(CreateItemMeta("item2"));

            _itemRegistry.GetItemMeta().Should().HaveCount(2);
        }

        [TestMethod]
        public void GettingNullHandleWillThrowException()
        {
            Action act = () => _itemRegistry.TryGetItemMeta(null, out _);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RegisteringTwoItemsWithIdenticalHandlesThrowsException()
        {
            _itemRegistry.AddItemMeta(CreateItemMeta("item"));
            _itemRegistry.AddItemMeta(CreateItemMeta("item"));

            Action act = () => _itemRegistry.ValidateAndCacheItemMeta();

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x => x.Message.Contains("duplicate handle"));
        }

        [TestMethod]
        public void CachingItemRegistryTwiceWillLoadOnlyOnce()
        {
            _itemRegistry.AddItemMeta(CreateItemMeta("item2"));
            _itemRegistry.AddItemMeta(CreateItemMeta("item"));

            _itemRegistry.ValidateAndCacheItemMeta();
            _itemRegistry.ValidateAndCacheItemMeta();

            _itemRegistry.LoadedAmount.Should().Be(1);
        }

        [TestMethod]
        public void SearchingForItemWillReturnTrueAndGiveItemMeta()
        {
            var originalMeta = CreateItemMeta("item");

            _itemRegistry.AddItemMeta(originalMeta);

            _itemRegistry.TryGetItemMeta("item", out var meta).Should().BeTrue();
            meta.Should().Be(originalMeta);
        }

        [TestMethod]
        public void ItemRegistryWillBeValidatedUponFirstCollectionAccess()
        {
            _itemRegistry.AddItemMeta(CreateItemMeta("item"));
            _itemRegistry.AddItemMeta(CreateItemMeta("item"));

            Action act = () => _itemRegistry.GetItemMeta();

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x => x.Message.Contains("duplicate handle"));
        }

        [TestMethod]
        public void ItemRegistryWillBeValidatedUponFirstTryGetAccess()
        {
            _itemRegistry.AddItemMeta(CreateItemMeta("item"));
            _itemRegistry.AddItemMeta(CreateItemMeta("item"));

            Action act = () => _itemRegistry.TryGetItemMeta("item", out _);

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x => x.Message.Contains("duplicate handle"));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CreatingItemMetaShortHandWillThrowExceptionOnInvalidHandle(string handle)
        {
            Action act = () => _itemRegistry.CreateItemMetaForward<FakeItem>(handle, "Testitem");

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CreatingItemMetaShortHandWillThrowExceptionOnInvalidDisplayName(string displayName)
        {
            Action act = () => _itemRegistry.CreateItemMetaForward<FakeItem>("item", displayName);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ItemMetaCreationShortHandReturnsValidItemMeta()
        {
            var meta = _itemRegistry.CreateItemMetaForward<FakeItem>("itemHandle", "Fake", 222, ItemFlags.NotStackable);

            meta.Should().NotBeNull();

            meta.Handle.Should().Be("itemHandle");
            meta.DisplayName.Should().Be("Fake");
            meta.DefaultWeight.Should().Be(222);
            meta.Flags.Should().Be(ItemFlags.NotStackable);
        }

        private ItemMeta CreateItemMeta(string handle = "itemhandle")
        {
            return new ItemMeta(handle, typeof(FakeItem), "Fake item", 1, ItemFlags.None);
        }

    }
}
