namespace Controllers.Middleware
{
    public class ConsoleLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        public ConsoleLoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            //до
            Console.WriteLine("Has been request...");

            await _next(context); //викликає наступний елемент конвеєра (middleware, controller)

            //після
        }
    }
}
