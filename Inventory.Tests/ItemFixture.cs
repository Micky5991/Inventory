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
        private const string ItemDisplayName = "FakeItem";
        private const int ItemWeight = 50;
        private const ItemFlags ItemFlags = Enums.ItemFlags.None;

        private ItemMeta _meta;
        private Item _item;

        [TestInitialize]
        public void Setup()
        {
            _meta = new ItemMeta(ItemHandle, typeof(RealItem), ItemDisplayName, ItemWeight, ItemFlags);
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
            item.TotalWeight.Should().Be(_meta.DefaultWeight);
            item.DisplayName.Should().Be(_meta.DisplayName);
            item.DefaultDisplayName.Should().Be(_meta.DisplayName);
            item.Stackable.Should().BeTrue();

            item.RuntimeId.Should().NotBe(Guid.Empty);
        }

        [TestMethod]
        [DataRow(ItemFlags.NotStackable, false)]
        [DataRow(ItemFlags.None, true)]
        public void SettingNonStackableFlagWillBeInterpretedCorrectly(ItemFlags flags, bool stackable)
        {
            _meta = new ItemMeta(_meta.Handle, _meta.Type, _meta.DisplayName, _meta.DefaultWeight, flags);
            _item = new RealItem(_meta);

            _item.Stackable.Should().Be(stackable);
        }

        [TestMethod]
        public void CreatingItemWithNullItemMetaWillThrowException()
        {
            Action act = () => new RealItem(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void ChangingDisplayNameUpdatesValueCorrecly()
        {
            _item.SetDisplayName("Cool");

            _item.DisplayName.Should().Be("Cool");
        }

        [TestMethod]
        public void ChangingDisplayNameKeepsValueInDefaultDisplayNameSame()
        {
            _item.SetDisplayName("Other");

            var oldName = _item.DefaultDisplayName;

            _item.DefaultDisplayName.Should().Be(oldName);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        public void SettingDisplayNameToNullThrowsException(string displayName)
        {
            var oldName = _item.DisplayName;
            Action act = () => _item.SetDisplayName(displayName);

            act.Should().Throw<ArgumentNullException>();
            _item.DisplayName.Should().Be(oldName);
        }

        [TestMethod]
        public void InequalHandleShouldPreventMerging()
        {
            var fakeItem = new FakeItem(10, "unmergableitem");

            _item.CanMergeWith(fakeItem).Should().BeFalse();
        }

        [TestMethod]
        public void NonStackableItemsWillNotBeMergable()
        {
            var fakeItem = new FakeItem(10, ItemHandle, flags: ItemFlags.NotStackable);

            _item.CanMergeWith(fakeItem).Should().BeFalse();
        }

        [TestMethod]
        public void SameItemWillNotBeMergable()
        {
            _item.CanMergeWith(_item).Should().BeFalse();
        }

    }
}
