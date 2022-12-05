using MongoDB.Bson;
using MongoDB.Driver.Extensions.Abstractions;
using MongoDb.Driver.Extensions.Sample.AspNetCore.Data;
using MongoDb.Driver.Extensions.Sample.AspNetCore.Data.Documents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMongoDb(x => x.ConnectionString = "mongodb://mongo.imperugo.com:27017");
builder.Services.AddSingleton<IRepository<User, ObjectId>, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
