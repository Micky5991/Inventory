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

        private ItemRegistry itemRegistry;

        [TestInitialize]
        public void Setup()
        {
            this.itemRegistry = new ItemRegistry();
        }

        [TestMethod]
        public void ReturningNullAtItemRegistryThrowsException()
        {
            this.itemRegistry.SetItemMetasNull();

            Action act = () => this.itemRegistry.GetItemMeta();

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x => x.Message.Contains("null") && x.Message.Contains("return"));
        }

        [TestMethod]
        public void NullWillBeIgnoredOnSetup()
        {
            this.itemRegistry.AddItemMeta(this.CreateItemMeta("item1"));
            this.itemRegistry.AddItemMeta(null);
            this.itemRegistry.AddItemMeta(this.CreateItemMeta("item2"));

            this.itemRegistry.GetItemMeta().Should().HaveCount(2);
        }

        [TestMethod]
        public void GettingNullHandleWillThrowException()
        {
            Action act = () => this.itemRegistry.TryGetItemMeta(null, out _);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RegisteringTwoItemsWithIdenticalHandlesThrowsException()
        {
            this.itemRegistry.AddItemMeta(this.CreateItemMeta("item"));
            this.itemRegistry.AddItemMeta(this.CreateItemMeta("item"));

            Action act = () => this.itemRegistry.ValidateAndCacheItemMeta();

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x => x.Message.Contains("duplicate handle"));
        }

        [TestMethod]
        public void CachingItemRegistryTwiceWillLoadOnlyOnce()
        {
            this.itemRegistry.AddItemMeta(this.CreateItemMeta("item2"));
            this.itemRegistry.AddItemMeta(this.CreateItemMeta("item"));

            this.itemRegistry.ValidateAndCacheItemMeta();
            this.itemRegistry.ValidateAndCacheItemMeta();

            this.itemRegistry.LoadedAmount.Should().Be(1);
        }

        [TestMethod]
        public void SearchingForItemWillReturnTrueAndGiveItemMeta()
        {
            var originalMeta = this.CreateItemMeta("item");

            this.itemRegistry.AddItemMeta(originalMeta);

            this.itemRegistry.TryGetItemMeta("item", out var meta).Should().BeTrue();
            meta.Should().Be(originalMeta);
        }

        [TestMethod]
        public void ItemRegistryWillBeValidatedUponFirstCollectionAccess()
        {
            this.itemRegistry.AddItemMeta(this.CreateItemMeta("item"));
            this.itemRegistry.AddItemMeta(this.CreateItemMeta("item"));

            Action act = () => this.itemRegistry.GetItemMeta();

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x => x.Message.Contains("duplicate handle"));
        }

        [TestMethod]
        public void ItemRegistryWillBeValidatedUponFirstTryGetAccess()
        {
            this.itemRegistry.AddItemMeta(this.CreateItemMeta("item"));
            this.itemRegistry.AddItemMeta(this.CreateItemMeta("item"));

            Action act = () => this.itemRegistry.TryGetItemMeta("item", out _);

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x => x.Message.Contains("duplicate handle"));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CreatingItemMetaShortHandWillThrowExceptionOnInvalidHandle(string handle)
        {
            Action act = () => this.itemRegistry.CreateItemMetaForward<FakeItem>(handle, "Testitem");

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void CreatingItemMetaShortHandWillThrowExceptionOnInvalidDisplayName(string displayName)
        {
            Action act = () => this.itemRegistry.CreateItemMetaForward<FakeItem>("item", displayName);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ItemMetaCreationShortHandReturnsValidItemMeta()
        {
            var meta = this.itemRegistry.CreateItemMetaForward<FakeItem>("itemHandle", "Fake", 222, ItemFlags.NotStackable);

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
