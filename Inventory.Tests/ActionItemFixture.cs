using System;
using System.Collections.Generic;
using FluentAssertions;
using Micky5991.Inventory.Data;
using Micky5991.Inventory.Entities.Item;
using Micky5991.Inventory.Entities.Item.Subtypes;
using Micky5991.Inventory.Exceptions;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class ActionItemFixture : ItemTest
    {

        [TestInitialize]
        public void Setup()
        {
            this.SetupItemTest();
        }

        [TestCleanup]
        public void Teardown()
        {
            this.TearDownItemTest();
        }

        [TestMethod]
        public void ActionItemImplementsInterface()
        {
            this.ActionItem.Should()
                .BeAssignableTo<IActionableItem<OutgoingItemActionData, IncomingItemActionData>>()
                .And.BeAssignableTo<ActionableItem<OutgoingItemActionData, IncomingItemActionData>>()
                .And.BeAssignableTo<IItem>()
                .And.BeAssignableTo<Item>();
        }

        [TestMethod]
        public void PassingNullToExecuteInActionItemThrowsException()
        {
            Action act = () => this.ActionItem.ExecuteAction(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void PassingActionDataWithUnknownActionRuntimeIdThrowsException()
        {
            Action act = () => this.ActionItem.ExecuteAction(new IncomingItemActionData(Guid.NewGuid()));

            act.Should().Throw<ItemActionNotFoundException>();
        }

        [TestMethod]
        public void PassingActionDataWithEmptyActionRuntimeIdThrowsException()
        {
            Action act = () => this.ActionItem.ExecuteAction(new IncomingItemActionData(Guid.Empty));

            act.Should().Throw<ItemActionNotFoundException>();
        }

        [TestMethod]
        public void ReturningNullInRegisterActionInItemThrowsException()
        {
            RealActionItem.ActionGenerator = null;

            Action act = () => this.ItemFactory.CreateItem(ActionItemHandle, 1);

            act.Should().Throw<InvalidActionException>()
               .Where(x => x.Message.Contains("returned null"));
        }

        [TestMethod]
        public void ReturningListWithNullValueThrowsException()
        {
            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                null
            };

            Action act = () => this.ItemFactory.CreateItem(ActionItemHandle, 1);

            act.Should().Throw<InvalidActionException>()
               .Where(x => x.Message.Contains("contained null"));
        }

        [TestMethod]
        public void ReturningListWithActionWithEmptyRuntimeIdValueThrowsException()
        {
            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                new RealAction(Guid.Empty)
            };

            Action act = () => this.ItemFactory.CreateItem(ActionItemHandle, 1);

            act.Should().Throw<InvalidActionException>()
               .Where(x => x.Message.Contains("empty guid"));
        }

        [TestMethod]
        public void AddingCorrectActionsRegistersItems()
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            var data = new IncomingItemActionData(action.RuntimeId);

            item.ExecuteAction(data);

            action.PassedActionData.Should().Be(data);
        }

        [TestMethod]
        public void ActionItemReturnsAllCreatedData()
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            var data = new OutgoingItemActionData(action.RuntimeId);

            action.ActionDataBuilder = () => data;

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.GetAllActionData().Should().ContainSingle(x => x == data);
        }

        [TestMethod]
        public void ActionItemReturnsAllCreatedMultipleData()
        {
            var action = new RealAction();
            var otherAction = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
                otherAction
            };

            var data = new OutgoingItemActionData(action.RuntimeId);
            var otherData = new OutgoingItemActionData(otherAction.RuntimeId);

            action.ActionDataBuilder = () => data;
            otherAction.ActionDataBuilder = () => otherData;

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);
            item.GetAllActionData().Should().OnlyContain(x => x == data || x == otherData);
        }

        [TestMethod]
        public void ActionItemReturnsNonNullCreatedMultipleData()
        {
            var action = new RealAction();
            var otherAction = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
                otherAction
            };

            var data = new OutgoingItemActionData(action.RuntimeId);

            action.ActionDataBuilder = () => data;
            otherAction.ActionDataBuilder = () => null;

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.GetAllActionData().Should().Equal(new List<OutgoingItemActionData>
            {
                data,
            });
        }

        [TestMethod]
        public void ActionItemWillOnlyReturnVisibleItems()
        {
            var action = new RealAction();
            var otherAction = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
                otherAction
            };

            var data = new OutgoingItemActionData(action.RuntimeId);
            var otherData = new OutgoingItemActionData(otherAction.RuntimeId);

            action.ActionDataBuilder = () => data;
            otherAction.ActionDataBuilder = () => otherData;

            otherAction.SetVisibleCheck(() => false);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);
            item.GetAllActionData().Should().Equal(new List<OutgoingItemActionData>
            {
                data,
            });
        }

        [TestMethod]
        public void ActionItemWillOnlyReturnVisibleItemsWithPositiveCheck()
        {
            var action = new RealAction();
            var otherAction = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
                otherAction
            };

            var data = new OutgoingItemActionData(action.RuntimeId);
            var otherData = new OutgoingItemActionData(otherAction.RuntimeId);

            action.ActionDataBuilder = () => data;
            otherAction.ActionDataBuilder = () => otherData;

            otherAction.SetVisibleCheck(() => true);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);
            item.GetAllActionData().Should().OnlyContain(x => x == data || x == otherData);
        }

        [TestMethod]
        public void PassingNullAsVisibleCheckCountsAsVisible()
        {
            var action = new RealAction();
            var otherAction = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
                otherAction
            };

            var data = new OutgoingItemActionData(action.RuntimeId);
            var otherData = new OutgoingItemActionData(otherAction.RuntimeId);

            action.ActionDataBuilder = () => data;
            otherAction.ActionDataBuilder = () => otherData;

            otherAction.SetVisibleCheck(() => false);
            otherAction.SetVisibleCheck(null);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);
            item.GetAllActionData().Should().OnlyContain(x => x == data || x == otherData);
        }

        [TestMethod]
        public void DisablingItemKeepsActionDataInResult()
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            var data = new OutgoingItemActionData(action.RuntimeId);

            action.ActionDataBuilder = () => data;

            action.SetVisibleCheck(() => true);
            action.SetEnabledCheck(() => false);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);
            item.GetAllActionData().Should().ContainSingle(x => x == data);
        }
    }
}
