using System;
using System.Linq;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Item;
using Micky5991.Inventory.Enums;
using Micky5991.Inventory.Extensions;
using Micky5991.Inventory.Interfaces;
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

        protected const string ActionItemHandle = "actionhandle";
        protected const string ActionItemDisplayName = "ActionItem";
        protected const int ActionItemWeight = 50;
        protected const ItemFlags ActionItemFlags = ItemFlags.None;

        protected ItemMeta RealMeta;
        protected ItemMeta FakeMeta;
        protected ItemMeta ActionMeta;

        protected ItemMeta DefaultRealMeta;
        protected ItemMeta DefaultFakeMeta;
        protected ItemMeta DefaultActionMeta;

        protected Item Item;
        protected FakeItem FakeItem;
        protected RealActionItem ActionItem;

        protected IInventory Inventory;

        protected IServiceCollection ServiceCollection;
        protected IServiceProvider ServiceProvider;

        protected ItemRegistry ItemRegistry;
        protected AggregatedItemServices ItemServices;
        protected AggregatedInventoryServices InventoryServices;

        protected Mock<IItemFactory> ItemFactoryMock;
        protected Mock<IInventoryFactory> InventoryFactoryMock;
        protected Mock<IItemSplitStrategyHandler> ItemSplitStrategyHandlerMock;
        protected Mock<IItemMergeStrategyHandler> ItemMergeStrategyHandlerMock;

        protected IItemFactory ItemFactory;
        protected IInventoryFactory InventoryFactory;
        protected IItemMergeStrategyHandler ItemMergeStrategyHandler;
        protected IItemSplitStrategyHandler ItemSplitStrategyHandler;

        protected void SetupItemTest()
        {
            this.DefaultRealMeta = new ItemMeta(ItemHandle, typeof(RealItem), ItemDisplayName, ItemWeight, ItemFlags);
            this.DefaultFakeMeta = new ItemMeta(FakeItemHandle, typeof(FakeItem), FakeItemDisplayName, FakeItemWeight, FakeItemFlags);
            this.DefaultActionMeta = new ItemMeta(ActionItemHandle, typeof(RealActionItem), ActionItemDisplayName, ActionItemWeight, ActionItemFlags);

            this.SetupDefaultServiceProvider();
        }

        protected void SetupDependencies()
        {
            RealActionItem.Reset();

            this.ItemRegistry = new ItemRegistry();
            this.ServiceCollection = new ServiceCollection();

            this.ServiceCollection.AddInventoryServices();

            if (this.ItemFactoryMock != null)
            {
                this.ServiceCollection.AddTransient(x => this.ItemFactoryMock.Object);
            }
            else
            {
                this.ServiceCollection.AddDefaultItemFactory();
            }

            if (this.InventoryFactoryMock != null)
            {
                this.ServiceCollection.AddTransient(x => this.InventoryFactoryMock.Object);
            }
            else
            {
                this.ServiceCollection.AddDefaultInventoryFactory();
            }

            if (this.ItemSplitStrategyHandlerMock != null)
            {
                this.ServiceCollection.AddTransient(x => this.ItemSplitStrategyHandlerMock.Object);
            }
            else
            {
                this.ServiceCollection.AddDefaultInventorySplitStrategy();
            }

            if (this.ItemMergeStrategyHandlerMock != null)
            {
                this.ServiceCollection.AddTransient(x => this.ItemMergeStrategyHandlerMock.Object);
            }
            else
            {
                this.ServiceCollection.AddDefaultInventoryMergeStrategy();
            }
        }

        protected void TearDownItemTest()
        {
            this.ServiceProvider = null;
            this.ServiceCollection = null;
            this.ItemFactory = null;
            this.ItemRegistry = null;

            this.RealMeta = null;
            this.FakeMeta = null;
            this.ActionMeta = null;

            this.Item = null;
            this.FakeItem = null;
            this.Inventory = null;

            this.InventoryFactoryMock = null;
            this.ItemFactoryMock = null;
            this.ItemMergeStrategyHandlerMock = null;
            this.ItemSplitStrategyHandlerMock = null;
        }

        protected void SetupServiceProvider(params ItemMeta[] itemMetas)
        {
            this.SetupDependencies();

            foreach (var itemMeta in itemMetas)
            {
                this.ItemRegistry.AddItemMeta(itemMeta);
            }

            this.ServiceCollection.AddItemTypes(this.ItemRegistry);
            this.ServiceProvider = this.ServiceCollection.BuildServiceProvider();

            this.ItemFactory = this.ServiceProvider.GetRequiredService<IItemFactory>();
            this.ItemServices = this.ServiceProvider.GetRequiredService<AggregatedItemServices>();

            this.InventoryFactory = this.ServiceProvider.GetRequiredService<IInventoryFactory>();
            this.InventoryServices = this.ServiceProvider.GetRequiredService<AggregatedInventoryServices>();

            this.ItemSplitStrategyHandler = this.ServiceProvider.GetRequiredService<IItemSplitStrategyHandler>();
            this.ItemMergeStrategyHandler = this.ServiceProvider.GetRequiredService<IItemMergeStrategyHandler>();

            this.Item = (Item) this.ItemFactory.CreateItem(itemMetas.First(), 1);
            this.Inventory = this.InventoryFactory.CreateInventory(100);
        }

        protected void SetupDefaultServiceProvider()
        {
            this.RealMeta = this.DefaultRealMeta;
            this.FakeMeta = this.DefaultFakeMeta;
            this.ActionMeta = this.DefaultActionMeta;

            this.SetupServiceProvider(this.DefaultRealMeta, this.DefaultFakeMeta, this.DefaultActionMeta);

            this.Item = (Item) this.ItemFactory.CreateItem(this.DefaultRealMeta, 1);
            this.FakeItem = (FakeItem) this.ItemFactory.CreateItem(this.DefaultFakeMeta, 1);
            this.ActionItem = (RealActionItem) this.ItemFactory.CreateItem(this.DefaultActionMeta, 1);
        }

    }
}
