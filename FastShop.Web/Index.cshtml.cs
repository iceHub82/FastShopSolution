using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FastShop.Web;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        if (Request.Headers["HX-Request"].ToString() == "true")
        {
            return Partial("_PageContent", null);
        }
        return Page(); // Load the full page if not an HTMX request
    }
}