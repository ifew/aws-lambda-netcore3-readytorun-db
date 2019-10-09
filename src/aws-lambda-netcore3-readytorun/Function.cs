using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.APIGatewayEvents;
using System.Net;
using MySql.Data.MySqlClient;
using LambdaNative;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace aws_lambda_netcore3_readytorun
{
    public class Handler : IHandler<APIGatewayProxyRequest, APIGatewayProxyResponse>
    {
        public ILambdaSerializer Serializer => new Amazon.Lambda.Serialization.Json.JsonSerializer();

        public APIGatewayProxyResponse Handle(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string unit_id = null;
            string lang = "THA";

            if (request.PathParameters != null && request.PathParameters.ContainsKey("unit_id")) {
                    unit_id = request.PathParameters["unit_id"];
            }
            if(request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey("lang")) {
                lang = request.QueryStringParameters["lang"];
            }

            Console.WriteLine("Log: Start Connection");

            string configDB = Environment.GetEnvironmentVariable("DB_CONNECTION");

            using (var _connection = new MySqlConnection(configDB))
            {
                Console.WriteLine("Log: _connection.ConnectionString: " + _connection.ConnectionString);

                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                Console.WriteLine("Log: State: " + _connection.State.ToString());
                Console.WriteLine("Log: DB ServerVersion: " + _connection.ServerVersion);

                using (var cmd = new MySqlCommand("SELECT * FROM test_member", _connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Log: reader.FieldCount: " + reader.FieldCount);
                        List<Member> members = new List<Member>();
                        while (reader.Read())
                        {
                            members.Add(new Member()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Firstname = reader["firstname"].ToString(),
                                Lastname = reader["lastname"].ToString() + lang + unit_id,
                            });
                        }

                        Console.WriteLine("Log: Count: " + members.Count);

                        APIGatewayProxyResponse respond = new APIGatewayProxyResponse {
                            StatusCode = (int)HttpStatusCode.OK,
                            Headers = new Dictionary<string, string>
                            {
                                { "Content-Type", "application/json" },
                                { "Access-Control-Allow-Origin", "*" }
                            },
                            Body = JsonConvert.SerializeObject(members)
                        };

                        return respond;
                    }
                }

            }
        }
        
    }
}
