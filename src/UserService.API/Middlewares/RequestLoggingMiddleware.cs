using System.Text;

namespace UserService.API.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only buffer for PUT/POST or needed methods
        if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
        {
            context.Request.EnableBuffering(); // allow multiple reads

            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // reset stream

            _logger.LogInformation("Request: {Method} '{Path}' \n{Body}", context.Request.Method, context.Request.Path, body);
        }

        await _next(context);

        _logger.LogInformation("Request: {Method} '{Path}' handled at {Time}.", context.Request.Method, context.Request.Path, DateTime.Now.ToLongTimeString());
    }
}