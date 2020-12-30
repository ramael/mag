using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MagApi.Exceptions
{
    public static class GlobalExceptionHandler
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, int StatusCode = 0, string message = "")
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        switch (contextFeature.Error)
                        {
                            //case Exception1 e:
                            //    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            //    break;
                            //case Exception2 e:
                            //    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            //    break;
                            //case Exception3 e:
                            //    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                            //    break;
                            default:
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                break;
                        }

                        logger.Error($"Something went wrong: {contextFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetails(context.Response.StatusCode, "Internal Server Error.").ToString());
                    }
                });
            });
        }
    }
}
