using CarParkAPICavu.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarParkAPICavu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarParkController : ControllerBase
    {
        private readonly CarParkContext _dbContext;
        private readonly int _carParkMaxCapacity;
        private readonly Pricing _summerMonthsPrice;
        private readonly Pricing _winterMonthsPrice;

        public CarParkController(CarParkContext dbContext)
        {
            _dbContext = dbContext;
            _carParkMaxCapacity = 10;
            _summerMonthsPrice = new Pricing()
            {
                Id = 1,
                PricingName = "Summer Months",
                Price = 30,
                Months = new int[6] { 4, 5, 6, 7, 8, 9} //April-September

            };
            _winterMonthsPrice = new Pricing()
            {
                Id = 2,
                PricingName = "Winter Months",
                Price = 20,
                Months = new int[6] { 10, 11, 12, 1, 2, 3 } //October-March
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarPark>>> GetCars()
        {
            if (_dbContext.Cars == null)
            {
                return NotFound();
            }
            return await _dbContext.Cars.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarPark>> GetCar(int id)
        {
            if (_dbContext.Cars == null)
            {
                return NotFound();
            }

            var car = await _dbContext.Cars.FindAsync(id);

            if(car == null)
            {
                return NotFound();
            }

            return car;
        }

        [HttpGet("{startDate}/{endDate}")]
        public async Task<ActionResult<int>> GetParkingCapcityForDates(DateTime startDate, DateTime endDate)
        {
            if (_dbContext.Cars == null)
            {
                return NotFound();
            }
            var currentCars = await _dbContext.Cars.Where(x => x.DateFrom >= startDate && x.DateTo <= endDate).ToListAsync();

            return currentCars.Count;          
        }

        [HttpPost]
        public async Task<ActionResult<CarPark>> CreateCarBooking(CarPark carBooking)
        {
            if (!CarParkingIsAvailable(carBooking))
            {
                //NOTE. Current issue = the start time must be 00:00:00 and the end time must be 23:59:59
                return BadRequest("Dates unavailable");
            }
            else
            {
                _dbContext.Cars.Add(carBooking);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCar), new { id = carBooking.Id }, carBooking);
               
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AmendCarBooking(int id, CarPark carBooking)
        {
            if(id != carBooking.Id)
            {
                return BadRequest("Booking not found");
            }

            if (!CarParkingIsAvailable(carBooking))
            {
                return BadRequest("Dates unavailable");
            }
            else
            {
                _dbContext.Entry(carBooking).State = EntityState.Modified;

                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarBookingExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
            }
        }

        //ID would be the customer's reference number
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelCarBooking(int id)
        {
            if (_dbContext.Cars == null)
            {
                return NotFound();
            }

            var car = await _dbContext.Cars.FindAsync(id);
            if(car == null)
            {
                return NotFound();
            }

            _dbContext.Cars.Remove(car);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }


        private bool CarBookingExists(int id)
        {
            return (_dbContext.Cars?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool CarParkingIsAvailable(CarPark carBooking)
        {
            
            if (GetParkingCapcityForDates(carBooking.DateFrom, carBooking.DateTo).Result.Value >= _carParkMaxCapacity)
            {
                return false;
            }
            else
            {
                CheckCarParkingPrice(carBooking);
                return true;
            }
        }

        private decimal CheckCarParkingPrice(CarPark carBooking)
        {
            var amountOfDays = carBooking.DateTo.Day - carBooking.DateFrom.Day;
            decimal totalPrice = 0;

            for(int i = 0; i < amountOfDays; i++)
            {
                if (_summerMonthsPrice.Months.Contains(carBooking.DateFrom.AddDays(i).Month))
                {
                    totalPrice = totalPrice + _summerMonthsPrice.Price;
                }
                if(_winterMonthsPrice.Months.Contains(carBooking.DateFrom.AddDays(i).Month))
                {
                    totalPrice = totalPrice + _winterMonthsPrice.Price;
                }
            }

            carBooking.Price = totalPrice;

            return carBooking.Price;
            
        }
    }
}
