using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDB.Driver.Extensions.Paging.Requests;
using MongoDb.Driver.Extensions.Sample.AspNetCore.Controllers.Requests;
using MongoDb.Driver.Extensions.Sample.AspNetCore.Data.Documents;

namespace MongoDb.Driver.Extensions.Sample.AspNetCore.Controllers;

[Route("api/[controller]")]
//[ApiController]
public class UserController : ControllerBase
{
    private readonly IRepository<User, ObjectId> userRepository;

    public UserController(IRepository<User, ObjectId> userRepository)
    {
        this.userRepository = userRepository;
    }

    // GET api/values
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] SimplePagedRequest request)
    {
        var result = await userRepository
            .GetPagedListAsync(request);

        return Ok(result);
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(ObjectId id)
    {
        var result = await userRepository.GetByIdAsync(id);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // POST api/values
    [HttpPost]
    public async Task PostAsync([FromBody] AddUserRequest request)
    {
        var user = new User(ObjectId.GenerateNewId());
        user.FirstName = request.Firstname;
        user.LastName = request.Lastname;

        await userRepository.SaveOrUpdateAsync(user);
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(ObjectId id, [FromBody] string firstaname, [FromBody] string lastname)
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
    public async Task<IActionResult> DeleteAsync(ObjectId id)
    {
        var deleteResponse = await userRepository.DeleteAsync(id);

        if (deleteResponse.DeletedCount < 1)
            return NotFound();

        return new EmptyResult();
    }
}
