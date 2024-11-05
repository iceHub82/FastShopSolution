using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using FastShop.Web;
using FastShop.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options => {
    options.RootDirectory = "/";
});

SqliteConnection? connection = null;
if (Debugger.IsAttached)
{
    connection = new SqliteConnection { ConnectionString = "Data Source=:memory:" };
    connection.Open();
}

builder.Services.AddDbContext<FastShopDbContext>(options => {
    if (Debugger.IsAttached)
    {
        options.LogTo(Console.WriteLine, LogLevel.Debug);
        options.UseSqlite(connection!);
    }
    else
        options.UseSqlite("Data Source=fastshop.db");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<FastShopDbContext>();
    db.Database.EnsureCreated(); // Creates tables and seeds data for in-memory database
    //db.Database.Migrate();
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

app.Run();