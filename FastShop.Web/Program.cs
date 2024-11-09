using Microsoft.EntityFrameworkCore;
using FastShop.Web;
using FastShop.Data;
using FastShop.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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

app.Use(async (context, next) =>
{
    if (!context.Request.Cookies.TryGetValue("CartSessionId", out var cartSessionGuid))
    {
        var guid = Guid.NewGuid();
        cartSessionGuid = guid.ToString();
        context.Response.Cookies.Append("CartSessionId", cartSessionGuid);

        var scopeFactory = context.RequestServices.GetRequiredService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FastShopDbContext>();
        dbContext.Carts.Add(new Cart { CartGuid = guid, Created = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();
    }

    context.Items["CartSessionId"] = cartSessionGuid;

    await next();
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MinimalApi();

app.Logger.LogInformation($"Fast Shop App Start - Environment: {env}");

app.Run();