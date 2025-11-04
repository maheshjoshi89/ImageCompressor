using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImageCompressor.WebApp.Models;
using ImageCompressor.Library;

namespace ImageCompressor.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ImageCompressor.Library.ImageCompressor _imageCompressor;

    public HomeController(ILogger<HomeController> logger, ImageCompressor.Library.ImageCompressor imageCompressor)
    {
        _logger = logger;
        _imageCompressor = imageCompressor;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Compress(string imageUrl, int width, int quality, string format)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            return View("Index");
        }

        var compressedImage = await _imageCompressor.CompressImageAsync(imageUrl, width, quality, format);

        if (compressedImage == null)
        {
            return View("Error");
        }

        return File(compressedImage, $"image/{format}", $"compressed.{format}");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}