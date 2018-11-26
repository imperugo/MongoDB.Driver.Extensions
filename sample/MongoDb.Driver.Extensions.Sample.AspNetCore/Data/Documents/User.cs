using MongoDB.Bson;
using MongoDB.Driver.Extensions.Documents;

namespace MongoDb.Driver.Extensions.Sample.AspNetCore.Data.Documents
{
    public class User : DocumentBase<string>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}