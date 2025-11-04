using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace ImageCompressor.Library.Tests
{
    [TestClass]
    public class ImageCompressorTests
    {
        [TestMethod]
        public async Task CompressImageAsync_WithValidImageUrl_ReturnsCompressedImage()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII="))
            };

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var imageCompressor = new ImageCompressor.Library.ImageCompressor(httpClient);

            // Act
            var result = await imageCompressor.CompressImageAsync("https://example.com/image.png", 100, 80, "webp");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task CompressImageAsync_WithUnsupportedFormat_ThrowsArgumentException()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII="))
            };

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var imageCompressor = new ImageCompressor.Library.ImageCompressor(httpClient);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => imageCompressor.CompressImageAsync("https://example.com/image.png", 100, 80, "bmp"));
        }

        [TestMethod]
        public async Task CompressImageAsync_WithHttpError_ReturnsNull()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
            };

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var imageCompressor = new ImageCompressor.Library.ImageCompressor(httpClient);

            // Act
            var result = await imageCompressor.CompressImageAsync("https://example.com/image.png", 100, 80, "webp");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task CompressImageAsync_WithInvalidImageData_ReturnsNull()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(new byte[] { 1, 2, 3 }) // Invalid image data
            };

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var imageCompressor = new ImageCompressor.Library.ImageCompressor(httpClient);

            // Act
            var result = await imageCompressor.CompressImageAsync("https://example.com/image.png", 100, 80, "webp");

            // Assert
            Assert.IsNull(result);
        }
    }
}