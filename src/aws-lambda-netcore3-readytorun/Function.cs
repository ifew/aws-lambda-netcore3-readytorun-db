using System.Runtime.CompilerServices;
using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace aws_lambda_netcore3_readytorun
{
    public class Function
    {
        private static async Task Main(string[] args)
        {
            Func<ILambdaContext, Task<List<Member>>> func = FunctionHandler;
			using(var handlerWrapper = HandlerWrapper.GetHandlerWrapper(func, new JsonSerializer()))
			{
				using(var lambdaBootstrap = new LambdaBootstrap(handlerWrapper))
				{
					await lambdaBootstrap.RunAsync();
				}
			}
        }
        
        public static List<Member> FunctionHandler(ILambdaContext context)
        {
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
                                Lastname = reader["lastname"].ToString(),
                            });
                        }

                        Console.WriteLine("Log: Count: " + members.Count);

                        return members;
                    }
                }

            }
        }
        
    }
}
