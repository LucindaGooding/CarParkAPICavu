namespace CarParkAPICavu.Models
{
    public class Pricing
    {
        public int Id { get; set; }
        public string? PricingName { get; set; }
        public decimal Price { get; set; }
        public int[]? Months { get; set; }
        //public int MonthFrom { get; set; }
        //public int MonthTo { get; set; }
    }
}
