using System;
using System.Data.SqlClient;

namespace Dental_Management.Data
{
    public class DatabaseConnection
    {
        // Corrected connection string
        private static readonly string connectionString = "Server=Rashiid\\SQLEXPRESS;Database=DentalClinicDB;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
