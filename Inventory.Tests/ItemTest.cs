using System;
using System.Linq;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Item;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Extensions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Micky5991.Inventory.Tests
{
    public class ItemTest
    {
        protected const string ItemHandle = "testhandle";
        protected const string ItemDisplayName = "RealItem";
        protected const int ItemWeight = 50;
        protected const ItemFlags ItemFlags = Enums.ItemFlags.None;

        protected const string FakeItemHandle = "fakehandle";
        protected const string FakeItemDisplayName = "FakeItem";
        protected const int FakeItemWeight = 50;
        protected const ItemFlags FakeItemFlags = ItemFlags.None;

        protected ItemMeta _realMeta;
        protected ItemMeta _fakeMeta;

        protected ItemMeta _defaultRealMeta;
        protected ItemMeta _defaultFakeMeta;

        protected Item _item;
        protected FakeItem _fakeItem;

        protected IInventory _inventory;

        protected IServiceCollection _serviceCollection;
        protected IServiceProvider _serviceProvider;

        protected ItemRegistry _itemRegistry;
        protected AggregatedItemServices _itemServices;
        protected AggregatedInventoryServices _inventoryServices;

        protected Mock<IItemFactory> _itemFactoryMock;
        protected Mock<IInventoryFactory> _inventoryFactoryMock;
        protected Mock<IItemSplitStrategyHandler> _itemSplitStrategyHandlerMock;
        protected Mock<IItemMergeStrategyHandler> _itemMergeStrategyHandlerMock;

        protected IItemFactory _itemFactory;
        protected IInventoryFactory _inventoryFactory;
        protected IItemMergeStrategyHandler _itemMergeStrategyHandler;
        protected IItemSplitStrategyHandler _itemSplitStrategyHandler;

        protected void SetupItemTest()
        {
            this._defaultRealMeta = new ItemMeta(ItemHandle, typeof(RealItem), ItemDisplayName, ItemWeight, ItemFlags);
            this._defaultFakeMeta = new ItemMeta(FakeItemHandle, typeof(FakeItem), FakeItemDisplayName, FakeItemWeight, FakeItemFlags);

            this.SetupDefaultServiceProvider();
        }

        protected void SetupDependencies()
        {
            this._itemRegistry = new ItemRegistry();
            this._serviceCollection = new ServiceCollection();

            this._serviceCollection.AddInventoryServices();

            if (this._itemFactoryMock != null)
            {
                this._serviceCollection.AddTransient(x => this._itemFactoryMock.Object);
            }
            else
            {
                this._serviceCollection.AddDefaultItemFactory();
            }

            if (this._inventoryFactoryMock != null)
            {
                this._serviceCollection.AddTransient(x => this._inventoryFactoryMock.Object);
            }
            else
            {
                this._serviceCollection.AddDefaultInventoryFactory();
            }

            if (this._itemSplitStrategyHandlerMock != null)
            {
                this._serviceCollection.AddTransient(x => this._itemSplitStrategyHandlerMock.Object);
            }
            else
            {
                this._serviceCollection.AddDefaultInventorySplitStrategy();
            }

            if (this._itemMergeStrategyHandlerMock != null)
            {
                this._serviceCollection.AddTransient(x => this._itemMergeStrategyHandlerMock.Object);
            }
            else
            {
                this._serviceCollection.AddDefaultInventoryMergeStrategy();
            }
        }

        protected void TearDownItemTest()
        {
            this._serviceProvider = null;
            this._serviceCollection = null;
            this._itemFactory = null;
            this._itemRegistry = null;

            this._realMeta = null;
            this._fakeMeta = null;

            this._item = null;
            this._fakeItem = null;
            this._inventory = null;

            this._inventoryFactoryMock = null;
            this._itemFactoryMock = null;
            this._itemMergeStrategyHandlerMock = null;
            this._itemSplitStrategyHandlerMock = null;
        }

        protected void SetupServiceProvider(params ItemMeta[] itemMetas)
        {
            this.SetupDependencies();

            foreach (var itemMeta in itemMetas)
            {
                this._itemRegistry.AddItemMeta(itemMeta);
            }

            this._serviceCollection.AddItemTypes(this._itemRegistry);
            this._serviceProvider = this._serviceCollection.BuildServiceProvider();

            this._itemFactory = this._serviceProvider.GetRequiredService<IItemFactory>();
            this._itemServices = this._serviceProvider.GetRequiredService<AggregatedItemServices>();

            this._inventoryFactory = this._serviceProvider.GetRequiredService<IInventoryFactory>();
            this._inventoryServices = this._serviceProvider.GetRequiredService<AggregatedInventoryServices>();

            this._itemSplitStrategyHandler = this._serviceProvider.GetRequiredService<IItemSplitStrategyHandler>();
            this._itemMergeStrategyHandler = this._serviceProvider.GetRequiredService<IItemMergeStrategyHandler>();

            this._item = (Item) this._itemFactory.CreateItem(itemMetas.First(), 1);
            this._inventory = this._inventoryFactory.CreateInventory(100);
        }

        protected void SetupDefaultServiceProvider()
        {
            this._realMeta = this._defaultRealMeta;
            this._fakeMeta = this._defaultFakeMeta;

            this.SetupServiceProvider(this._defaultRealMeta, this._defaultFakeMeta);

            this._item = (Item) this._itemFactory.CreateItem(this._defaultRealMeta, 1);
            this._fakeItem = (FakeItem) this._itemFactory.CreateItem(this._defaultFakeMeta, 1);
        }

    }
}
