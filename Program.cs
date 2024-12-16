using ApiDiariosOficiais.Factory;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Services.Acre;
using ApiDiariosOficiais.Services.Alagoas;
using ApiDiariosOficiais.Services.Amapa;
using ApiDiariosOficiais.Services.Ceara;
using ApiDiariosOficiais.Services.MatoGrossoDoSul;
using ApiDiariosOficiais.Services.MinasGerais;
using ApiDiariosOficiais.Services.RioDeJaneiro;
using ApiDiariosOficiais.Services.RioGrandeDoSul;
using ApiDiariosOficiais.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;
using Polly;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers();
//builder.Services.AddSingleton<BrowserManager>();
builder.Services.AddScoped<IAcreService, AcreService>();
builder.Services.AddScoped<IAlagoasService, AlagoasService>();
builder.Services.AddScoped<ISaoPauloService, SaoPauloService>();
builder.Services.AddScoped<IRioDeJaneiroService, RioDeJaneiroService>();
builder.Services.AddScoped<IAmapaService, AmapaService>();
builder.Services.AddScoped<IRioGrandeDoSulService, RioGrandeDoSulService>();
builder.Services.AddScoped<IMinasGeraisService, MinasGeraisService>();
builder.Services.AddScoped<IMatoGrossoDoSulService, MatoGrossoDoSulService>();
builder.Services.AddScoped<ICearaService, CearaService>();
builder.Services.AddScoped<GenericServiceFactory>();

// Enable rate limiting
builder.Services.AddRateLimiter(options =>
{
    // Set a global limit
    options.AddFixedWindowLimiter("global", config =>
    {
        config.PermitLimit = 20; // max 20 requests
        config.Window = TimeSpan.FromSeconds(10); // per 10 seconds
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 10; // max 10 requests in the queue
    });

    //// Set rate limiting per endpoint
    //options.AddSlidingWindowLimiter("/api/acre", config =>
    //{
    //    config.PermitLimit = 50; // max 50 requests
    //    config.Window = TimeSpan.FromMinutes(1); // per 1 minute
    //    config.SegmentsPerWindow = 5; // segment the window
    //    config.QueueLimit = 10; // max 10 requests in the queue
    //});
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Allows any origin
              .AllowAnyMethod()  // Allows any HTTP method
              .AllowAnyHeader(); // Allows any header
    });
});
ConfigureHttpClients(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers().RequireRateLimiting("global");
//app.Lifetime.ApplicationStopping.Register(() =>
//{
//    var browserManager = app.Services.GetRequiredService<BrowserManager>();
//    browserManager.Dispose();
//});
app.Run();

void ConfigureHttpClients(IServiceCollection services, IConfiguration configuration)
{
    // Retrieve API configurations from appsettings.json or other configuration sources
    var apiConfigurations = configuration.GetSection("ApiConfigurations").GetChildren();

    foreach (var apiConfig in apiConfigurations)
    {
        // Register each HTTP client using the 'Api' as the name and 'BaseAddress' as the address
        var apiName = apiConfig["Api"];
        var baseAddress = apiConfig["BaseAddress"];
        services.AddHttpClient(apiName, client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        }).AddTransientHttpErrorPolicy(policyBuilder =>
    policyBuilder.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(3))
);
    }
}