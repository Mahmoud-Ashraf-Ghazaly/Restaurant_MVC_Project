namespace Restaurant.Middleware
{
    public class Time
    {
        private readonly RequestDelegate _next;

        public Time(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var currentHour = DateTime.Now.Hour;
            var path = context.Request.Path.Value?.ToLower();

           
            bool isOrderRequest = path != null && (
                path.Contains("/order/create")||


                (context.Request.Method == "POST" && path.Contains("/order"))
            );

           
            if (isOrderRequest && (currentHour >= 20 || currentHour < 8))
            {
           
                if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                    context.Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(@"{
                        ""success"": false,
                        ""message"": ""Sorry, ordering system is currently closed. Operating hours: 8:00 AM - 8:00 PM"",
                        ""currentTime"": """ + DateTime.Now.ToString("hh:mm tt") + @"""
                    }");
                    return;
                }

                // For regular requests, show HTML page
                context.Response.StatusCode = 403;
                context.Response.ContentType = "text/html; charset=utf-8";

                await context.Response.WriteAsync(@"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='utf-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Ordering System Closed</title>
                        <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
                        <link href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css' rel='stylesheet'>
                        <style>
                            body {
                                background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
                                min-height: 100vh;
                                display: flex;
                                align-items: center;
                                justify-content: center;
                                font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            }
                            .restriction-card {
                                background: white;
                                border-radius: 20px;
                                padding: 50px;
                                box-shadow: 0 20px 60px rgba(0,0,0,0.3);
                                text-align: center;
                                max-width: 500px;
                                animation: fadeInUp 0.6s ease;
                            }
                            @keyframes fadeInUp {
                                from {
                                    opacity: 0;
                                    transform: translateY(30px);
                                }
                                to {
                                    opacity: 1;
                                    transform: translateY(0);
                                }
                            }
                            .icon-wrapper {
                                font-size: 80px;
                                color: #f5576c;
                                margin-bottom: 20px;
                                animation: pulse 2s infinite;
                            }
                            @keyframes pulse {
                                0%, 100% { transform: scale(1); }
                                50% { transform: scale(1.1); }
                            }
                            h1 {
                                color: #2c3e50;
                                font-weight: 700;
                                margin-bottom: 15px;
                            }
                            p {
                                color: #6c757d;
                                font-size: 1.1rem;
                                margin-bottom: 10px;
                            }
                            .time-info {
                                background: #f8f9fa;
                                padding: 15px;
                                border-radius: 10px;
                                margin: 20px 0;
                                font-weight: 600;
                                color: #495057;
                            }
                            .btn-back {
                                background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
                                color: white;
                                padding: 12px 40px;
                                border-radius: 30px;
                                border: none;
                                font-weight: 600;
                                text-decoration: none;
                                display: inline-block;
                                margin-top: 20px;
                                transition: all 0.3s ease;
                            }
                            .btn-back:hover {
                                transform: translateY(-3px);
                                box-shadow: 0 10px 25px rgba(245, 87, 108, 0.4);
                                color: white;
                            }
                        </style>
                    </head>
                    <body>
                        <div class='restriction-card'>
                            <div class='icon-wrapper'>
                                <i class='bi bi-basket-fill'></i>
                            </div>
                            <h1>Ordering System Closed</h1>
                            <p>Sorry, we cannot accept new orders at this time</p>
                            <div class='time-info'>
                                <i class='bi bi-moon-stars-fill'></i> Closed: 8:00 PM - 8:00 AM<br>
                                <i class='bi bi-sun-fill'></i> Open: 8:00 AM - 8:00 PM
                            </div>
                            <p style='font-size: 0.95rem; color: #868e96;'>
                                Current Time: " + DateTime.Now.ToString("hh:mm tt") + @"
                            </p>
                            <p style='font-size: 0.9rem; margin-top: 20px;'>
                                You can browse our menu, but orders are currently unavailable
                            </p>
                            <a href='/' class='btn-back'>
                                <i class='bi bi-arrow-left'></i> Back to Home
                            </a>
                        </div>
                    </body>
                    </html>
                ");

                return;
            }

            await _next(context);
        }
    }
}
