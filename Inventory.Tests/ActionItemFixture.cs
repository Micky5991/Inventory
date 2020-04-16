using System;
using System.Collections.Generic;
using FluentAssertions;
using Micky5991.Inventory.Data;
using Micky5991.Inventory.Entities.Item;
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
            Action act = () => this.ActionItem.ExecuteAction(new object(), null);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void PassingActionDataWithUnknownActionRuntimeIdThrowsException()
        {
            var actionId = Guid.NewGuid();

            Action act = () => this.ActionItem.ExecuteAction(new object(), new IncomingItemActionData(actionId));

            act.Should().Throw<ItemActionNotFoundException>()
               .Where(x => x.Message.Contains("not find") && x.Message.Contains(actionId.ToString()));
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

            item.ExecuteAction(new object(), data);

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

            action.ActionDataBuilder = x => data;

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.GetAllActionData(new object()).Should().ContainSingle(x => x == data);
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

            action.ActionDataBuilder = x => data;
            otherAction.ActionDataBuilder = x => otherData;

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);
            item.GetAllActionData(new object()).Should().OnlyContain(x => x == data || x == otherData);
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

            action.ActionDataBuilder = x => data;
            otherAction.ActionDataBuilder = x => null;

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.GetAllActionData(new object()).Should().Equal(new List<OutgoingItemActionData>
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

            action.ActionDataBuilder = x => data;
            otherAction.ActionDataBuilder = x => otherData;

            otherAction.SetVisibleCheck((x) => false);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);
            item.GetAllActionData(new object()).Should().Equal(new List<OutgoingItemActionData>
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

            action.ActionDataBuilder = x => data;
            otherAction.ActionDataBuilder = x => otherData;

            otherAction.SetVisibleCheck(x => true);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);
            item.GetAllActionData(new object()).Should().OnlyContain(x => x == data || x == otherData);
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

            action.ActionDataBuilder = x => data;
            otherAction.ActionDataBuilder = x => otherData;

            otherAction.SetVisibleCheck(x => false);
            otherAction.SetVisibleCheck(null);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);
            item.GetAllActionData(new object()).Should().OnlyContain(x => x == data || x == otherData);
        }

        [TestMethod]
        public void SettingVisibleToNullExecutesAction()
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            action.SetVisibleCheck(null);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.ExecuteAction(null, new IncomingItemActionData(action.RuntimeId));

            action.ExecutedAmount.Should().Be(1);
        }

        [TestMethod]
        public void SettingEnabledToNullExecutesAction()
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            action.SetEnabledCheck(null);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.ExecuteAction(null, new IncomingItemActionData(action.RuntimeId));

            action.ExecutedAmount.Should().Be(1);
        }

        [TestMethod]
        public void SettingBothChecksToNullExecutesAction()
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            action.SetVisibleCheck(null);
            action.SetEnabledCheck(null);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.ExecuteAction(null, new IncomingItemActionData(action.RuntimeId));

            action.ExecutedAmount.Should().Be(1);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, true)]
        public void VisibleStatusWillBeRespectedAndOverwritesEnabled(bool visible, bool shouldSucceed)
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            action.SetVisibleCheck(x => visible);
            action.SetEnabledCheck(x => true);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.ExecuteAction(null, new IncomingItemActionData(action.RuntimeId));

            action.ExecutedAmount.Should().Be(shouldSucceed ? 1 : 0);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, true)]
        public void EnabledStatusWillBeRespectedAndAppliesWhenVisibleIsTrue(bool enabled, bool shouldSucceed)
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            action.SetVisibleCheck(x => true);
            action.SetEnabledCheck(x => enabled);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.ExecuteAction(null, new IncomingItemActionData(action.RuntimeId));

            action.ExecutedAmount.Should().Be(shouldSucceed ? 1 : 0);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, true)]
        public void VisibleAndEnabledSetToSameValueWillOnlyTriggerIfBothTrue(bool enabled, bool shouldSucceed)
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            action.SetVisibleCheck(x => enabled);
            action.SetEnabledCheck(x => enabled);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.ExecuteAction(null, new IncomingItemActionData(action.RuntimeId));

            action.ExecutedAmount.Should().Be(shouldSucceed ? 1 : 0);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, true)]
        public void SettingVisibleToNullRespectsEnabledStatus(bool enabled, bool shouldSucceed)
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            action.SetVisibleCheck(null);
            action.SetEnabledCheck(x => enabled);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.ExecuteAction(null, new IncomingItemActionData(action.RuntimeId));

            action.ExecutedAmount.Should().Be(shouldSucceed ? 1 : 0);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, true)]
        public void SettingEnabledToNullRespectsVisibleStatus(bool visible, bool shouldSucceed)
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            action.SetVisibleCheck(x => visible);
            action.SetEnabledCheck(null);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.ExecuteAction(null, new IncomingItemActionData(action.RuntimeId));

            action.ExecutedAmount.Should().Be(shouldSucceed ? 1 : 0);
        }

        [TestMethod]
        public void PassingObjectPassesRightObjectToActionDataFactory()
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            object receivedObject = null;

            action.ActionDataBuilder = x =>
            {
                receivedObject = x;

                return null;
            };

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            var passedObject = new object();

            item.GetAllActionData(passedObject);

            receivedObject.Should().NotBeNull().And.Be(passedObject);
            action.PassedReceiver.Should().NotBeNull().And.Be(passedObject);
        }

        [TestMethod]
        public void PassingNullPassesNullToActionDataFactory()
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            var receivedObject = new object();

            action.ActionDataBuilder = x =>
            {
                receivedObject = x;

                return null;
            };

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.GetAllActionData(null);

            receivedObject.Should().BeNull();
            action.PassedReceiver.Should().BeNull();
        }

        [TestMethod]
        public void PassingExecutorToExecutePassesRightObject()
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            var passedObject = new object();

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.ExecuteAction(passedObject, new IncomingItemActionData(action.RuntimeId));

            action.PassedExecutor.Should().NotBeNull().And.Be(passedObject);
        }

        [TestMethod]
        public void PassingNullAsExecutorToExecutePassesNullToAction()
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            item.ExecuteAction(null, new IncomingItemActionData(action.RuntimeId));

            action.PassedExecutor.Should().BeNull();
        }

        [TestMethod]
        public void ExecutingActionChecksVisibilityWithRightExecutor()
        {
            var action = new RealAction();

            RealActionItem.ActionGenerator = () => new List<IItemAction<OutgoingItemActionData, IncomingItemActionData>>
            {
                action,
            };

            object visibleReceivedObject = null;
            object enabledReceivedObject = null;

            action.SetVisibleCheck(x =>
            {
                visibleReceivedObject = x;

                return true;
            });

            action.SetEnabledCheck(x =>
            {
                enabledReceivedObject = x;

                return true;
            });

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);

            var passedObject = new object();

            item.ExecuteAction(passedObject, new IncomingItemActionData(action.RuntimeId));

            visibleReceivedObject.Should().NotBeNull().And.Be(passedObject);
            enabledReceivedObject.Should().NotBeNull().And.Be(passedObject);
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

            action.ActionDataBuilder = x => data;

            action.SetVisibleCheck(x => true);
            action.SetEnabledCheck(x => false);

            var item = (RealActionItem) this.ItemFactory.CreateItem(ActionItemHandle, 1);
            item.GetAllActionData(new object()).Should().ContainSingle(x => x == data);
        }
    }
}
