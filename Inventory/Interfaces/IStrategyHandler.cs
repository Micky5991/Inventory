using System.Collections.Generic;

namespace Micky5991.Inventory.Interfaces
{
    public interface IStrategyHandler<T> : ICollection<T> where T : IStrategy
    {

    }
}
