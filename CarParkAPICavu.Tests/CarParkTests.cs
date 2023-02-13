using CarParkAPICavu.Controllers;
using CarParkAPICavu.Models;
using Moq;

namespace CarParkAPICavu.Tests
{
    public class Tests
    {
        //private readonly CarParkController _carParkController;
        //private readonly CarParkContext _carParkContext;
        
        [SetUp]
        public void Setup()
        {
            //_carParkContext = new CarParkContext();
            //_carParkController = new CarParkController(_carParkContext);

        }

        [Test]
        public void Test1()
        {
            var dbContext = new Mock<CarParkContext>();
            var controller = new CarParkController(dbContext);


            Assert.Pass();



            var cars = new CarPark[] {
                new CarPark
                {
                    Id = 1,
                    CarReg = "QW22JKL",
                    Price = 210,
                    DateFrom = DateTime.Now,
                    DateTo = DateTime.Now.AddDays(7)
                }
            };

            var mockContext = new Mock<CarParkContext>();
            mockContext.Setup(x => x.Cars).ReturnsDbSet(cars);

            var manager = new DbShoppingCartManager(mockContext.Object);

            var items = manager.GetCart();

            Assert.AreEqual(initialItems, items);

        }
    }
}