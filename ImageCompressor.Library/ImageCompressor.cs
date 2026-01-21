
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ImageMagick;

namespace ImageCompressor.Library
{
    public class ImageCompressor
    {
        private readonly HttpClient _httpClient;

        public ImageCompressor(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> CompressImageAsync(string imageUrl, int width, int quality, string format)
        {
            try
            {
                var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
                return CompressImageFromBytes(imageBytes, width, quality, format);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public byte[] CompressImageFromBytes(byte[] imageBytes, int width, int quality, string format)
        {
            try
            {
                using (var image = new MagickImage(imageBytes))
                {
                    var size = new MagickGeometry((uint)width, 0);
                    image.Resize(size);

                    switch (format.ToLower())
                    {
                        case "webp":
                            image.Format = MagickFormat.WebP;
                            image.Quality = (uint)quality;
                            break;
                        case "avif":
                            image.Format = MagickFormat.Avif;
                            image.Quality = (uint)quality;
                            break;
                        default:
                            throw new ArgumentException("Unsupported image format. Please choose 'webp' or 'avif'.");
                    }

                    return image.ToByteArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
