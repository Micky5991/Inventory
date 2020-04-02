using System;
using System.Collections.Generic;
using Micky5991.Inventory.Interfaces;

namespace Micky5991.Inventory.Strategies.Handlers
{
    public abstract class StrategyHandler<T> : List<T>, IStrategyHandler<T> where T : IStrategy
    {
    }
}
