using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Action act = () => this._inventory.GetItems(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public async Task GettingItemsWithHandleOnlyWillReturnCorrectItems()
        {
            var items = await this.AddNonStackableItemsAsync();

            var returnedItem = this._inventory.GetItems(ItemHandle);
            var expectedResult = items.Where(x => x.Handle == ItemHandle).ToArray();

            returnedItem.Should().OnlyContain(x => expectedResult.Contains(x));
        }

        [TestMethod]
        public async Task GettingItemsWithItemInterfaceWithNullHandleReturnsAllItems()
        {
            var items = await this.AddNonStackableItemsAsync();

            var returnedItem = this._inventory.GetItems<IItem>(null);

            returnedItem.Should().OnlyContain(x => items.Contains(x));
        }

        [TestMethod]
        public async Task GettingItemsWithItemInterfaceWithSpecificHandleReturnsSubsetOfItems()
        {
            var items = await this.AddNonStackableItemsAsync();

            var returnedItem = this._inventory.GetItems<IItem>(ItemHandle);
            var expectedResult = items.Where(x => x.Handle == ItemHandle).ToArray();

            returnedItem.Should().OnlyContain(x => expectedResult.Contains(x));
        }

        [TestMethod]
        public async Task GetItemsWithSpecificTypeParameterReturnsCorrectItems()
        {
            var items = await this.AddNonStackableItemsAsync();

            var returnedItem = this._inventory.GetItems<RealItem>(ItemHandle);
            var expectedResult = items.Where(x => x.GetType() == typeof(RealItem)).ToArray();

            returnedItem.Should().OnlyContain(x => expectedResult.Contains(x));
        }

        [TestMethod]
        public void GetItemWithEmptyGuidThrowsException()
        {
            Action act = () => this._inventory.GetItem(Guid.Empty);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public async Task GetItemWithRightRuntimeIdReturnsCorrectItem()
        {
            await this._inventory.InsertItemAsync(this._item);

            this._inventory.GetItem(this._item.RuntimeId)
                .Should().Be(this._item);
        }

        [TestMethod]
        public void GetItemWithUnknownItemReturnsNull()
        {
            this._inventory.GetItem(this._item.RuntimeId)
                .Should().BeNull();
        }

        [TestMethod]
        public void GetItemWithTypeAndEmptyGuidThrowsException()
        {
            Action act = () => this._inventory.GetItem<IItem>(Guid.Empty);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void GetItemWithTypeUnknownItemReturnsNull()
        {
            this._inventory.GetItem<IItem>(this._item.RuntimeId)
                .Should().BeNull();
        }

        [TestMethod]
        public async Task GetItemWithWrongTypeAndKnownItemReturnsNull()
        {
            await this._inventory.InsertItemAsync(this._item);

            this._inventory.GetItem<FakeItem>(this._item.RuntimeId)
                .Should().BeNull();
        }

        [TestMethod]
        public async Task GetItemWithInheritedTypeAndKnownItemReturnsItem()
        {
            await this._inventory.InsertItemAsync(this._item);

            this._inventory.GetItem<IItem>(this._item.RuntimeId)
                .Should().Be(this._item).And.BeAssignableTo<IItem>();
        }

        [TestMethod]
        public async Task GetItemWithRightTypeAndKnownItemReturnsItem()
        {
            await this._inventory.InsertItemAsync(this._item);

            this._inventory.GetItem<RealItem>(this._item.RuntimeId)
                .Should().Be(this._item).And.BeOfType<RealItem>();
        }

        private async Task<ICollection<IItem>> AddNonStackableItemsAsync()
        {
            var realMeta = this._defaultRealMeta;
            this._defaultRealMeta = new ItemMeta(realMeta.Handle, realMeta.Type, realMeta.DisplayName, realMeta.DefaultWeight, ItemFlags.NotStackable);

            this.Setup();

            this._inventory.SetCapacity(1000);

            var items = new[]
            {
                this._itemFactory.CreateItem(this._realMeta, 1), this._itemFactory.CreateItem(this._realMeta, 1), this._itemFactory.CreateItem(this._fakeMeta, 1),
            };

            foreach (var item in items)
            {
                await this._inventory.InsertItemAsync(item);
            }

            return items;
        }

    }
}
