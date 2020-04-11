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
            Action act = () => this.Inventory.GetItems(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public async Task GettingItemsWithHandleOnlyWillReturnCorrectItems()
        {
            var items = await this.AddNonStackableItemsAsync();

            var returnedItem = this.Inventory.GetItems(ItemHandle);
            var expectedResult = items.Where(x => x.Handle == ItemHandle).ToArray();

            returnedItem.Should().OnlyContain(x => expectedResult.Contains(x));
        }

        [TestMethod]
        public async Task GettingItemsWithItemInterfaceWithNullHandleReturnsAllItems()
        {
            var items = await this.AddNonStackableItemsAsync();

            var returnedItem = this.Inventory.GetItems<IItem>(null);

            returnedItem.Should().OnlyContain(x => items.Contains(x));
        }

        [TestMethod]
        public async Task GettingItemsWithItemInterfaceWithSpecificHandleReturnsSubsetOfItems()
        {
            var items = await this.AddNonStackableItemsAsync();

            var returnedItem = this.Inventory.GetItems<IItem>(ItemHandle);
            var expectedResult = items.Where(x => x.Handle == ItemHandle).ToArray();

            returnedItem.Should().OnlyContain(x => expectedResult.Contains(x));
        }

        [TestMethod]
        public async Task GetItemsWithSpecificTypeParameterReturnsCorrectItems()
        {
            var items = await this.AddNonStackableItemsAsync();

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
        public async Task GetItemWithRightRuntimeIdReturnsCorrectItem()
        {
            await this.Inventory.InsertItemAsync(this.Item);

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
        public async Task GetItemWithWrongTypeAndKnownItemReturnsNull()
        {
            await this.Inventory.InsertItemAsync(this.Item);

            this.Inventory.GetItem<FakeItem>(this.Item.RuntimeId)
                .Should().BeNull();
        }

        [TestMethod]
        public async Task GetItemWithInheritedTypeAndKnownItemReturnsItem()
        {
            await this.Inventory.InsertItemAsync(this.Item);

            this.Inventory.GetItem<IItem>(this.Item.RuntimeId)
                .Should().Be(this.Item).And.BeAssignableTo<IItem>();
        }

        [TestMethod]
        public async Task GetItemWithRightTypeAndKnownItemReturnsItem()
        {
            await this.Inventory.InsertItemAsync(this.Item);

            this.Inventory.GetItem<RealItem>(this.Item.RuntimeId)
                .Should().Be(this.Item).And.BeOfType<RealItem>();
        }

        private async Task<ICollection<IItem>> AddNonStackableItemsAsync()
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
                await this.Inventory.InsertItemAsync(item);
            }

            return items;
        }

    }
}
