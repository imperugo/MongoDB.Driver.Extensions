using System.ComponentModel.DataAnnotations;

namespace MongoDb.Driver.Extensions.Sample.AspNetCore.Controllers.Requests
{
    public class AddUserRequest
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
    }
}