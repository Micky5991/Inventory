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
        protected const ItemFlags FakeItemFlags = Enums.ItemFlags.None;

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
            _defaultRealMeta = new ItemMeta(ItemHandle, typeof(RealItem), ItemDisplayName, ItemWeight, ItemFlags);
            _defaultFakeMeta = new ItemMeta(FakeItemHandle, typeof(FakeItem), FakeItemDisplayName, FakeItemWeight, FakeItemFlags);

            SetupDefaultServiceProvider();
        }

        protected void SetupDependencies()
        {
            _itemRegistry = new ItemRegistry();
            _serviceCollection = new ServiceCollection();

            _serviceCollection.AddInventoryServices();

            if (_itemFactoryMock != null)
            {
                _serviceCollection.AddTransient(x => _itemFactoryMock.Object);
            }
            else
            {
                _serviceCollection.AddDefaultItemFactory();
            }

            if (_inventoryFactoryMock != null)
            {
                _serviceCollection.AddTransient(x => _inventoryFactoryMock.Object);
            }
            else
            {
                _serviceCollection.AddDefaultInventoryFactory();
            }

            if (_itemSplitStrategyHandlerMock != null)
            {
                _serviceCollection.AddTransient(x => _itemSplitStrategyHandlerMock.Object);
            }
            else
            {
                _serviceCollection.AddDefaultInventorySplitStrategy();
            }

            if (_itemMergeStrategyHandlerMock != null)
            {
                _serviceCollection.AddTransient(x => _itemMergeStrategyHandlerMock.Object);
            }
            else
            {
                _serviceCollection.AddDefaultInventoryMergeStrategy();
            }
        }

        protected void TearDownItemTest()
        {
            _serviceProvider = null;
            _serviceCollection = null;
            _itemFactory = null;
            _itemRegistry = null;

            _realMeta = null;
            _fakeMeta = null;

            _item = null;
            _fakeItem = null;
            _inventory = null;

            _inventoryFactoryMock = null;
            _itemFactoryMock = null;
            _itemMergeStrategyHandlerMock = null;
            _itemSplitStrategyHandlerMock = null;
        }

        protected void SetupServiceProvider(params ItemMeta[] itemMetas)
        {
            SetupDependencies();

            foreach (var itemMeta in itemMetas)
            {
                _itemRegistry.AddItemMeta(itemMeta);
            }

            _serviceCollection.AddItemTypes(_itemRegistry);
            _serviceProvider = _serviceCollection.BuildServiceProvider();

            _itemFactory = _serviceProvider.GetRequiredService<IItemFactory>();
            _itemServices = _serviceProvider.GetRequiredService<AggregatedItemServices>();

            _inventoryFactory = _serviceProvider.GetRequiredService<IInventoryFactory>();
            _inventoryServices = _serviceProvider.GetRequiredService<AggregatedInventoryServices>();

            _itemSplitStrategyHandler = _serviceProvider.GetRequiredService<IItemSplitStrategyHandler>();
            _itemMergeStrategyHandler = _serviceProvider.GetRequiredService<IItemMergeStrategyHandler>();

            _item = (Item) _itemFactory.CreateItem(itemMetas.First(), 1);
            _inventory = _inventoryFactory.CreateInventory(100);
        }

        protected void SetupDefaultServiceProvider()
        {
            _realMeta = _defaultRealMeta;
            _fakeMeta = _defaultFakeMeta;

            SetupServiceProvider(_defaultRealMeta, _defaultFakeMeta);

            _item = (Item) _itemFactory.CreateItem(_defaultRealMeta, 1);
            _fakeItem = (FakeItem) _itemFactory.CreateItem(_defaultFakeMeta, 1);
        }

    }
}
