# MongoDB.Driver.Extensions

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
internal class UserRepository : RepositoryBase<User,ObjectId>
{
	public UserRepository(MongoDbDatabaseConfiguration configuration, 
							IMongoClient mongoClient) 
									: base(configuration, 
											mongoClient, 
											"MyDatabaseName")
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

If you are using Castle Windsort is enough to do something like this:

```csharp

container.Register(Component.For<MongoDbDatabaseConfiguration>()
				.Instance(conf)
				.LifestyleSingleton());

container.Register(Component.For<IMongoClient>()
				.Instance(client)
				.LifestyleSingleton());

container.Register(Component.For<IRepository<User,ObjectId>>()
				.ImplementedBy<UserRepository>()
				.LifestyleSingleton());
```

In you are in Asp.Net Core:

```csharp
services.AddSingleton<MongoDbDatabaseConfiguration>(conf);
services.AddSingleton<IMongoClient>(client);

services.AddSingleton<IRepository<User,ObjectId>, UserRepository>(client);


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

## DB name (Conventions and overrides)

Sometimes you have the same MongoDb Instance for all environments (I know this is not good) but you have to be sure you are using a different db name in order to work on new feature or test something that is almost ready for production.

For example for the User db you could have something like:

* Users (production environment);
* UsersTest (test environment);
* UserDev (dev environment);

As you saw in the example of `UserRepository` you can specify the name of the database you want to use; it means all you need to do is change this with `UserDev` and it works with Dev.
The bad part of this is that you have to change the name everytime you need to switch between the different environments.

Of course this is possible using this library without hardcode the db name into your code or adding tons of if.

What you have to do is to add the suffix you want into the `MongoDbDatabaseConfiguration` class;

```csharp
var conf = new MongoDbDatabaseConfiguration();
conf.ConnectionString = "mongodb://localhost:27017
conf.EnvironmentSuffix = "Dev";
```

In this case, when you specify the database on your repository, is enough to say `Users` the repo will translate it in `UsersDev`;

> If you want to override this behaviour with something more complicated of course you can just creating a class that inherits from `IMongoDbNamingHelper` apply your logic and register it to you dependency injection container

##Collection Name (Conventions and overrides)

Out of the box the collection name is calculated using the name of the document pluralised. In the example of the `User` the collection is `Users`, for a document called `vehicle` the collection name is `vehicles` and so on.

If you want to change this behaviour you have two possible ways:

1. Force the collection name in your repository;
2. Add a dynamic behaviour; 

### Force collection name

```csharp
internal class UserRepository : RepositoryBase<User,string>, IRepository<User,string>
{
	public UserRepository(MongoDbDatabaseConfiguration configuration, 
							IMongoClient mongoClient) 
									: base(configuration, 
											mongoClient, 
											"MyDatabase",
											 null,
											 "MyCollectionName")
	{
	}
}
```

### Claculate dinamically the name of the collection

Like mentioned for the database name, the same is valid here, just create a class that inherits from `IMongoDbNamingHelper` apply your logic and register it to you dependency injection container

## IMongoDbNamingHelper

As you understood the `IMongoDbNamingHelper` has an important role in this library, here the code of the default implementation:

```csharp
internal class DefaultMongoDbNamingHelper : IMongoDbNamingHelper
{
	public string GetDatabaseName(MongoDbDatabaseConfiguration configuration, string dbName)
	{
		return string.Concat(dbName,configuration.EnvironmentSuffix);
	}

	public string GetCollectionName(string requiredCollection)
	{
		return requiredCollection?.Pluralize().ToLowerInvariant();
	}
}
```



