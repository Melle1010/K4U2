namespace LLM_Proxy_API.Middlewares
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "X-API-KEY";
        

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var config = context.RequestServices.GetRequiredService<IConfiguration>();
            var VALID_API_KEY = config["ApiKey"];

            if (string.IsNullOrWhiteSpace(VALID_API_KEY))
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("API key configuration is missing.");
                return;
            }
            
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API-nyckel saknas.");
                return;
            }

            
            if (!string.Equals(VALID_API_KEY, extractedApiKey, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Ogiltig API-nyckel.");
                return; // Avbryter även här
            }

            
            await _next(context);
        }
    }
}