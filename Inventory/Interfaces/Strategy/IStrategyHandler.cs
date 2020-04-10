using System.Collections.Generic;

namespace Micky5991.Inventory.Interfaces.Strategy
{
    public interface IStrategyHandler<T> : ICollection<T>
        where T : IStrategy
    {
    }
}
