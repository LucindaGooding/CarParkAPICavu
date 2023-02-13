using System.ComponentModel.DataAnnotations;

namespace CarParkAPICavu.Models
{
    public class CarPark
    {
        public int Id { get; set; }
        public string? CarReg { get; set; }
        public decimal Price { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

    }
}
