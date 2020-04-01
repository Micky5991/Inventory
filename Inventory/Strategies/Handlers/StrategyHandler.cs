using System.Collections.Generic;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Strategies.Handlers
{
    public abstract class StrategyHandler<T> : IStrategyHandler<T> where T : IStrategy
    {
        private readonly List<T> _strategies = new List<T>();

        public void AddStrategy(T strategy)
        {
            _strategies.Add(strategy);
        }

        public void RemoveStrategy(T strategy)
        {
            _strategies.Remove(strategy);
        }

        public void ClearStrategies()
        {
            _strategies.Clear();
        }

        public IReadOnlyCollection<T> GetAllStrategies()
        {
            return _strategies;
        }
    }
}
