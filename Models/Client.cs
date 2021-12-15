namespace QaBitTestApi.Models
{
    public class ClientSuscription
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public int RequestedProductID { get; set; }
        public int RequestedProductAmount { get; set; }
    }

    public class ClientSuscriptionRequest
    {
        public string Name { get; set; }
        public string EMail { get; set; }
        public int RequestedProductAmount { get; set; }

    }
}
