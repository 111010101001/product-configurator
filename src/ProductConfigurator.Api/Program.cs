using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using ProductConfigurator.Api.Data;
using ProductConfigurator.Api.Options;
using ProductConfigurator.Api.Services;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(o =>
        o.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider()))
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ConfiguratorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ConfigurationService>();

builder.Services.AddSingleton<ConfigurationValidator>();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddOptions<ConfiguratorOptions>()
    .BindConfiguration(ConfiguratorOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //app.MapScalarApiReference();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "v1"));
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ConfiguratorDbContext>();
    await SeedData.SeedAsync(db);
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
