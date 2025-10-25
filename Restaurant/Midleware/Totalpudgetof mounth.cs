using Models;

namespace Restaurant.Midleware
{
    public class Totalpudgetof_mounth
    {
        private readonly RequestDelegate _next;

        public Totalpudgetof_mounth(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var day = DateTime.Today;
            Order order = new();
            if(day.Day==24 &&DateTime.Now.Hour==12&&DateTime.Now.Minute==19&&DateTime.Now.Second==0)
            {
                order.TotalAmount = 0;
            }
            await _next(context);
        }
    }
}
