using Amazon.Lambda.APIGatewayEvents;
using aws_lambda_netcore3_readytorun;

namespace LambdaNative
{
    public static class EntryPoint
    {
        public static void Main()
        {
            LambdaNative.RunAsync<Handler, APIGatewayProxyRequest, APIGatewayProxyResponse>();
        }
    }
}
