using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Entities;


var builder = WebApplication.CreateBuilder(args);
var configs = builder.Configuration;

const string _ALLOWED_ORIGIN_STRING = "AllowOrigin";

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMongo()
                .AddMongoRepository<InventoryItem>("inventory_items")
                .AddMongoRepository<CatalogItem>("catalog-items");

builder.Services.AddMassTransitWithRabbitMQ();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

     app.UseCors(corsPolicyBuilder =>
    {
        corsPolicyBuilder.WithOrigins(configs[key: _ALLOWED_ORIGIN_STRING]).AllowAnyHeader().AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();