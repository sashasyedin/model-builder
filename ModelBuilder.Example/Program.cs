using System;
using ModelBuilder.Example.Services;
using Newtonsoft.Json;

namespace ModelBuilder.Example
{
    sealed class Program
    {
        static void Main(string[] args)
        {
            var discounts = DiscountService.ListServicesDiscounts();
            var json = JsonConvert.SerializeObject(discounts, Formatting.Indented);

            Console.WriteLine(json);
            Console.ReadKey();
        }
    }
}
