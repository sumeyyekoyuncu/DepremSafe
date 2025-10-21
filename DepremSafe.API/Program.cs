using System.Text.Json.Serialization;
using DepremSafe.Core.Interfaces;
using DepremSafe.Core.Mapping;
using DepremSafe.Data.Context;
using DepremSafe.Data.Repositories;
using DepremSafe.Service.Interfaces;
using DepremSafe.Service.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<DepremSafeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserLocationRepository, UserLocationRepository>();
builder.Services.AddScoped<IEarthquakeRepository, EarthquakeRepository>();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Sonsuz döngü tespit edildiðinde döngüdeki referanslarý görmezden gel.
        // Bu, döngüdeki ikinci User nesnesinin null olarak serileþtirilmesini saðlar.
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IUserLocationService,UserLocationService>();
builder.Services.AddScoped<IEarthquakeService,EarthquakeService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IFcmService, FcmService>(client => { });
builder.Services.AddSingleton<IFcmService>(sp =>
    new FcmService(sp.GetRequiredService<HttpClient>(), "BEmPwDNQspj6qs0lRnV2CVnyg_Qpt0QwbYySqQi-XyYcCwWzQUOvLnNVf0Kfa8U2GNiBJ6iNu3_ZRSOUZvlML2Y"));
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
