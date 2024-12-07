using ApiDiariosOficiais.Infrastructure.BrowserManager;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Services;
using ApiDiariosOficiais.Services.Acre;
using ApiDiariosOficiais.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;
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
builder.Services.AddSingleton<IAcreService, AcreService>();
builder.Services.AddSingleton<IAlagoasService, AlagoasService>();

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

builder.Services.AddHttpClient("ApiAcre", client =>
{
    client.BaseAddress = new Uri("https://diario.ac.gov.br/");
});
builder.Services.AddHttpClient("ApiAlagoas", client =>
{
    client.BaseAddress = new Uri("https://diario.imprensaoficial.al.gov.br/");
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Allows any origin
              .AllowAnyMethod()  // Allows any HTTP method
              .AllowAnyHeader(); // Allows any header
    });
});
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