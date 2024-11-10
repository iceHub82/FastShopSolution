using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using FastShop.Data;
using FastShop.Data.Entities;
using FastShop.Web.ViewModels;

namespace FastShop.Web;

public static class MinimalApis
{
    public static void MinimalApi(this WebApplication app)
    {
        app.MapGet("/", () => {
            return Results.Extensions.RazorSlice<Slices.Index, IndexVm>(new IndexVm { SectionUrl = "/Products" });
        });

        app.MapGet("/cart-button", async (HttpContext context, FastShopDbContext dbContext) => {

            var sessionId = context.Request.Cookies["CartSessionId"];

            var cart = dbContext.Carts.Where(c => c.CartGuid == Guid.Parse(sessionId)).First();

            var cartItemsCount = await dbContext.CartItems.CountAsync(c => c.CartId == cart.Id);

            return Results.Extensions.RazorSlice<Slices.CartButton, int>(cartItemsCount);
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

        app.MapGet("/add-to-cart/{id}", async (HttpContext context, FastShopDbContext dbContext, int id, int checkedSize) =>
        {
            var sessionId = context.Request.Cookies["CartSessionId"];

            var cart = dbContext.Carts.Where(c => c.CartGuid == Guid.Parse(sessionId)).First();

            await dbContext.CartItems.AddAsync(new CartItem { CartId = cart.Id, ProductId = id, SizeId = checkedSize == 0 ? null : checkedSize });
            await dbContext.SaveChangesAsync();

            var cartItemsCount = dbContext.CartItems.Count(c => c.CartId == cart.Id);

            return Results.Extensions.RazorSlice<Slices.CartButton, int>(cartItemsCount);
        });

        app.MapGet("/Redirects", (int statusCode) => {

            if (statusCode == 404)
            {
                 
            }

            return Results.Extensions.RazorSlice<Slices.NotFound>();
        });

        app.MapGet("/Error", (HttpContext context) =>
        {
            // Access exception details, if needed
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

            // Log the exception or handle it as needed
            Console.WriteLine($"Unhandled exception: {exception?.Message}");

            // Return a custom response or redirect
            //return Results.Problem(
            //    detail: "An unexpected error occurred.",
            //    statusCode: 500
            //);

            return Results.Extensions.RazorSlice<Slices.Error>();
        });
    }

    private static bool IsHtmx(HttpRequest request)
    {
        return request.Headers["hx-request"] == "true";
    }
}