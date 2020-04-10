using System.Collections.Generic;

namespace Micky5991.Inventory.Interfaces.Strategy
{
    /// <summary>
    /// Handler that executes all added <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Implementation of <see cref="IStrategy"/> that should be handled.</typeparam>
    public interface IStrategyHandler<T> : ICollection<T>
        where T : IStrategy
    {
    }
}
