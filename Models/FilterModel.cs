using System;

namespace QaBitTestApi.Models
{
    public class FilterModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }
        public Boolean InStock { get; set; }

        public string Color { get; set; }
        public int Size { get; set; }
        public string Manufacturer { get; set; }
        public DateTime ProductionDate { get; set; }

    }
}
