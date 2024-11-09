using Microsoft.EntityFrameworkCore;
using FastShop.Data;
using FastShop.Web.ViewModels;

namespace FastShop.Web;

public static class MinimalApis
{
    public static void MinimalApi(this WebApplication app)
    {
        app.MapGet("/", (HttpRequest request) => {
            return Results.Extensions.RazorSlice<Slices.Index, IndexVm>(new IndexVm { SectionUrl = "/Products" });
        });

        app.MapGet("/Products", async (FastShopDbContext context, HttpRequest request) =>
        {
            var products = await context.Products
                .AsNoTracking()
                //.Where(x => x.SomeCondition)
                .Select(p => new ProductVm { Id = p.Id, Name = p.Name, Price = p.Price.ToString() })
                .ToListAsync();

            return Results.Extensions.RazorSlice<Slices.Products, List<ProductVm>>(products);
        });

        app.MapGet("/Product/{id}", async (HttpRequest request, HttpResponse response, FastShopDbContext dbContext, int id) =>
        {
            var product = await dbContext.Products.AsNoTracking().FirstAsync(p => p.Id == id);

            var productVm = new ProductVm { Id = product.Id, Name = product.Name, Price = product.Price.ToString() };

            if (IsHtmx(request))
            {
                response.Headers.Append("Vary", "HX-Request");
                return Results.Extensions.RazorSlice<Slices.Product, ProductVm>(productVm);
            }

            return Results.Extensions.RazorSlice<Slices.Index, IndexVm>(new IndexVm { SectionUrl = $"/Product/{id}" });
        });

        app.MapGet("/add-to-cart/{id}", async (FastShopDbContext context, int id) =>
        {
            //var products = await context.Products
            //    .AsNoTracking()
            //    //.Where(x => x.SomeCondition)
            //    .Select(p => new ProductViewModel { Id = p.Id, Name = p.Name, Price = p.Price.ToString() })
            //    .ToListAsync();

            //return Results.Extensions.RazorSlice<Slices.Products, List<ProductViewModel>>(products);
        });
    }

    private static bool IsHtmx(HttpRequest request)
    {
        return request.Headers["hx-request"] == "true";
    }
}