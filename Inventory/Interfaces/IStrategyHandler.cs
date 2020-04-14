using System.Collections.Generic;
using JetBrains.Annotations;

namespace Micky5991.Inventory.Interfaces
{
    /// <summary>
    /// Handler that executes all added <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Implementation of <see cref="IStrategy"/> that should be handled.</typeparam>
    [PublicAPI]
    public interface IStrategyHandler<T> : ICollection<T>
        where T : IStrategy
    {
    }
}
