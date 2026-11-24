using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProductConfigurator.Api.Data;
using ProductConfigurator.Api.Options;
using ProductConfigurator.Api.Services;
using ProductConfigurator.Api.Telemetry;
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

var serviceVersion = typeof(Program).Assembly
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

builder.Services.AddSingleton<ConfiguratorMetrics>();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddDbContextCheck<ConfiguratorDbContext>(tags: ["ready"]);

var otel = builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("product-configurator-api", serviceVersion: serviceVersion))
    .WithTracing(tracing => tracing
        .SetSampler(new AlwaysOnSampler())
        .AddAspNetCoreInstrumentation(o => o.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health"))  // a span for each incoming request
        .AddHttpClientInstrumentation()   // a span for each outgoing HttpClient call (and passes the trace ID on)
        .AddSource(ConfiguratorTelemetry.Name)
        .AddSqlClientInstrumentation())   // a span for each SQL command, with the query text
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()   // request duration, active requests, errors
        .AddHttpClientInstrumentation()   // outgoing call duration
        .AddSqlClientInstrumentation()    // SQL command duration
        .AddMeter("System.Runtime", "Microsoft.EntityFrameworkCore", ConfiguratorTelemetry.Name)) // GC, thread pool, CPU; EF query counts
    .WithLogging(configureBuilder: null, configureOptions: options =>
    {
        options.IncludeScopes = true;            // BeginScope data becomes log fields
        options.IncludeFormattedMessage = true;  // also keep the finished message text
    });

// Only export when the variable is set, so tests and CI don't try to reach a dashboard
if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
{
    otel.UseOtlpExporter();
}

var app = builder.Build();
app.Use(async (context, next) =>
{
    var traceId = Activity.Current?.TraceId.ToString();
    if (!string.IsNullOrEmpty(traceId) && traceId != default(ActivityTraceId).ToString())
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Trace-Id"] = traceId;
            return Task.CompletedTask;
        });
    }

    await next();
});

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

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
}).DisableHttpMetrics();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
}).DisableHttpMetrics();

app.Run();
