using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OrderWebApi.Models;
using System.Collections.Generic;

namespace OrderWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMongoCollection<Order> _orderCollection;

        public readonly IMongoClient _client;
        public readonly IMongoDatabase _database;

        private readonly IConfiguration _conf;
        public OrderController(IConfiguration configuration)
        {
            _conf = configuration;

            MongoCredential credential = MongoCredential.CreateCredential("admin", "root", _conf["DB_ROOT_PASSWORD"]);
            var settings = new MongoClientSettings
            {
                Credential = credential,
                Server = new MongoServerAddress(_conf["DB_HOST"], Convert.ToInt32(_conf["DB_PORT"]))
            };

            _client = new MongoClient(settings);
            _database = _client.GetDatabase(_conf["CLIENT_DB_NAME"]);

            _orderCollection = _database.GetCollection<Order>("order");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            // Tüm orderları çekmek istediğimiz Find method'una boş filter geçtik.
            return await _orderCollection.Find(Builders<Order>.Filter.Empty).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            //var filterDefinition = Builders<Order>.Filter.Eq(o => o.Id, id);
            var order = await _orderCollection.Find(o => o.Id == id).SingleOrDefaultAsync();
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            await _orderCollection.InsertOneAsync(order);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(Order order)
        {
            await _orderCollection.ReplaceOneAsync(o => o.Id == order.Id, order);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _orderCollection.DeleteOneAsync(o => o.Id == id);
            return Ok();
        }
    }
}
