﻿using System.Data;
using api.Repositories;
using api.Validators;
using Microsoft.Data.SqlClient;

namespace api
{
    public static class ServiceExtensions
    {
        private static string GetEnvironmentVariablesOrThrow(IConfiguration configuration, string key)
        {
            var value = configuration[key];

            if (string.IsNullOrWhiteSpace(value))
            {   
                throw new InvalidOperationException($"Environment variable '{key}' is required but was not set.");
            }

            return value;
        }

        private static string BuildConnectionString(IConfiguration configuration, IWebHostEnvironment environment)
        {
            string server = GetEnvironmentVariablesOrThrow(configuration, "DB_SERVER");
            string database = GetEnvironmentVariablesOrThrow(configuration, "DB_DATABASE");
            string user = GetEnvironmentVariablesOrThrow(configuration, "DB_USER");
            string password = GetEnvironmentVariablesOrThrow(configuration, "DB_PASSWORD");

            string encrypt = environment.IsDevelopment() ? "False" : "True";
            string trustCertificate = environment.IsDevelopment() ? "True" : "False";

            return $"Server={server};Database={database};User Id={user};Password={password};Encrypt={encrypt};TrustServerCertificate={trustCertificate};";
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            string connectionString = BuildConnectionString(configuration, environment);

            services.AddTransient<IDbConnection>(sc => new SqlConnection(connectionString));

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<AddressRepository>();

            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<AddressCreateValidator>();
            services.AddScoped<AddressUpdateValidator>();

            return services;
        }
    }
}
