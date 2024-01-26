namespace MesDoigtsDeFees.Services
{
    public class ThemeMiddleware
    {
        private readonly RequestDelegate _next;

        public ThemeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var themeCookie = context.Request.Cookies["UserTheme"];
            if (!string.IsNullOrEmpty(themeCookie))
            {
                // Pas het thema toe op de huidige gebruikerssessie
                context.Items["UserTheme"] = themeCookie;
            }

            await _next(context);
        }
    }

}
