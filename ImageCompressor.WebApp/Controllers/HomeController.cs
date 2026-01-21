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
    public async Task<IActionResult> Compress(string imageUrl, IFormFile imageFile, int width, int quality, string format)
    {
        byte[] compressedImage = null;

        // Handle file upload
        if (imageFile != null && imageFile.Length > 0)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                compressedImage = _imageCompressor.CompressImageFromBytes(imageBytes, width, quality, format);
            }
        }
        // Handle URL
        else if (!string.IsNullOrEmpty(imageUrl))
        {
            compressedImage = await _imageCompressor.CompressImageAsync(imageUrl, width, quality, format);
        }
        else
        {
            ModelState.AddModelError("", "Please provide either an image URL or upload a file.");
            return View("Index");
        }

        if (compressedImage == null)
        {
            ModelState.AddModelError("", "Image compression failed. Please try again.");
            return View("Index");
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