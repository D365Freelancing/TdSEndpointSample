using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

public class App
{
    static void Main(string[] args)
    {
        
        var config = new Configuration()
        {
            ClientId = "00000000-0000-0000-0000-000000000000",
            Secret = "xxxx~XXxxxxXXxxXXxxxxXXxxXXXXxxxxXXXxxx",
            TenantId = "00000000-0000-0000-0000-000000000000",
            ResourceId = "https://<environment>.crm<i>.dynamics.com/",
            SqlServer = "<environment>.crm<i>.dynamics.com",
            SqlPort = 5558
        };

        using (var connection = BuildSQLConnectionString(config))
        {
            connection.Open();

            var query = @"SELECT
                            accountId,
                            name
                        FROM account";

            var command = new SqlCommand(query,connection);
            using (var reader = command.ExecuteReader())
            {
                int count = 0;
                while (reader.Read())
                {
                    Console.WriteLine($"Count: {count}, AccountId: {reader.GetGuid(0)}, Name:{reader.GetString(1)}");
                    count++;
                }
            }               
        }

        Console.WriteLine("Press Any Key To Exit");
        Console.ReadKey();
    }
  
    private static SqlConnection BuildSQLConnectionString(Configuration config)
    {
        var app = ConfidentialClientApplicationBuilder
            .Create(config.ClientId)
            .WithClientSecret(config.Secret)
            .WithTenantId(config.TenantId)
            .Build();

        var result = app.AcquireTokenForClient(config.Scopes)
            .ExecuteAsync()
            .GetAwaiter()
            .GetResult();

        var connection = new SqlConnection(config.SqlConnectionString)
        {
            AccessToken = result.AccessToken,
        };

        return connection;
    }

    private class Configuration
    {
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string TenantId { get; set; }
        public string ResourceId { get; set; }
        public string SqlServer { get; set; }
        public int SqlPort { get; set; }
        public string[] Scopes
        {
            get
            {
                return new[] { $"{ResourceId}/.default" };
            }
        }
        public string SqlConnectionString
        {
            get
            {
                return $"Server={SqlServer}, {SqlPort}";
            }
        }
    }
}