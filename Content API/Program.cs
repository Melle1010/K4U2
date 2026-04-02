using Content_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("MyDatabase"));

var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
