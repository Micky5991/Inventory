using System;
using FluentAssertions;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ItemFixture
    {
        private const string ItemHandle = "testhandle";
        private const int ItemWeight = 50;
        private const ItemFlags ItemFlags = Enums.ItemFlags.None;

        private ItemMeta _meta;
        private Item _item;

        [TestInitialize]
        public void Setup()
        {
            _meta = new ItemMeta(ItemHandle, typeof(RealItem), ItemWeight, ItemFlags);
            _item = new RealItem(_meta);
        }

        [TestMethod]
        public void CreatingItemWillBeSuccessful()
        {
            var item = _item;

            item.Should()
                .NotBeNull()
                .And.BeAssignableTo<IItem>();
        }

        [TestMethod]
        public void CreatingItemWillSetParametersCorrectly()
        {
            var item = new RealItem(_meta);

            item.Meta.Should().Be(_meta);
            item.Handle.Should().Be(_meta.Handle);
            item.Weight.Should().Be(_meta.DefaultWeight);

            item.RuntimeId.Should().NotBe(Guid.Empty);
        }

        [TestMethod]
        public void CreatingItemWithNullItemMetaWillThrowException()
        {
            Action act = () => new RealItem(null);

            act.Should().Throw<ArgumentNullException>();
        }


    }
}
