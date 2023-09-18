using CustomerWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CustomerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDbContext _dbContext;

        public CustomerController(CustomerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(_dbContext.Customers);
        }

        [HttpGet("{customerId:int}")]
        public async Task<IActionResult> GetById(int customerId)
        {
            var customer = await _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
            return Ok(customer);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(Customer customer)
        {
            await _dbContext.Customers.AddAsync(customer);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Administrator, Guest")]
        public async Task<IActionResult> Update(Customer customer)
        {
            _dbContext.Customers.Update(customer);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{customerId:int}")]
        public async Task<IActionResult> Delete(int customerId)
        {
            var currentCus = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
            _dbContext.Customers.Remove(currentCus);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
