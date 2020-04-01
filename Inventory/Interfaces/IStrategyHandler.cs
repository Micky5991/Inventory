using System.Collections.Generic;

namespace Micky5991.Inventory.Interfaces
{
    public interface IStrategyHandler<T> where T : IStrategy
    {
        void AddStrategy(T strategy);
        void RemoveStrategy(T strategy);

        void ClearStrategies();

        IReadOnlyCollection<T> GetAllStrategies();
    }
}
