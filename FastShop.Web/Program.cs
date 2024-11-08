using Microsoft.EntityFrameworkCore;
using FastShop.Web;
using FastShop.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options => {
    options.RootDirectory = "/";
});

var connectionString = builder.Configuration.GetConnectionString("FastShopConnection");
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Services.AddDbContext<FastShopDbContext>(options => {
    if (env == "Development")
    {
        options.EnableSensitiveDataLogging();
        options.LogTo(Console.WriteLine, LogLevel.Debug);
    }

    options.UseSqlite(connectionString!);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    scope.ServiceProvider.GetRequiredService<FastShopDbContext>().Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MinimalApi();

app.Logger.LogInformation($"Fast Shop App Start - Environment: {env}");

app.Run();