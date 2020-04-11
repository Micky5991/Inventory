using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Micky5991.Inventory.AggregatedServices;
using Micky5991.Inventory.Entities.Factories;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Extensions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class InventoryDependencyExtensionFixture
    {

        private IServiceCollection serviceCollection;
        private IServiceProvider serviceProvider;

        private Mock<IItemRegistry> itemRegistryMock;
        private ItemRegistry itemRegistry;

        [TestInitialize]
        public void Setup()
        {
            this.serviceCollection = new ServiceCollection();

            this.itemRegistryMock = new Mock<IItemRegistry>();
            this.itemRegistry = new ItemRegistry();
        }

        [TestCleanup]
        public void Teardown()
        {
            this.serviceCollection = null;
            this.serviceProvider = null;
        }

        [TestMethod]
        public void RunningAddInventoryServicesOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddInventoryServices(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultInventoryStrategiesOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultInventoryStrategies(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultInventoryMergeStrategyOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultInventoryMergeStrategy(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultInventorySplitStrategyOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultInventorySplitStrategy(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultInventoryFactoryOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultInventoryFactory(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultItemFactoryOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultItemFactory(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddDefaultFactoriesOnNullThrowsException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultFactories(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void RunningAddServiceExtensionsOnNullThrowsArgumentNullException()
        {
            Action act = () => InventoryDependencyExtensions.AddDefaultInventoryServices(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void AddServicesExtensionRegistersNeededServices()
        {
            InventoryDependencyExtensions.AddDefaultInventoryServices(this.serviceCollection);

            this.AddItemRegistryMock();
            this.BuildServiceProvider();

            this.serviceProvider.GetRequiredService<IInventoryFactory>()
                .Should().BeOfType<InventoryFactory>();

            this.serviceProvider.GetRequiredService<IItemFactory>()
                .Should().BeOfType<ItemFactory>();
        }

        [TestMethod]
        public void AddingItemRegistryOnNullServiceCollectionWillThrowException()
        {
            Action act = () => InventoryDependencyExtensions.AddItemTypes(null, this.itemRegistryMock.Object);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void AddingNullRegistryToServiceCollectionWillThrowException()
        {
            Action act = () => InventoryDependencyExtensions.AddItemTypes(this.serviceCollection, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void AddItemTypesRegistersItemRegistry()
        {
            InventoryDependencyExtensions.AddItemTypes(this.serviceCollection, this.itemRegistry);

            this.BuildServiceProvider();

            this.serviceProvider.GetRequiredService<IItemRegistry>().Should().Be(this.itemRegistry);
        }

        [TestMethod]
        public void ReturningNullOnGetItemMetaWillThrowInvalidItemRegistryException()
        {
            this.itemRegistryMock
                .Setup(x => x.GetItemMeta())
                .Returns<IEnumerable<ItemMeta>>(null);

            Action act = () => InventoryDependencyExtensions.AddItemTypes(this.serviceCollection, this.itemRegistryMock.Object);

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x => x.Message.Contains("returns") && x.Message.Contains("null"));
        }

        [TestMethod]
        public void RegisteredItemsInItemRegistryWillBeResolvable()
        {
            var meta = this.itemRegistry.CreateItemMetaForward<FakeItem>("item", "Fake Item");

            this.SetupRegistryWithItems(meta);

            InventoryDependencyExtensions.AddItemTypes(this.serviceCollection, this.itemRegistry);

            this.BuildServiceProvider();

            var factory = this.serviceProvider.GetRequiredService(typeof(FakeItem));

            factory.Should()
                .NotBeNull()
                .And.BeOfType<ObjectFactory>();

            var resolvedItem = ((ObjectFactory) factory)(this.serviceProvider, new [] { meta });

            resolvedItem.Should()
                .NotBeNull()
                .And.BeOfType<FakeItem>();
        }

        [TestMethod]
        public void ReturningNullAsValueInItemRegistryShouldThrowException()
        {
            this.itemRegistryMock.Setup(x => x.GetItemMeta()).Returns(new ItemMeta[] {null});

            Action act = () => InventoryDependencyExtensions.AddItemTypes(this.serviceCollection, this.itemRegistryMock.Object);

            act.Should().Throw<InvalidItemRegistryException>()
                .Where(x =>
                    x.Message.Contains(nameof(IItemRegistry.GetItemMeta))
                    && x.Message.Contains("contains")
                    && x.Message.Contains("null")
                    && x.Message.Contains("value"));
        }

        [TestMethod]
        public void GetItemMetaWillBeCalledOnce()
        {
            this.itemRegistryMock.Setup(x => x.GetItemMeta()).Returns(new ItemMeta[0]);

            InventoryDependencyExtensions.AddItemTypes(this.serviceCollection, this.itemRegistryMock.Object);

            this.itemRegistryMock.Verify(x => x.GetItemMeta(), Times.Once);
        }

        private void BuildServiceProvider()
        {
            this.serviceProvider = this.serviceCollection.BuildServiceProvider();
        }

        private void AddItemRegistryMock()
        {
            this.serviceCollection.AddTransient(x => this.itemRegistryMock.Object);
        }

        private void SetupRegistryWithItems(params ItemMeta[] metas)
        {
            foreach (var meta in metas)
            {
                this.serviceCollection.AddTransient(meta.Type, x => meta);

                this.itemRegistry.AddItemMeta(meta);
            }
        }

    }
}
