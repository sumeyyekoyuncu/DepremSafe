using DepremSafe.Core.Interfaces;
using DepremSafe.Data.Context;
using DepremSafe.Data.Repositories;
using DepremSafe.Service.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<DepremSafeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserLocationRepository, UserLocationRepository>();
builder.Services.AddScoped<IEarthquakeRepository, EarthquakeRepository>();


builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserLocationService>();
builder.Services.AddScoped<EarthquakeService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DepremSafe API V1");
        c.RoutePrefix = string.Empty; // Swagger UI ana sayfada açýlýr
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
