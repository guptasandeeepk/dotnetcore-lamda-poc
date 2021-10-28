using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.APIGatewayEvents;
using Microsoft.AspNetCore.Hosting;

namespace Poc.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // AWS Lambda will call the "bootstrap" shell script, which
            // will pass the string "lambda" as an argument
            if (args.Length > 0 && args[0] == "lambda")
            {
                var lambdaEntryPoint = new LambdaEntryPoint();

                // This explicit cast is needed with slightly older C# versions
                var lambdaFunctionHandler = (Func<APIGatewayProxyRequest, Amazon.Lambda.Core.ILambdaContext, Task<APIGatewayProxyResponse>>)lambdaEntryPoint.FunctionHandlerAsync;

                var handlerWrapper = HandlerWrapper.GetHandlerWrapper<APIGatewayProxyRequest, APIGatewayProxyResponse>(lambdaFunctionHandler, new DefaultLambdaJsonSerializer());
                using (handlerWrapper)
                {
                    using (var lambdaBootstrap = new LambdaBootstrap(handlerWrapper))
                    {
                        lambdaBootstrap.RunAsync().GetAwaiter().GetResult();
                    }
                }
            }
            // Otherwise, it gets called like most ASP.NET Core web apps
            else
            {
                CreateHostBuilder(args).Build().Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
