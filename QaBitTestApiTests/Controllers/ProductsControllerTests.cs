using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using QaBitTestApi.Db;
using QaBitTestApi.Models;
using System.Linq;

namespace QaBitTestApi.Controllers.Tests
{
    [TestClass()]
    public class ProductsControllerTests
    {
        [TestMethod()]
        public void SuscribeToNotificationListTest()
        {
            ServiceProvider serviceProvider = GetServiceProvider();
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var logger = serviceScope.ServiceProvider.GetService<ILogger<ProductsController>>();
                ApiDbContext dbContext = serviceScope.ServiceProvider.GetService<ApiDbContext>();
                var controler = new ProductsController(logger, dbContext);
                 IActionResult result = controler.SuscribeToNotificationList(1, new ClientSuscriptionRequest() { EMail="user@domain", Name="John Smish" , RequestedProductAmount=2});
                Assert.IsNotNull(result);
            }
        }

        [TestMethod()]
        public void PutProductTest()
        {
            ServiceProvider serviceProvider = GetServiceProvider();
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var logger = serviceScope.ServiceProvider.GetService<ILogger<ProductsController>>();
                ApiDbContext dbContext = serviceScope.ServiceProvider.GetService<ApiDbContext>();
                var controler = new ProductsController(logger, dbContext);
                IActionResult result = controler.Put(1, "{\"Stock\":4,\"Attributes\":{\"Color\":\"blue\"}}");
                Assert.IsNotNull(result);
            }
        }


        [TestMethod()]
        public void ProductFilterTest()
        {
            ServiceProvider serviceProvider = GetServiceProvider();
            using (var serviceScope = serviceProvider.CreateScope())
            {
                ApiDbContext dbContext = serviceScope.ServiceProvider.GetService<ApiDbContext>();
                var filter = new FilterModel() {
                InStock = true
                };
                var task = dbContext.ApplyFilterAsync(filter);
                task.Wait();
                var result = task.Result;
                Assert.IsTrue(Enumerable.Count(result) == 1);


                filter = new FilterModel()
                {
                    Name = "smart",
                    Description = "Coka Cola",
                };
                task = dbContext.ApplyFilterAsync(filter);
                task.Wait();
                result = task.Result;
                Assert.IsTrue(Enumerable.Count(result) == 2);
            }
        }



        private static ServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddDbContext<ApiDbContext>(options =>
            {
                options.UseInMemoryDatabase("data");
                options.EnableSensitiveDataLogging();
            });
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();

            using (var serviceScope = serviceProvider.CreateScope())
            {
                ApiDbContext dbContext = serviceScope.ServiceProvider.GetService<ApiDbContext>();
                dbContext.Database.EnsureCreated();
                dbContext.PopulateDb();
            }

            return serviceProvider;
        }
    }
}