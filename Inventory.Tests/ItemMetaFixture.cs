using System;
using FluentAssertions;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ItemMetaFixture
    {

        [TestMethod]
        public void ItemMetaConstructorSetsProperties()
        {
            var meta = new ItemMeta("testhandle", typeof(FakeItem), "FakeItem", 5, ItemFlags.NotStackable);

            meta.Handle.Should().Be("testhandle");
            meta.Type.Should().Be(typeof(FakeItem));
            meta.DisplayName.Should().Be("FakeItem");
            meta.DefaultWeight.Should().Be(5);
            meta.Flags.Should().Be(ItemFlags.NotStackable);
        }

        [TestMethod]
        public void ItemMetaWithTypeThatDoesNotImplementItemInterfaceWillThrowException()
        {
            Action act = () => new ItemMeta("testhandle", typeof(int), "FakeItem", 5);

            act.Should().Throw<ArgumentException>()
                .Where(x => x.Message.Contains(typeof(IItem).ToString()));
        }

        [TestMethod]
        public void ItemMetaWithNullTypeWillThrowException()
        {
            Action act = () => new ItemMeta("testhandle", null, "FakeItem", 1);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-2)]
        [DataRow(int.MinValue)]
        public void ItemMetaWithInvalidDefaultWeightTypeWillThrowException(int weight)
        {
            Action act = () => new ItemMeta("testhandle", typeof(FakeItem), "FakeItem", weight);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .Where(x => x.Message.Contains("1 or higher"));
        }

        [TestMethod]
        [DataRow(new string[] { null })]
        [DataRow(new string[] { "" })]
        [DataRow(new string[] { " " })]
        public void ItemMetaWithInvalidItemHandleWillThrowException(string[] handles)
        {
            Action act = () => new ItemMeta(handles[0], typeof(FakeItem), "FakeItem", 1);

            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        [DataRow(new string[] { null })]
        [DataRow(new string[] { "" })]
        [DataRow(new string[] { " " })]
        public void ItemMetaWithInvalidDisplayNameWillThrowException(string[] displayNames)
        {
            Action act = () => new ItemMeta("fakehandle", typeof(FakeItem), displayNames[0], 1);

            act.Should().Throw<ArgumentException>();
        }

    }
}
