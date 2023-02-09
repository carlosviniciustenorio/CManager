using CManager.API.Common;
using CManager.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
builder.Configuration.SetBasePath(env.ContentRootPath)
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                     .AddEnvironmentVariables();

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

CacheExtensions.AddCacheDependency(builder.Services);
builder.Services.AddStackExchangeRedisCache(redis =>
{
    redis.InstanceName = builder.Configuration.GetSection("ApiSettings:Cache:InstanceName").Value;
    redis.Configuration = builder.Configuration.GetSection("ApiSettings:Cache:Configuration").Value;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health");
});

app.Run();