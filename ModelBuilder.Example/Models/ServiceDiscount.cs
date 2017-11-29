namespace ModelBuilder.Example.Models
{
    public class ServiceDiscount
    {
        public int ID { get; set; }

        public int CompanyId { get; set; }

        public int ServiceId { get; set; }

        public float Discount { get; set; }
    }
}
