using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using aws_lambda_netcore3_readytorun;

namespace LambdaNative
{
    public static class EntryPoint
    {
        public static void Main()
        {
            LambdaNative.Run<Handler, string, List<Member>>();
        }
    }
}
