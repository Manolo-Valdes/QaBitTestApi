using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QaBitTestApi.Db;
using QaBitTestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QaBitTestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ApiDbContext _dbContext;

        public ProductsController(ILogger<ProductsController> logger, ApiDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        // GET api/Products
        [HttpGet]
        public async Task<IEnumerable<Product>> GetAsync([FromQuery] FilterModel filter)
        {
            _logger.LogInformation("quering products according user filter..");
            return await _dbContext.ApplyFilterAsync(filter);
        }

        // POST api/Products
        [HttpPost]
        public IActionResult Post([FromBody] Product value)
        {
            if (!TryValidateModel(value))
            {
                _logger.LogError("Invalid product format.");
                return BadRequest();
            }
            _logger.LogInformation("Adding new product to db");
            _dbContext.Products.Add(value);
            _dbContext.SaveChanges();
            return Ok(value);
        }

        // PUT api/Products/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string values)
        {
            _logger.LogInformation("Editing product..");
            var product = _dbContext.Products.Include(p=> p.Attributes).First(a => a.Id == id);
            JsonConvert.PopulateObject(values, product);

            if (!TryValidateModel(product))
                return BadRequest();
            _dbContext.SaveChanges();
            return Ok();
        }

        // DELETE api/Products/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _logger.LogInformation("Deleting Product..");
            var product = _dbContext.Products.First(a => a.Id == id);
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Suscribe to notification List
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public IActionResult SuscribeToNotificationList(int id,[FromBody] ClientSuscriptionRequest value)
        {
            var product = _dbContext.Products.First(a => a.Id == id);
            if (product == null)
            {
                _logger.LogInformation("Requested Product not found");
                return NoContent();
            }
            _logger.LogInformation("Suscribing to Notification list for Product {0}", product.Name);
            _dbContext.ClientSuscriptions.Add(new ClientSuscription() { RequestedProductID =id, Name = value.Name , EMail = value.EMail , RequestedProductAmount = value.RequestedProductAmount });
            _dbContext.SaveChanges();
            return Ok();
        }
    }
}
