using Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;


var builder = WebApplication.CreateBuilder(args);
var configs = builder.Configuration;

const string _ALLOWED_ORIGIN_STRING = "AllowOrigin";

// Get & Deserialize Settings From AppSettings...
var serviceSettings = configs.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

builder.Services.AddMongo();
builder.Services.AddMongoRepository<Item>("items");

builder.Services.AddMassTransitWithRabbitMQ();

// Add services to the container.
builder.Services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; });

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