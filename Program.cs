using ApiDiariosOficiais.Infrastructure.BrowserManager;
using ApiDiariosOficiais.Interfaces;
using ApiDiariosOficiais.Services;
using ApiDiariosOficiais.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers();
builder.Services.AddSingleton<BrowserManager>();
builder.Services.AddSingleton<IAcreService, AcreService>();

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

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowAll");

app.MapControllers();
app.Lifetime.ApplicationStopping.Register(() =>
{
    var browserManager = app.Services.GetRequiredService<BrowserManager>();
    browserManager.Dispose();
});
app.Run();