using System;
using System.Collections.Generic;
using Micky5991.Inventory.Interfaces;
using Micky5991.Inventory.Interfaces.Strategy;

namespace Micky5991.Inventory.Strategies.Handlers
{
    /// <inheritdoc cref="IStrategyHandler{T}" />
    public abstract class StrategyHandler<T> : List<T>, IStrategyHandler<T>
        where T : IStrategy
    {
        /// <inheritdoc />
        public new void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            base.Add(item);
        }
    }
}
