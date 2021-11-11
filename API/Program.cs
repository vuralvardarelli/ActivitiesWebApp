using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

IConfiguration Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

AppSettings appSettings = new AppSettings();
Configuration.GetSection("AppSettings").Bind(appSettings);
builder.Services.AddSingleton<AppSettings>(appSettings);

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(appSettings.SqlConnectionString));

//For seeding data
builder.Services.AddTransient<DataSeeder>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Seed Data
IServiceScopeFactory scopedFactory = app.Services.GetService<IServiceScopeFactory>();
using (IServiceScope scope = scopedFactory.CreateScope())
{
    DataSeeder service = scope.ServiceProvider.GetService<DataSeeder>();
    service.SeedData();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
