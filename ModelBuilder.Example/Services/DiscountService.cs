using System;
using System.Collections.Generic;
using System.Linq;
using ModelBuilder.Example.Models;

namespace ModelBuilder.Example.Services
{
    public static class DiscountService
    {
        private static readonly IList<Company> _dummyCompanies;
        private static readonly IList<Service> _dummyServices;
        private static readonly IList<ServiceDiscount> _dummyDiscounts;

        static DiscountService()
        {
            _dummyCompanies = new List<Company>
            {
                new Company { ID = 1, Name = "Company 1" },
                new Company { ID = 2, Name = "Company 2" }
            };

            _dummyServices = new List<Service>
            {
                new Service { ID = 1, Name = "Service 1" },
                new Service { ID = 2, Name = "Service 2" }
            };

            _dummyDiscounts = new List<ServiceDiscount>
            {
                new ServiceDiscount { ID = 1, CompanyId = 1, ServiceId = 1, Discount = 10f },
                new ServiceDiscount { ID = 2, CompanyId = 1, ServiceId = 2, Discount = 20f },
                new ServiceDiscount { ID = 3, CompanyId = 2, ServiceId = 1, Discount = 40f }
            };
        }

        public static IEnumerable<dynamic> ListServicesDiscounts()
        {
            var services = _dummyServices.Select(service => new
            {
                service.ID,
                service.Name,
                PropName = service.Name.ToLower()
            });

            var companies = _dummyCompanies.Select(company => new
            {
                company.ID,
                company.Name,
                Discounts = from service in services
                            join discount in _dummyDiscounts on new
                            {
                                serviceId = service.ID,
                                companyId = company.ID
                            }
                            equals new
                            {
                                serviceId = discount.ServiceId,
                                companyId = discount.CompanyId
                            }
                            into discountJoin
                            from discount in discountJoin.DefaultIfEmpty()
                            select new
                            {
                                ServiceName = service.Name,
                                service.PropName,
                                Discount = discount != null ? discount.Discount : 0
                            }
            });

            // Combine properties into a dictionary:
            var properties = new Dictionary<string, Type>
            {
                { "companyID", typeof(int) },
                { "companyName", typeof(string) }
            };

            foreach (var item in services)
                properties.Add(item.PropName, typeof(float));

            // Create a new class:
            var rowType = CustomTypeBuilder.CompileResultType("ServicesDiscountsRow", properties);

            // Create a table representation based on dynamic objects:
            var servicesDiscounts = companies.Select(company =>
            {
                var obj = Activator.CreateInstance(rowType.AsType());

                var companyID = rowType.GetProperty("companyID");
                companyID.SetValue(obj, company.ID, null);

                var companyName = rowType.GetProperty("companyName");
                companyName.SetValue(obj, company.Name, null);

                foreach (var discount in company.Discounts)
                {
                    var propName = discount.PropName;
                    var prop = rowType.GetProperty(propName);
                    prop.SetValue(obj, discount.Discount, null);
                }

                return obj;
            });

            return servicesDiscounts;
        }
    }
}
