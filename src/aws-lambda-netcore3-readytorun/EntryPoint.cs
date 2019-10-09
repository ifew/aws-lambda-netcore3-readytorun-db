using System.Collections.Generic;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using aws_lambda_netcore3_readytorun;

namespace LambdaNative
{
    public static class EntryPoint
    {
        public static void Main()
        {
            LambdaNative.Run<Handler, APIGatewayProxyRequest, APIGatewayProxyResponse>();
        }
    }
}
