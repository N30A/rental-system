using System.Data;
using Microsoft.Data.SqlClient;

namespace api
{
    public class Program
    {
        public static string BuildConnectionString(IConfiguration configuration, IWebHostEnvironment environment)
        {   
            string? server = configuration["DB_SERVER"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(server, "Environment variable 'DB_SERVER' must be set");
            string? database = configuration["DB_DATABASE"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(database, "Environment variable 'DB_DATABASE' must be set");
            string? user = configuration["DB_USER"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(user, "Environment variable 'DB_USER' must be set");
            string? password = configuration["DB_PASSWORD"];
            ArgumentNullException.ThrowIfNullOrWhiteSpace(password, "Environment variable 'DB_PASSWORD' must be set");

            string encrypt;
            string trustCertificate;

            if (environment.IsDevelopment())
            {
                encrypt = "False";
                trustCertificate = "True";
            }
            else
            {
                encrypt = "True";
                trustCertificate = "False";
            }

            return $"Server={server};Database={database};User Id={user};Password={password};Encrypt={encrypt};TrustServerCertificate={trustCertificate};";
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Configuration.AddEnvironmentVariables();

            string connectionString = BuildConnectionString(builder.Configuration, builder.Environment);

            builder.Services.AddTransient<IDbConnection>(sc => new SqlConnection(connectionString));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
