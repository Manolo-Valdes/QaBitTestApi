using System;

namespace QaBitTestApi.Models
{
    public class ProductAttributes
    {
        public int Id { get; set; }
        public string Color { get; set;}
        public int Size { get; set;}
        public string Manufacturer { get; set;}
        public DateTime ProductionDate { get; set; }

        public float ColorPrice { get; set; }
        public float SizePrice { get; set; }
        public float ManufacturerPrice { get; set; }
        public float ProductionDatePrice { get; set; }
    }
}
