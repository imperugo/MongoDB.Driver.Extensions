# MongoDB.Driver.Extensions

[![Nuget](https://img.shields.io/nuget/v/MongoDB.Driver.Extensions?style=flat-square)](https://www.nuget.org/packages/MongoDB.Driver.Extensions/)
[![Nuget](https://img.shields.io/nuget/vpre/MongoDB.Driver.Extensions?style=flat-square)](https://www.nuget.org/packages/MongoDB.Driver.Extensions/)
[![GitHub](https://img.shields.io/github/license/imperugo/MongoDB.Driver.Extensions?style=flat-square)](https://github.com/imperugo/MongoDB.Driver.Extensions/blob/main/LICENSE)


**MongoDB.Driver.Extensions** is a library that extends [MongoDB.Driver](https://docs.mongodb.com/ecosystem/drivers/csharp/) allowing you a set of functionality needed by common applications.
The library is completely compatible with the **.Net Standard 2.0**

## What can it be used for?
The idea behind this library is to make easier the common operation around a document you have persisted into MongoDb.

For example you have:

* **Save or update** a document;
* **Insert** a new document;
* **Update** a document;
* **Delete** a document;
* **Check** if a document exists into your database;
* **Insert many** documents in a single roundtrip;
* **Update many** documents in a single roundtrip;
* **Retrieve** a document **by Id**;
* **Handling pagination**;
* **Count** the number of documents;

All the methods available to do in the list above are available in both sync / async version and offers different parameters in order to change the amount of data to work.

##How to install it
MongoDB.Driver.Extensions is available via NuGet, so to install is enough to do 

```
PM> Install-Package MongoDB.Driver.Extensions
```

## How to configure it

To use this library the first is to provide all the necessary information to the library. To do that the first thing to do is to create your document:

```csharp
public class User : DocumentBase<ObjectId>
{
	public string Firstname { get; set; }
	public string Lastname { get; set; }
	public string Email { get; set; }
	public Guid CompanyId { get; set; }
}
```

> In this example I'm using an `ObjectId` and database key, but of course you can change it with your favourite type (string, Guid, and so on).

Now is time to create your repository:

```csharp
internal class UserRepository : RepositoryBase<User, ObjectId>
{
    public UserRepository( IMongoClient mongoClient)
        : base(mongoClient, "MyDatabase", "MyCollectionName")
    {
    }
}
```

The next step is the configuration and the `IMongoClient` :

```csharp
var conf = new MongoDbDatabaseConfiguration();
conf.ConnectionString = "mongodb://localhost:27017

IMongoClient client = new MongoClient(conf.ConnectionString);

```

In you are in Asp.Net Core:

```csharp
services.AddMongoDb(x => x.ConnectionString = "mongodb://mongodbhost:27017/sample");

services.AddSingleton<IRepository<User, ObjectId>, UserRepository>();


```

Now, in your service, you can do someting like this:

```csharp
public class MyService : IMyService
{
	private readonly IRepository<User,ObjectId> userRepository;
	
	public MyService(IRepository<User,ObjectId> userRepository)
    {
    	this.userRepository = userRepository;
    }
    
    public Task<IPagedResult<User>> DoSomething(int pageIndex, int pageSize, Guid companyId)
    {
    	var request = new SimplePagedRequest();
    	request.PageIndex = pageIndex;
    	request.PageSize = pageSize;
    	
    	var filter = Builders<User>.Filter.Eq(x => x.CompanyId, companyId);
    	
    	return this.userRepository.GetPagedListAsync(request,filter);
    }
}
```
## Sample

Take a look [here](https://github.com/imperugo/MongoDB.Driver.Extensions/tree/master/sample/MongoDb.Driver.Extensions.Sample.AspNetCore)

## License

Imperugo.HttpRequestToCurl [MIT](https://github.com/imperugo/MongoDB.Driver.Extensions/blob/main/LICENSE) licensed.

### Contributing

Thanks to all the people who already contributed!

<a href="https://github.com/imperugo/MongoDB.Driver.Extensions/graphs/contributors">
  <img src="https://contributors-img.web.app/image?repo=imperugo/MongoDB.Driver.Extensions" />
</a>
