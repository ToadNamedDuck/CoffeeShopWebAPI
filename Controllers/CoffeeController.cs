using CoffeeShop.Models;
using CoffeeShop.Repositories;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoffeeShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoffeeController : ControllerBase
    {
        private readonly IBeanVarietyRepository _beanVarietyRepository;
        private readonly ICoffeeRepository _coffeeRepository;
        public CoffeeController(ICoffeeRepository coffeeRepository)
        {
            _coffeeRepository = coffeeRepository;
        }
        // GET: api/<CoffeController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_coffeeRepository.GetAll());
        }

        // GET api/<CoffeController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Coffee coffee = _coffeeRepository.Get(id);
            if(coffee == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(coffee);
            }
        }

        // POST api/<CoffeController>
        [HttpPost]
        public IActionResult Post(Coffee coffee)
        {
            try
            {
                _coffeeRepository.Add(coffee);
                return CreatedAtAction("Get", new { id = coffee.Id }, coffee);
            }
            catch
            {
                return BadRequest();
            }
        }

        // PUT api/<CoffeController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, Coffee coffee)
        {
            if (id != coffee.Id || coffee == null)
            {
                return BadRequest();
            }
            try
            {
                _coffeeRepository.Update(coffee);
                return NoContent();
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE api/<CoffeController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if(_coffeeRepository.Get(id) == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    _coffeeRepository.Delete(id);
                     return NoContent();
                }
                catch
                {
                    return BadRequest(); 
                }
            }
        }
    }
}
