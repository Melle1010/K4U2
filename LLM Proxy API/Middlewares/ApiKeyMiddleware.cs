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

            var VALID_API_KEY = context.RequestServices.GetRequiredService<IConfiguration>()["ApiKey"];
            
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API-nyckel saknas.");
                return;
            }

            
            if (!VALID_API_KEY.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Ogiltig API-nyckel.");
                return; // Avbryter även här
            }

            
            await _next(context);
        }
    }
}