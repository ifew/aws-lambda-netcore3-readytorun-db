using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using MySql.Data.MySqlClient;
using LambdaNative;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;

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
                                Lastname = reader["lastname"].ToString(),
                            });
                        }

                        Console.WriteLine("Log: Count: " + members.Count);

                        APIGatewayProxyResponse respond = new APIGatewayProxyResponse {
                            StatusCode = 200,
                            Headers = new Dictionary<string, string>
                            {
                                { "Content-Type", "application/json" },
                                { "Access-Control-Allow-Origin", "*" },
                                { "X-Debug-lang", lang },
                                { "X-Debug-unit-id", unit_id }
                            },
                            Body = System.Text.Json.JsonSerializer.Serialize<List<Member>>(members)
                        };

                        return respond;
                    }
                }

            }
        }
        
    }
}
