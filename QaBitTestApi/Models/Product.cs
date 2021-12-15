using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QaBitTestApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get;set;}
        public string Description { get; set; }
        public ProductAttributes Attributes { get; set;}
        public float BasePrice { get; set; }
        public int Stock { get; set; }
    }
}
