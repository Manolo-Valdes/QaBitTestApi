using QaBitTestApi.Db;
using QaBitTestApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QaBitTestApi
{
    public static class ProductExtentions
    {
        public static float GetPrice(this Product product)
        {
            if (product.Attributes == null)
            {
                return product.BasePrice;
            }
            return product.BasePrice + product.Attributes.ColorPrice + product.Attributes.SizePrice
                    + product.Attributes.ManufacturerPrice + product.Attributes.ProductionDatePrice;

        }
    }
}
