using System;
using Micky5991.Inventory.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public Program()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddInventoryServices()
                .AddItemTypes(new ItemRegistry());
        }
    }
}
