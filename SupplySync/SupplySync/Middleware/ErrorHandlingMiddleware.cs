using System.Net;
using System.Text.Json;

namespace SupplySync.Middleware
{
    public class ErrorHandlingMiddleware
    {

		private readonly RequestDelegate _next;
		private readonly ILogger<ErrorHandlingMiddleware> _logger;

		public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unhandled exception occurred");
				var status = GetStatusCode(ex);

				context.Response.StatusCode = (int)status;
				context.Response.ContentType = "application/json";

				var response = new
				{
					success = false,
					message = ex.Message,
					status = (int)status,
					errorName = ex.GetType().Name,
					traceId = context.TraceIdentifier
				};

				await context.Response.WriteAsync(JsonSerializer.Serialize(response));
			}
		}

		private HttpStatusCode GetStatusCode(Exception ex)
		{
			return ex switch
			{
				KeyNotFoundException => HttpStatusCode.NotFound,          // 404
				ArgumentException => HttpStatusCode.BadRequest,           // 400
				InvalidOperationException => HttpStatusCode.Conflict,     // 409
				UnauthorizedAccessException => HttpStatusCode.Unauthorized, // 401
				_ => HttpStatusCode.InternalServerError                   // 500 default
			};
		}
	}
}