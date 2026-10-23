using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using ProductConfigurator.Api.Data;
using ProductConfigurator.Api.Options;
using ProductConfigurator.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ConfiguratorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ProductService>();

builder.Services.AddOptions<ConfiguratorOptions>()
    .BindConfiguration(ConfiguratorOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ConfiguratorDbContext>();
    await SeedData.SeedAsync(db);
}


app.UseHttpsRedirection();

app.MapControllers();

app.Run();
