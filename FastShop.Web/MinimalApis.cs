namespace FastShop.Web;

public static class MinimalApis
{
    public static void MinimalApi(this WebApplication app)
    {
        app.MapGet("/privacy", () =>
        {
            return Results.Extensions.RazorSlice<Slices.Privacy, DateTime>(DateTime.Now);

        });
    }
}