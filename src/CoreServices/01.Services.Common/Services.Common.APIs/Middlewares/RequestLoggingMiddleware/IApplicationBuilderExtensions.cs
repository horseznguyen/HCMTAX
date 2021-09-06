using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Services.Common.APIs
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }

    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;

            _logger = logger;

            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);

            await _next(context);
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();

            await context.Request.Body.CopyToAsync(requestStream);

            _logger.LogInformation("----- HTTP REQUEST INFORMATION\n" +
                                   " || PATH: {Path} \n" +
                                   " || QUERY STRING: {QueryString} \n" +
                                   " || REQUEST BODY: {Body}",
                                   context.Request.Path,
                                   context.Request.QueryString,
                                   ReadStreamInChunks(requestStream));

            context.Request.Body.Position = 0;
        }

        private async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            await using var responseBody = _recyclableMemoryStreamManager.GetStream();

            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation($"Http Response Information:{Environment.NewLine}" +
                                   $"Schema:{context.Request.Scheme} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString} " +
                                   $"Response Body: {text}");

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();

            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];

            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);

                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }
}