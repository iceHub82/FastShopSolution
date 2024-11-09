using Microsoft.EntityFrameworkCore;
using FastShop.Data;
using FastShop.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            var product = await dbContext.Products
                .Include(x => x.ProductSizes)
                .ThenInclude(x => x.Size).AsNoTracking().FirstAsync(p => p.Id == id);

            var productVm = new ProductVm { Id = product.Id, Name = product.Name, Price = product.Price.ToString() };

            if (product.ProductSizes.Any())
            {
                productVm.CheckedSizeId = product.ProductSizes.First().SizeId;

                foreach (var size in product.ProductSizes)
                    productVm.ProductSizes.Add(new ProductSizeVm { Id = size.SizeId, Name = size.Size.Name });
            }

            if (IsHtmx(request))
            {
                response.Headers.Append("Vary", "HX-Request");
                return Results.Extensions.RazorSlice<Slices.Product, ProductVm>(productVm);
            }

            return Results.Extensions.RazorSlice<Slices.Index, IndexVm>(new IndexVm { SectionUrl = $"/Product/{id}" });
        });

        app.MapGet("/add-to-cart/{id}", (FastShopDbContext context, int id, int checkedSize) =>
        {

            return "";
        });
    }

    private static bool IsHtmx(HttpRequest request)
    {
        return request.Headers["hx-request"] == "true";
    }
}