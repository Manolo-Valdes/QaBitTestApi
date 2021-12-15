using Microsoft.EntityFrameworkCore;
using QaBitTestApi.Db;
using QaBitTestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace QaBitTestApi
{
    public static class AppExtentions
    {
        public static void PopulateDb(this ApiDbContext dbContext)
        {
            if (dbContext.Products.Any())
            {
                return;
            }
            var p1 = new Product() {
                 Name = "Smart TV",
                 Description = "52 inches HD SANSUNG Smart TV",
                 BasePrice = 430.99f,
                 Stock=10,
                 Attributes = new ProductAttributes()
                 {
                     Color ="Black",
                     Manufacturer="SANSUNG",
                     ManufacturerPrice=30f,
                     ProductionDate = new DateTime(2020,3,1),
                     Size= 52,
                     SizePrice=25f
                 }
            };
            dbContext.Products.Add(p1);

            var p2 = new Product()
            {
                Name = "Coka Cola",
                Description = "1.5l Coka Cola",
                BasePrice = 2.99f,
                Stock = 0,
                Attributes = new ProductAttributes()
                {
                    Color = "Black",
                    Manufacturer = "Coka Cola",
                    ProductionDate = new DateTime(2021, 3, 1),
                }
            };
            dbContext.Products.Add(p2);

            dbContext.SaveChanges();
        }

        public static async Task<IEnumerable<Product>> ApplyFilterAsync(this ApiDbContext dbContext, FilterModel filter)
        {
            var name = filter.Name ?? string.Empty;
            var desc = filter.Description ?? string.Empty;
            var color = filter.Color ?? string.Empty;
            var manufacturer = filter.Manufacturer ?? string.Empty;
            var minPrice = filter.MinPrice;
            var maxPrice = filter.MaxPrice > filter.MinPrice ? filter.MaxPrice : float.MaxValue;
            var date = filter.ProductionDate;
            var inStock = filter.InStock;

            var param = Expression.Parameter(typeof(Product), "p");
            Expression left = null;

            Type stringtype = typeof(String);
            MethodInfo MethodContains = stringtype.GetMethods()
              .First(m => m.ReturnType == typeof(bool) && m.Name == nameof(String.Contains)
              && m.GetParameters().Length == 2);


            if (!string.IsNullOrEmpty(name))
            {
                left = Expression.Call(Expression.Property(param, nameof(Product.Name)), MethodContains,new Expression[] { Expression.Constant(name, stringtype), Expression.Constant( StringComparison.OrdinalIgnoreCase) });
            }
            if (!string.IsNullOrEmpty(desc))
            {
                var right = Expression.Call(Expression.Property(param, nameof(Product.Description)), MethodContains, new Expression[] { Expression.Constant(desc, stringtype), Expression.Constant(StringComparison.OrdinalIgnoreCase) });

                if (left!=null)
                {
                    left = Expression.Or(left, right);
                }
                else
                {
                    left = right;
                }
            }
            if (inStock)
            {
                var right = Expression.GreaterThan(Expression.Property(param, nameof(Product.Stock)), Expression.Constant(0));
                if (left != null)
                {
                    left = Expression.AndAlso(left, right);
                }
                else
                {
                    left = right;
                }
            }

            var attribute = Expression.Property(param, nameof(Product.Attributes));
            if (!string.IsNullOrEmpty(color))
            {
                var right = Expression.Call(Expression.Property(attribute, nameof(ProductAttributes.Color)), MethodContains, new Expression[] { Expression.Constant(color, stringtype), Expression.Constant(StringComparison.OrdinalIgnoreCase) });
                if (left != null)
                {
                    left = Expression.AndAlso(left, right);
                }
                else
                {
                    left = right;
                }
            }
            if (!string.IsNullOrEmpty(manufacturer))
            {
                var right = Expression.Call(Expression.Property(attribute, nameof(ProductAttributes.Manufacturer)), MethodContains, new Expression[] { Expression.Constant(manufacturer, stringtype), Expression.Constant(StringComparison.OrdinalIgnoreCase) });
                if (left != null)
                {
                    left = Expression.AndAlso(left, right);
                }
                else
                {
                    left = right;
                }
            }
            if (date > DateTime.MinValue)
            {
                var right = Expression.GreaterThanOrEqual(Expression.Property(attribute, nameof(ProductAttributes.ProductionDate)),Expression.Constant(date));
                if (left != null)
                {
                    left = Expression.AndAlso(left, right);
                }
                else
                {
                    left = right;
                }
            }

            var selection = Enumerable.Empty<Product>();
            if (left != null)
            {
                var exp = Expression.Lambda<Func<Product, bool>>(left, param);
                selection = await dbContext.Products.Include(p => p.Attributes).Where(exp).ToListAsync();
            }
            else
            {
                selection = await dbContext.Products.Include(p => p.Attributes).ToListAsync();
            }



            /*
             * Como el precio del producto varia según los atributos,
             * se aplica ese filtro fuera de la consulta SQL generada por EntityFramework
             * 
             */
            if (minPrice == 0 && maxPrice == float.MaxValue)
            {
                // no se establecieron criterios de selección de precios
                return selection;
            }
            return selection.Where(p => {
                var price = p.GetPrice();
                return price >= minPrice & price <= maxPrice;
            });

        }
    }
}
