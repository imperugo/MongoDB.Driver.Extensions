using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDb.Driver.Extensions.Sample.AspNetCore.Controllers.Requests;
using MongoDb.Driver.Extensions.Sample.AspNetCore.Data.Documents;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Paging.Requests;

namespace MongoDb.Driver.Extensions.Sample.AspNetCore.Controllers
{
    [Route("api/[controller]")]
    //[ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User, string> userRepository;

        public UserController(IRepository<User, string> userRepository)
        {
            this.userRepository = userRepository;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SimplePagedRequest request)
        {
            var result = await userRepository
                .GetPagedListAsync(request);

            return Ok(result);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await userRepository.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST api/values
        [HttpPost]
        public async Task Post([FromBody] AddUserRequest request)
        {
            var user = new User();
            user.Id = Guid.NewGuid().ToString();
            user.FirstName = request.Firstname;
            user.LastName = request.Lastname;

            await userRepository.SaveOrUpdateAsync(user);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] string firstaname, [FromBody] string lastname)
        {
            var user = await userRepository.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            user.FirstName = firstaname;
            user.LastName = lastname;

            await userRepository.SaveOrUpdateAsync(user);

            return new EmptyResult();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleteResponse = await userRepository.DeleteAsync(id);

            if (deleteResponse.DeletedCount < 1)
                return NotFound();

            return new EmptyResult();
        }
    }
}