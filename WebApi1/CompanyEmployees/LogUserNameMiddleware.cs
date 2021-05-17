using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CompanyEmployees
{
    public class LogUserNameMiddleware
    {
        private readonly RequestDelegate next;

        public LogUserNameMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            try
            {
                if (context.User?.Identity?.Name != null)
                    Serilog.Context.LogContext.PushProperty("UserName", context.User.Identity.Name);
            }
            catch { }
            return next(context);
        }
    }
}
