
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImageCompressor.Library;

namespace ImageCompressor.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompressController : ControllerBase
    {
        private readonly ImageCompressor.Library.ImageCompressor _imageCompressor;

        public CompressController(ImageCompressor.Library.ImageCompressor imageCompressor)
        {
            _imageCompressor = imageCompressor;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CompressionRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ImageUrl))
            {
                return BadRequest();
            }

            var compressedImage = await _imageCompressor.CompressImageAsync(request.ImageUrl, request.Width, request.Quality, request.Format);

            if (compressedImage == null)
            {
                return StatusCode(500, "Image compression failed.");
            }

            return File(compressedImage, $"image/{request.Format}", $"compressed.{request.Format}");
        }
    }

    public class CompressionRequest
    {
        public required string ImageUrl { get; set; }
        public int Width { get; set; } = 1024;
        public int Quality { get; set; } = 80;
        public string Format { get; set; } = "webp";
    }
}
