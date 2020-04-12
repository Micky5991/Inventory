using System;
using FluentAssertions;
using Micky5991.Inventory.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Micky5991.Inventory.Tests
{
    [TestClass]
    public class OutgoingItemActionDataFixture
    {
        [TestMethod]
        public void CreatingDataSetsCorrectValues()
        {
            var guid = Guid.NewGuid();

            var createdData = new OutgoingItemActionData(guid);

            createdData.ActionRuntimeId.Should().Be(guid);
        }

        [TestMethod]
        public void OutgoingActionDataInheritsRightBaseClass()
        {
            var data = new OutgoingItemActionData(Guid.NewGuid());

            data.Should().BeAssignableTo<ItemActionData>();
        }

        [TestMethod]
        public void SetEmptyGuidThrowsException()
        {
            Action act = () => new OutgoingItemActionData(Guid.Empty);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
