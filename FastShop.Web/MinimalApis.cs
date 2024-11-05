using FastShop.Data;

namespace FastShop.Web;

public static class MinimalApis
{
    public static void MinimalApi(this WebApplication app)
    {
        app.MapGet("/privacy", (FastShopDbContext ctx) =>
        {
            var test = ctx.Categories;

            return Results.Extensions.RazorSlice<Slices.Privacy, DateTime>(DateTime.Now);
        });
    }
}