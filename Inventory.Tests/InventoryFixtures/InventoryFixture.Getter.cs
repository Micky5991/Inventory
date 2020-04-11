using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests.InventoryFixtures
{
    public partial class InventoryFixture
    {

        [TestMethod]
        public void GettingItemsWithNullHandleThrowsException()
        {
            Action act = () => this.Inventory.GetItems(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void GettingItemsWithHandleOnlyWillReturnCorrectItems()
        {
            var items = this.AddNonStackableItems();

            var returnedItem = this.Inventory.GetItems(ItemHandle);
            var expectedResult = items.Where(x => x.Handle == ItemHandle).ToArray();

            returnedItem.Should().OnlyContain(x => expectedResult.Contains(x));
        }

        [TestMethod]
        public void GettingItemsWithItemInterfaceWithNullHandleReturnsAllItems()
        {
            var items = this.AddNonStackableItems();

            var returnedItem = this.Inventory.GetItems<IItem>(null);

            returnedItem.Should().OnlyContain(x => items.Contains(x));
        }

        [TestMethod]
        public void GettingItemsWithItemInterfaceWithSpecificHandleReturnsSubsetOfItems()
        {
            var items = this.AddNonStackableItems();

            var returnedItem = this.Inventory.GetItems<IItem>(ItemHandle);
            var expectedResult = items.Where(x => x.Handle == ItemHandle).ToArray();

            returnedItem.Should().OnlyContain(x => expectedResult.Contains(x));
        }

        [TestMethod]
        public void GetItemsWithSpecificTypeParameterReturnsCorrectItems()
        {
            var items = this.AddNonStackableItems();

            var returnedItem = this.Inventory.GetItems<RealItem>(ItemHandle);
            var expectedResult = items.Where(x => x.GetType() == typeof(RealItem)).ToArray();

            returnedItem.Should().OnlyContain(x => expectedResult.Contains(x));
        }

        [TestMethod]
        public void GetItemWithEmptyGuidThrowsException()
        {
            Action act = () => this.Inventory.GetItem(Guid.Empty);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void GetItemWithRightRuntimeIdReturnsCorrectItem()
        {
            this.Inventory.InsertItem(this.Item);

            this.Inventory.GetItem(this.Item.RuntimeId)
                .Should().Be(this.Item);
        }

        [TestMethod]
        public void GetItemWithUnknownItemReturnsNull()
        {
            this.Inventory.GetItem(this.Item.RuntimeId)
                .Should().BeNull();
        }

        [TestMethod]
        public void GetItemWithTypeAndEmptyGuidThrowsException()
        {
            Action act = () => this.Inventory.GetItem<IItem>(Guid.Empty);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void GetItemWithTypeUnknownItemReturnsNull()
        {
            this.Inventory.GetItem<IItem>(this.Item.RuntimeId)
                .Should().BeNull();
        }

        [TestMethod]
        public void GetItemWithWrongTypeAndKnownItemReturnsNull()
        {
            this.Inventory.InsertItem(this.Item);

            this.Inventory.GetItem<FakeItem>(this.Item.RuntimeId)
                .Should().BeNull();
        }

        [TestMethod]
        public void GetItemWithInheritedTypeAndKnownItemReturnsItem()
        {
            this.Inventory.InsertItem(this.Item);

            this.Inventory.GetItem<IItem>(this.Item.RuntimeId)
                .Should().Be(this.Item).And.BeAssignableTo<IItem>();
        }

        [TestMethod]
        public void GetItemWithRightTypeAndKnownItemReturnsItem()
        {
            this.Inventory.InsertItem(this.Item);

            this.Inventory.GetItem<RealItem>(this.Item.RuntimeId)
                .Should().Be(this.Item).And.BeOfType<RealItem>();
        }

        private ICollection<IItem> AddNonStackableItems()
        {
            var realMeta = this.DefaultRealMeta;
            this.DefaultRealMeta = new ItemMeta(realMeta.Handle, realMeta.Type, realMeta.DisplayName, realMeta.DefaultWeight, ItemFlags.NotStackable);

            this.Setup();

            this.Inventory.SetCapacity(1000);

            var items = new[]
            {
                this.ItemFactory.CreateItem(this.RealMeta, 1), this.ItemFactory.CreateItem(this.RealMeta, 1), this.ItemFactory.CreateItem(this.FakeMeta, 1),
            };

            foreach (var item in items)
            {
                this.Inventory.InsertItem(item);
            }

            return items;
        }

    }
}
