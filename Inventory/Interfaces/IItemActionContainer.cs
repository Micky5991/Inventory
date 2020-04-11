using System;
using System.Collections.Generic;
using Micky5991.Inventory.Data;

namespace Micky5991.Inventory.Interfaces
{
    /// <summary>
    /// Container that holds all actions and handles incoming and outgoing data transfer.
    /// </summary>
    /// <typeparam name="TOut">Outgoing data type.</typeparam>
    /// <typeparam name="TIn">Incoming data type.</typeparam>
    public interface IItemActionContainer<TOut, TIn>
        where TOut : ItemActionData
    {
        /// <summary>
        /// Executes a certain action with given <paramref name="data"/>.
        /// </summary>
        /// <param name="actionRuntimeId">Runtime of the action to run.</param>
        /// <param name="data">Data that should be passed to the action.</param>
        void ExecuteAction(Guid actionRuntimeId, TIn data);

        /// <summary>
        /// Collects all data from all items and returns a list of data from all actions.
        /// </summary>
        /// <returns>Collection of action data of all actions in this container.</returns>
        ICollection<TOut> GetAllActionData();
    }
}
