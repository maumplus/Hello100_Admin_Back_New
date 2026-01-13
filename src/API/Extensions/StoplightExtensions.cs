namespace Hello100Admin.API.Extensions
{
    public static class StoplightExtensions
    {
        public static IApplicationBuilder UseStoplight(this IApplicationBuilder app, string path = "/stoplight", string openApiJsonPath = "/swagger/v1/swagger.json")
        {
            app.Map(path, builder =>
            {
                builder.Run(async context =>
                {
                    var html = $@"
                        <!DOCTYPE html>
                        <html lang='en'>
                        <head>
                            <meta charset='UTF-8'>
                            <title>Stoplight API Docs</title>
                            <script src='https://unpkg.com/@stoplight/elements/web-components.min.js'></script>
                            <link rel='stylesheet' href='https://unpkg.com/@stoplight/elements/styles.min.css'>
                        </head>
                        <body>
                            <elements-api apiDescriptionUrl='{openApiJsonPath}' router='hash' layout='sidebar'></elements-api>
                        </body>
                        </html>
                    ";

                    context.Response.ContentType = "text/html";
                    await context.Response.WriteAsync(html);
                });
            });

            return app;
        }
    }
}
